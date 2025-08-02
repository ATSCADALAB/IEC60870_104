using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace IEC60870Driver
{
    public class DeviceReader
    {
        #region FIELDS 

        private int readTimes;
        private readonly ATDriver driver;
        private readonly List<BlockReader> blockReaders;
        private ClientAdapter clientAdapter;
        private DateTime lastPingTest = DateTime.MinValue;
        private bool lastPingResult = false;
        private readonly object pingLock = new object();
        #endregion

        #region PROPERTIES

        public string DeviceName { get; set; }
        public string DeviceID { get; set; }
        public DeviceSettings Settings { get; set; }

        /// <summary>
        /// Trang thai ket noi
        /// </summary>
        public bool ConnectionStatus { get; private set; }

        #endregion

        public DeviceReader(ATDriver driver)
        {
            this.driver = driver;
            this.blockReaders = new List<BlockReader>();
        }

        /// <summary>
        /// Khoi tao ban dau. Tao danh sach cac Block can doc Multi
        /// </summary>
        public void Initialize()
        {
            if (Settings is null) return;
            this.clientAdapter = this.driver.TryGetClientAdapter(Settings.ClientID, out ClientAdapter adapter) ?
                adapter : this.driver.AddClientAdapter(Settings);

            if (string.IsNullOrEmpty(Settings.BlockSettings)) return;
            this.blockReaders.Clear();
            foreach (var blockSetting in Settings.BlockSettings.Split('/'))
                if (!string.IsNullOrEmpty(blockSetting.Trim()))
                    this.blockReaders.AddRange(BlockReader.Initialize(blockSetting));

            this.readTimes = Settings.MaxReadTimes;
        }

        public bool CheckConnection()
        {
            if (Settings == null) return false;

            lock (pingLock)
            {
                // Cache ping result trong 10 giây để tránh ping quá thường xuyên
                if (DateTime.Now - lastPingTest < TimeSpan.FromSeconds(10))
                {
                    ConnectionStatus = lastPingResult && this.clientAdapter?.CheckConnection() == true;
                    return ConnectionStatus;
                }

                // ✅ BƯỚC 1: Test ping trước
                lastPingTest = DateTime.Now;
                lastPingResult = PingDevice(Settings.IpAddress);

                if (!lastPingResult)
                {
                    Console.WriteLine($"[WARNING] Device {DeviceName} ({Settings.IpAddress}) not reachable via ping");
                    ConnectionStatus = false;
                    return false;
                }

                // ✅ BƯỚC 2: Nếu ping OK, kiểm tra IEC60870 connection
                bool iecConnection = this.clientAdapter?.CheckConnection() == true;

                if (!iecConnection && lastPingResult)
                {
                    Console.WriteLine($"[INFO] Device {DeviceName} ping OK but IEC60870 failed. Attempting reconnect...");
                    // Ping OK nhưng IEC60870 fail → reconnect
                    Task.Run(() => ReconnectDevice());
                }

                ConnectionStatus = lastPingResult && iecConnection;
                return ConnectionStatus;
            }
        }
        private void ReconnectDevice()
        {
            try
            {
                Console.WriteLine($"[INFO] Reconnecting device {DeviceName}...");

                // Đợi một chút để tránh reconnect quá nhanh
                Thread.Sleep(1000);

                // Force reconnect ClientAdapter
                if (this.clientAdapter != null)
                {
                    this.clientAdapter.Reconnect();

                    // Test lại sau khi reconnect
                    Thread.Sleep(2000);
                    bool reconnectSuccess = this.clientAdapter.CheckConnection();

                    if (reconnectSuccess)
                    {
                        Console.WriteLine($"[SUCCESS] Device {DeviceName} reconnected successfully");
                        ConnectionStatus = true;
                        // Reset cache để test ngay lần tới
                        lastPingTest = DateTime.MinValue;
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR] Device {DeviceName} reconnect failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Device reconnect exception: {ex.Message}");
            }
        }
        private bool PingDevice(string ipAddress, int timeoutMs = 3000)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send(ipAddress, timeoutMs);
                    bool success = reply.Status == IPStatus.Success;

                    if (success)
                    {
                        Console.WriteLine($"[PING] {ipAddress} - OK ({reply.RoundtripTime}ms)");
                    }
                    else
                    {
                        Console.WriteLine($"[PING] {ipAddress} - FAILED ({reply.Status})");
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PING] {ipAddress} - ERROR: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ham doc multi
        /// </summary>
        public void ReadMulti()
        {
            if (this.blockReaders.Count == 0) return;
            this.readTimes++;

            // Ap dung voi co che doc Multi
            // Khi doc Multi. Du lieu se duoc luu vao trong Buffer
            // Moi lan read Tag. Se duoc lay data tu Buffer (neu co).
            // Sau MaxReadTimes lan. Se doc lai Multi 1 lan de cap nhat gia tri moi
            if (this.readTimes <= Settings.MaxReadTimes) return;
            this.readTimes = 1;

            // Doc tuan tu cac Block
            // Ket qua tra ve se duoc luu vao Buffer
            foreach (var blockReader in this.blockReaders)
            {
                if (ConnectionStatus)
                    blockReader.ReadBlock(this.clientAdapter, Settings.CommonAddress);
            }
        }

        /// <summary>
        /// Ham doc single tag
        /// </summary>
        public bool Read(IOAddress ioAddress, out string value)
        {
            value = "0";

            // Tim trong buffer truoc (neu co Multi read)
            foreach (var blockReader in this.blockReaders)
            {
                if (blockReader.Contains(ioAddress.InformationObjectAddress))
                {
                    value = blockReader.GetValue(ioAddress.InformationObjectAddress);
                    return true;
                }
            }

            if (!CheckConnection())
            {
                return false;
            }

            return this.clientAdapter.Read(Settings.CommonAddress, ioAddress, out object objValue) &&
                   ConvertValue(objValue, ioAddress.DataType, out value);
        }

        /// <summary>
        /// Ham ghi gia tri
        /// </summary>
        public bool Write(IOAddress ioAddress, string value)
        {
            if (!CheckConnection())
            {
                return false;
            }

            return this.clientAdapter.Write(Settings.CommonAddress, ioAddress, value);
        }

        private bool ConvertValue(object objValue, DataType dataType, out string value)
        {
            value = "0";
            try
            {
                if (objValue == null) return false;

                switch (dataType)
                {
                    case DataType.Bool:
                        value = Convert.ToBoolean(objValue) ? "1" : "0";
                        break;
                    case DataType.Word:
                    case DataType.Int:
                        value = Convert.ToInt32(objValue).ToString();
                        break;
                    case DataType.DWord:
                        value = Convert.ToUInt32(objValue).ToString();
                        break;
                    case DataType.Float:
                        value = Convert.ToSingle(objValue).ToString();
                        break;
                    default:
                        value = objValue.ToString();
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}