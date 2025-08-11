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

        private readonly object keyLock = new object();
        private DateTime lastConnected;
        private int reconnectFailures = 0;

        // THÊM: Reference đến DeviceSettings để lấy timeout configs
        private DeviceSettings deviceSettings;

        #endregion

        #region PROPERTIES

        public string Name { get; set; }

        /// <summary>
        /// Chu ky song cua ket noi
        /// Neu qua thoi gian tren. 
        /// Client se ket noi lai voi Server (RTU) bat ke trang thai ket noi
        /// </summary>
        public uint Lifetime { get; set; } = 3600;

        public IEC60870Client Client { get; private set; }

        /// <summary>
        /// Trang thai ket noi
        /// </summary>
        public bool Connected => Client != null && Client.IsConnected;

        #endregion

        #region CONSTRUCTORS

        public ClientAdapter(string name, IEC60870Client client, DeviceSettings settings = null)
        {
            Name = name;
            Client = client;
            this.lastConnected = DateTime.MinValue;
            this.deviceSettings = settings;
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
        /// Ket noi den Server với configurable retry
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            int maxTryConnection = deviceSettings?.MaxRetryCount ?? 2;
            int retryDelay = deviceSettings?.RetryDelay ?? 100;

            for (var i = 0; i < maxTryConnection; i++)
            {
                if (Client.Connect())
                {
                    this.lastConnected = DateTime.Now;
                    reconnectFailures = 0; // reset backoff on success
                    Console.WriteLine($"[SUCCESS] ClientAdapter connected on attempt {i + 1}");
                    return true;
                }

                if (i < maxTryConnection - 1) // Không sleep ở lần cuối
                {
                    Console.WriteLine($"[RETRY] Connection attempt {i + 1} failed, retrying in {retryDelay}ms");
                    Thread.Sleep(retryDelay);
                }
            }

            Console.WriteLine($"[FAILED] ClientAdapter connection failed after {maxTryConnection} attempts");
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
        /// Ket noi lai với configurable delay
        /// </summary>
        public void Reconnect()
        {
            Console.WriteLine("[INFO] ClientAdapter reconnecting...");
            try
            {
                try { Client?.Disconnect(); } catch { }
                try { Client?.Dispose(); } catch { }
            }
            catch { }

            // Exponential backoff to give server time to drop old session
            int baseDelay = deviceSettings?.RetryDelay ?? 200;
            int backoff = baseDelay * (int)Math.Pow(2, Math.Min(reconnectFailures, 5)); // cap exponent
            int delayMs = Math.Min(Math.Max(backoff, 500), 10000); // clamp 0.5s..10s
            Thread.Sleep(delayMs);

            // Recreate client instance to avoid stale socket/state during reconnect
            if (deviceSettings != null)
            {
                Client = new IEC60870Client
                {
                    IpAddress = deviceSettings.IpAddress,
                    Port = deviceSettings.Port,
                    CommonAddress = deviceSettings.CommonAddress,
                    OriginatorAddress = deviceSettings.OriginatorAddress,
                    CotFieldLength = deviceSettings.CotFieldLength,
                    CommonAddressFieldLength = deviceSettings.CommonAddressFieldLength,
                    IoaFieldLength = deviceSettings.IoaFieldLength
                };
            }
            // If settings are missing, fall back to the existing instance
            bool ok = Connect();
            if (!ok) reconnectFailures++;
        }

        /// <summary>
        /// Cập nhật DeviceSettings (để thay đổi timeout runtime)
        /// </summary>
        public void UpdateSettings(DeviceSettings settings)
        {
            this.deviceSettings = settings;
        }

        #endregion

        #region READ/WRITE FUNCTIONS

        /// <summary>
        /// Doc du lieu tu IEC60870 device với configurable timeout
        /// </summary>
        public bool Read(int commonAddress, IOAddress ioAddress, out object value)
        {
            value = null;

            if (!CheckConnection())
            {
                return false;
            }

            int maxTryRead = deviceSettings?.MaxRetryCount ?? 2;
            int readTimeout = deviceSettings?.ReadTimeout ?? 5000;
            int retryDelay = deviceSettings?.RetryDelay ?? 500;

            for (int i = 0; i < maxTryRead; i++)
            {
                lock (this.keyLock)
                {
                    try
                    {
                        // Send General Interrogation
                        Client.SendInterrogation(commonAddress, 0);

                        // Polling với timeout từ settings
                        int pollInterval = 100;
                        int maxPolls = readTimeout / pollInterval;

                        for (int poll = 0; poll < maxPolls; poll++)
                        {
                            Thread.Sleep(pollInterval);

                            if (Client.GetValue(ioAddress.InformationObjectAddress, out value))
                            {
                                Console.WriteLine($"[SUCCESS] Read IOA {ioAddress.InformationObjectAddress} after {(poll + 1) * pollInterval}ms");
                                return true;
                            }
                        }

                        Console.WriteLine($"[TIMEOUT] Read IOA {ioAddress.InformationObjectAddress} - No data after {readTimeout}ms");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Read IOA {ioAddress.InformationObjectAddress} attempt {i + 1}: {ex.Message}");
                    }
                }

                if (i < maxTryRead - 1) // Không sleep ở lần cuối
                {
                    Thread.Sleep(retryDelay);
                }
            }

            return false;
        }
        public bool ReadSmart(int commonAddress, int ioa, out object value, out TypeId detectedTypeId)
        {
            value = null;
            detectedTypeId = default(TypeId);

            if (!CheckConnection()) return false;

            int maxTryRead = deviceSettings?.MaxRetryCount ?? 2;
            int interrogationTimeout = deviceSettings?.InterrogationTimeout ?? 1000;
            int retryDelay = deviceSettings?.RetryDelay ?? 500;

            for (int i = 0; i < maxTryRead; i++)
            {
                lock (this.keyLock)
                {
                    try
                    {
                        // Gửi interrogation
                        Client.SendInterrogation(commonAddress, 0);

                        // Polling với timeout từ settings
                        int pollInterval = 100;
                        int maxPolls = interrogationTimeout / pollInterval;

                        for (int poll = 0; poll < maxPolls; poll++)
                        {
                            Thread.Sleep(pollInterval);

                            if (Client.GetValueSmart(ioa, out value, out detectedTypeId))
                            {
                                Console.WriteLine($"[SUCCESS] ReadSmart IOA {ioa} after {(poll + 1) * pollInterval}ms");
                                return true;
                            }
                        }

                        Console.WriteLine($"[TIMEOUT] ReadSmart IOA {ioa} - No data after {interrogationTimeout}ms");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] ReadSmart IOA {ioa} attempt {i + 1}: {ex.Message}");
                    }
                }

                if (i < maxTryRead - 1) // Không sleep ở lần cuối
                {
                    Thread.Sleep(retryDelay);
                }
            }

            return false;
        }
        public bool Write(int commonAddress, IOAddress ioAddress, string value)
        {
            if (!CheckConnection()) return false;

            int maxTryWrite = deviceSettings?.MaxRetryCount ?? 2;
            int retryDelay = deviceSettings?.RetryDelay ?? 500;

            for (int i = 0; i < maxTryWrite; i++)
            {
                lock (this.keyLock)
                {
                    try
                    {
                        bool result = Client.SendCommand(commonAddress, ioAddress.InformationObjectAddress, ioAddress.TypeId, value);
                        if (result)
                        {
                            Console.WriteLine($"[SUCCESS] Write IOA {ioAddress.InformationObjectAddress} = {value} on attempt {i + 1}");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Write IOA {ioAddress.InformationObjectAddress} attempt {i + 1}: {ex.Message}");
                    }
                }

                if (i < maxTryWrite - 1) // Không sleep ở lần cuối
                {
                    Thread.Sleep(retryDelay);
                }
            }

            Console.WriteLine($"[FAILED] Write IOA {ioAddress.InformationObjectAddress} after {maxTryWrite} attempts");
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
