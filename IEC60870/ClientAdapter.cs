using System;
using System.Threading;
using IEC60870.SAP;
using IEC60870.Object;
using IEC60870.Enum;

namespace IEC60870Driver
{
    /// <summary>
    /// Ho tro cac ham lien quan den ket noi cua IEC60870 Client
    /// </summary>
    public class ClientAdapter : IDisposable
    {
        #region FIELDS

        private const int MaxTryConnection = 2;
        private const int MaxTryRead = 2;
        private const int MaxTryWrite = 2;

        private readonly object keyLock = new object();
        private DateTime lastConnected;

        #endregion

        #region PROPERTIES

        public string Name { get; set; }

        /// <summary>
        /// Chu ky song cua ket noi
        /// Neu qua thoi gian tren. 
        /// Client se ket noi lai voi Server (RTU) bat ke trang thai ket noi
        /// </summary>
        public uint Lifetime { get; set; } = 3600;

        public IEC60870Client Client { get; }

        /// <summary>
        /// Trang thai ket noi
        /// </summary>
        public bool Connected => Client != null && Client.IsConnected;

        #endregion

        #region CONSTRUCTORS

        public ClientAdapter(string name, IEC60870Client client)
        {
            Name = name;
            Client = client;
            this.lastConnected = DateTime.MinValue;
        }

        #endregion

        #region CONNECTION FUNCTION

        /// <summary>
        /// Kiem tra thoi gian ket noi cua Client
        /// Neu = 0. Duy tri trang thai ket noi hien tai
        /// Other. Neu qua thoi gian Lifetime. Ket noi lai
        /// </summary>
        public void CheckLifeTime()
        {
            if (Lifetime == 0) return;
            var timeSpan = DateTime.Now - this.lastConnected;
            if (timeSpan.TotalSeconds >= Lifetime)
                Reconnect();
        }

        /// <summary>
        /// Kiem tra trang thai ket noi
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            CheckLifeTime();
            return Connected;
        }

        /// <summary>
        /// Ket noi den Server
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            for (var i = 0; i < MaxTryConnection; i++)
            {
                if (Client.Connect())
                {
                    this.lastConnected = DateTime.Now;
                    return true;
                }
                Thread.Sleep(100);
            }
            return false;
        }

        /// <summary>
        /// Ngat ket noi den Server
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                Client?.Disconnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ket noi lai
        /// </summary>
        public void Reconnect()
        {
            Disconnect();
            Thread.Sleep(100);
            Connect();
        }

        #endregion

        #region READ/WRITE FUNCTIONS

        /// <summary>
        /// Doc du lieu tu IEC60870 device
        /// </summary>
        public bool Read(int commonAddress, IOAddress ioAddress, out object value)
        {
            value = null;

            if (!CheckConnection())
            {
                return false;
            }

            for (int i = 0; i < MaxTryRead; i++)
            {
                lock (this.keyLock)
                {
                    try
                    {
                        // Send General Interrogation (không gửi cho specific IOA)
                        Client.SendInterrogation(commonAddress, 0);

                        // Đợi response đầy đủ
                        Thread.Sleep(2000); // Tăng lên 2 giây

                        // Kiểm tra buffer nhiều lần
                        for (int retry = 0; retry < 10; retry++)
                        {
                            if (Client.GetValue(ioAddress.InformationObjectAddress, out value))
                            {
                                return true;
                            }
                            Thread.Sleep(200); // Đợi thêm 200ms mỗi lần retry
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                }
                Thread.Sleep(500); // Đợi trước khi thử lại
            }

            return false;
        }
        public bool ReadSmart(int commonAddress, int ioa, out object value, out TypeId detectedTypeId)
        {
            value = null;
            detectedTypeId = default(TypeId);

            if (!CheckConnection()) return false;

            for (int i = 0; i < MaxTryRead; i++)
            {
                lock (this.keyLock)
                {
                    try
                    {
                        Client.SendInterrogation(commonAddress, 0);
                        Thread.Sleep(2000);

                        // ✅ ĐÚNG: out TypeId (không nullable)
                        if (Client.GetValueSmart(ioa, out value, out detectedTypeId))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] ReadSmart attempt {i + 1} failed: {ex.Message}");
                    }
                }
                Thread.Sleep(500);
            }
            return false;
        }
        public bool Write(int commonAddress, IOAddress ioAddress, string value)
        {
            if (!CheckConnection()) return false;

            for (int i = 0; i < MaxTryWrite; i++)
            {
                lock (this.keyLock)
                {
                    try
                    {
                        return Client.SendCommand(commonAddress, ioAddress.InformationObjectAddress, ioAddress.TypeId, value);
                    }
                    catch (Exception)
                    {
                        // Handle write error
                    }
                }
                Thread.Sleep(10);
            }
            return false;
        }

        #endregion

        public void Dispose()
        {
            try
            {
                Client?.Disconnect();
                Client?.Dispose();
            }
            catch { }
        }
    }
}
