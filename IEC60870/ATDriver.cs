using ATDriver_Server;
using IEC60870.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IEC60870Driver
{
    public class ATDriver : IATDriver, IDisposable
    {
        #region CONSTANTS

        private const string WriteGood = "Good";
        private const string WriteBad = "Bad";

        #endregion

        #region FIELDS

        private readonly List<DeviceReader> deviceReaders;
        private readonly Dictionary<string, DeviceSettings> deviceSettingMapping;
        private readonly Dictionary<string, IOAddress> addressMapping;
        private readonly List<ClientAdapter> clientAdapters;
        private readonly object editLock = new object();

        private uint lifetime = 3600;
        private string deviceID;
        private DeviceReader currentReader;

        #endregion

        #region PROPERTIES

        public ATDriverTypes ATDriverType => ATDriverTypes.TCPIP;
        public ErrorCodes Error { get; set; }
        public string ChannelName { get; set; }

        public string ChannelAddress
        {
            get => this.lifetime.ToString();
            set
            {
                if (!uint.TryParse(value, out uint time))
                    time = 60;
                if (this.lifetime == time)
                {
                    foreach (var adapter in this.clientAdapters)
                        adapter.CheckLifeTime();
                    return;
                }

                this.lifetime = time;
                foreach (var adapter in this.clientAdapters)
                    adapter.Lifetime = this.lifetime;
            }
        }

        public string DeviceName { get; set; }

        public string DeviceID
        {
            get => this.deviceID;
            set
            {
                this.deviceID = value;

                if (string.IsNullOrEmpty(this.deviceID)) return;
                if (!this.deviceSettingMapping.ContainsKey(this.deviceID))
                    this.deviceSettingMapping[this.deviceID] = DeviceSettings.Initialize(this.deviceID);
                var deviceSettings = this.deviceSettingMapping[this.deviceID];
                if (deviceSettings is null) return;

                if (!CreateDeviceReaderIfNotExist(DeviceName, this.deviceID, deviceSettings))
                    if (!RemoveDeviceReaderIfNotUse(DeviceName, deviceSettings))
                        UpdateDeviceReaderIfChanged(DeviceName, deviceSettings);

                this.currentReader = this.deviceReaders.FirstOrDefault(x => x.DeviceID == DeviceID);
                if (this.currentReader is null) return;
                this.currentReader.CheckConnection();
            }
        }

        public string TagName { get; set; }
        public string TagAddress { get; set; }
        public string TagType { get; set; }
        public string TagClientAccess { get; set; }
        public string TagDescription { get; set; }

        // Design Mode Properties
        public string DeviceNameDesignMode { get; set; }
        public string DeviceIDDesignMode { get; set; }
        public string TagNameDesignMode { get; set; }
        public string TagAddressDesignMode { get; set; }
        public string TagTypeDesignMode { get; set; }
        public string TagClientAccessDesignMode { get; set; }

        public UserControl ctlChannelAddress => new ctlChannelAddress(this);
        public UserControl ctlDeviceDesign => new ctlDeviceDesign(this);
        public UserControl ctlTagDesign => new ctlTagDesign(this);

        #endregion

        #region CONSTRUCTORS

        public ATDriver()
        {
            this.deviceReaders = new List<DeviceReader>();
            this.deviceSettingMapping = new Dictionary<string, DeviceSettings>();
            this.addressMapping = new Dictionary<string, IOAddress>();
            this.clientAdapters = new List<ClientAdapter>();
        }

        #endregion

        #region CONNECTION

        public bool Connect() { return true; }

        public bool Disconnect()
        {
            var result = true;
            foreach (var clientAdapter in this.clientAdapters)
            {
                result = clientAdapter.Disconnect();
            }
            return result;
        }

        public void Dispose()
        {
            foreach (var clientAdapter in this.clientAdapters)
            {
                clientAdapter.Dispose();
            }
        }

        #endregion

        #region READ

        public SendPack Read()
        {
            try
            {
                if (this.currentReader is null || !this.currentReader.ConnectionStatus)
                    return default;

                // ✅ PARSE TagAddress thành IOA number
                if (int.TryParse(TagAddress, out int ioa))
                {

                    // ✅ SỬ DỤNG ReadByIOA để auto-detect TypeId
                    var smartResult = ReadByIOA(ioa);
                    if (smartResult != null)
                    {
                        // Return với TagAddress gốc (IOA number), không phải TypeId:IOA
                        return new SendPack()
                        {
                            ChannelAddress = ChannelAddress,
                            DeviceID = DeviceID,
                            TagAddress = TagAddress,      // ✅ Giữ nguyên "400001"
                            TagType = smartResult.TagType, // ✅ Auto-detected type
                            Value = smartResult.Value
                        };
                    }
                }

                // ✅ FALLBACK: Nếu TagAddress không phải số, dùng cách cũ
                if (!GetIOAddress(TagAddress, TagType, out IOAddress address))
                    return default;

                this.currentReader.ReadMulti();

                if (this.currentReader.Read(address, out string value))
                {
                    return new SendPack()
                    {
                        ChannelAddress = ChannelAddress,
                        DeviceID = DeviceID,
                        TagAddress = TagAddress,
                        TagType = TagType,
                        Value = value
                    };
                }

                return default;
            }
            catch
            {
                return default;
            }
        }
        public SendPack ReadByIOA(int ioa)
        {
            try
            {
                if (this.currentReader == null || !this.currentReader.ConnectionStatus)
                    return null;

                var clientAdapter = this.clientAdapters.FirstOrDefault(x => x.Name == this.currentReader.Settings.ClientID);
                if (clientAdapter != null)
                {
                    // ✅ ĐÚNG: out TypeId (không nullable)
                    if (clientAdapter.ReadSmart(this.currentReader.Settings.CommonAddress, ioa, out object value, out TypeId detectedTypeId))
                    {
                        string dataType = GetDataTypeFromTypeId(detectedTypeId);
                        string address = detectedTypeId.ToString() + ":" + ioa.ToString();

                        return new SendPack()
                        {
                            ChannelAddress = ChannelAddress,
                            DeviceID = DeviceID,
                            TagAddress = address,
                            TagType = dataType,
                            Value = ConvertValueToString(value, dataType)
                        };
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // THÊM helper methods
        private string GetDataTypeFromTypeId(TypeId typeId)
        {
            switch (typeId)
            {
                case TypeId.M_SP_NA_1:
                case TypeId.M_DP_NA_1:
                    return "Bool";
                case TypeId.M_ME_NA_1:
                case TypeId.M_ME_NB_1:
                    return "Int";
                case TypeId.M_ME_NC_1:
                    return "Float";
                case TypeId.M_IT_NA_1:
                    return "DWord";
                default:
                    return "String";
            }
        }

        private string ConvertValueToString(object value, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "Bool":
                        return Convert.ToBoolean(value) ? "1" : "0";
                    case "Int":
                        return Convert.ToInt32(value).ToString();
                    case "Float":
                        return Convert.ToSingle(value).ToString("F2");
                    case "DWord":
                        return Convert.ToUInt32(value).ToString();
                    default:
                        return value?.ToString() ?? "0";
                }
            }
            catch
            {
                return value?.ToString() ?? "0";
            }
        }
        #endregion

        #region WRITE

        public string Write(SendPack sendPack)
        {
            try
            {
                if (this.currentReader is null || !this.currentReader.ConnectionStatus)
                    return WriteBad;

                // ✅ THÊM: Kiểm tra nếu TagAddress là IOA number
                if (int.TryParse(sendPack.TagAddress, out int ioa))
                {
                    Console.WriteLine($"[DEBUG] Using smart write for IOA: {sendPack.TagAddress}");

                    // Smart write với auto-detect TypeId
                    return WriteSmartIOA(ioa, sendPack.Value, sendPack.TagType);
                }

                // ✅ GIỮ NGUYÊN: Cách cũ cho format "TypeId:IOA"
                if (!GetIOAddress(sendPack.TagAddress, sendPack.TagType, out IOAddress address))
                    return WriteBad;

                var deviceReader = this.deviceReaders.FirstOrDefault(x => x.DeviceID == DeviceID);
                return deviceReader is null ? WriteBad :
                       deviceReader.Write(address, sendPack.Value.Trim()) ? WriteGood : WriteBad;
            }
            catch
            {
                return WriteBad;
            }
        }

        // SỬA WriteSmartIOA() - VERSION TỐI ƯU
        private string WriteSmartIOA(int ioa, string value, string hintType)
        {
            try
            {
                // ✅ SỬ DỤNG HINT TYPE TRỰC TIẾP - KHÔNG ĐỌC TRƯỚC
                string targetTagType = hintType ?? GuessTypeFromValue(value);

                // Map sang write command TypeId
                var typeId = GetTypeIdFromDataType(targetTagType, true);
                var address = new IOAddress(ioa, typeId, GetDataTypeEnum(targetTagType));

                Console.WriteLine($"[DEBUG] Writing IOA {ioa} as {typeId}: {value}");

                // Execute write ngay lập tức
                var deviceReader = this.deviceReaders.FirstOrDefault(x => x.DeviceID == DeviceID);
                return deviceReader is null ? WriteBad :
                       deviceReader.Write(address, value.Trim()) ? WriteGood : WriteBad;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] WriteSmartIOA failed: {ex.Message}");
                return WriteBad;
            }
        }

        // ✅ IMPROVE GuessTypeFromValue
        private string GuessTypeFromValue(string value)
        {
            // More accurate guessing
            if (value == "0" || value == "1" || value.ToLower() == "true" || value.ToLower() == "false")
                return "Bool";

            if (int.TryParse(value, out int intVal))
            {
                if (intVal >= 0 && intVal <= 65535)
                    return "Word";
                else
                    return "Int";
            }

            if (float.TryParse(value, out _))
                return "Float";

            return "String";
        }

        // ✅ COMPLETE GetTypeIdFromDataType mapping
        private TypeId GetTypeIdFromDataType(string dataType, bool isWrite = false)
        {
            if (isWrite)
            {
                // Write commands
                switch (dataType)
                {
                    case "Bool":
                        return TypeId.C_SC_NA_1; // Single Command
                    case "Int":
                        return TypeId.C_SE_NA_1; // Set Point Normalized
                    case "Word":
                        return TypeId.C_SE_NA_1; // Set Point Normalized
                    case "Float":
                        return TypeId.C_SE_NC_1; // Set Point Float
                    case "DWord":
                        return TypeId.C_SE_NB_1; // Set Point Scaled
                    default:
                        return TypeId.C_SC_NA_1; // Default to Single Command
                }
            }
            else
            {
                // Read types
                switch (dataType)
                {
                    case "Bool":
                        return TypeId.M_SP_NA_1; // Single Point
                    case "Int":
                        return TypeId.M_ME_NA_1; // Normalized Value
                    case "Word":
                        return TypeId.M_ME_NB_1; // Scaled Value
                    case "Float":
                        return TypeId.M_ME_NC_1; // Short Float
                    case "DWord":
                        return TypeId.M_IT_NA_1; // Integrated Total
                    default:
                        return TypeId.M_SP_NA_1;
                }
            }
        }

        private DataType GetDataTypeEnum(string dataTypeString)
        {
            switch (dataTypeString)
            {
                case "Bool": return DataType.Bool;
                case "Int": return DataType.Int;
                case "Word": return DataType.Word;
                case "Float": return DataType.Float;
                case "DWord": return DataType.DWord;
                case "String": return DataType.String;
                default: return DataType.Default;
            }
        }

        public bool GetIOAddress(string tagAddress, string tagType, out IOAddress address)
        {
            var dataType = tagType.GetDataType();
            var addressKey = $"{tagAddress}-{tagType}";
            if (!this.addressMapping.ContainsKey(addressKey))
                this.addressMapping[addressKey] = tagAddress.GetIOAddress(dataType, out _);
            address = this.addressMapping[addressKey];
            if (address is null) return false;

            return true;
        }

        #endregion

        #region CLIENT ADAPTER

        public ClientAdapter AddClientAdapter(DeviceSettings deviceSettings)
        {
            lock (this.editLock)
            {
                var client = new IEC60870Client()
                {
                    IpAddress = deviceSettings.IpAddress,
                    Port = deviceSettings.Port,
                    CommonAddress = deviceSettings.CommonAddress,
                    OriginatorAddress = deviceSettings.OriginatorAddress
                };
                var clientAdapter = new ClientAdapter(deviceSettings.ClientID, client)
                {
                    Lifetime = this.lifetime
                };
                clientAdapter.Connect();
                this.clientAdapters.Add(clientAdapter);
                return clientAdapter;
            }
        }

        public bool TryGetClientAdapter(string name, out ClientAdapter adapter)
        {
            lock (this.editLock)
            {
                adapter = this.clientAdapters.FirstOrDefault(x => x.Name == name);
                return adapter != null;
            }
        }

        #endregion

        #region DEVICE READER MANAGEMENT

        private bool CreateDeviceReaderIfNotExist(string deviceName, string deviceID, DeviceSettings deviceSettings)
        {
            try
            {
                var index = this.deviceReaders.FindIndex(x => x.DeviceID == deviceID);
                if (index < 0)
                {
                    lock (this.editLock)
                    {
                        var deviceReader = new DeviceReader(this)
                        {
                            DeviceName = deviceName,
                            DeviceID = deviceID,
                            Settings = deviceSettings
                        };
                        this.deviceReaders.Add(deviceReader);
                        deviceReader.Initialize();
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        private bool RemoveDeviceReaderIfNotUse(string deviceName, DeviceSettings deviceSettings)
        {
            try
            {
                var index = this.deviceReaders.FindIndex(x =>
                    x.DeviceName == deviceName &&
                    (x.Settings.IpAddress != deviceSettings.IpAddress ||
                    x.Settings.Port != deviceSettings.Port));
                if (index > -1)
                {
                    var deviceReader = this.deviceReaders[index];
                    this.deviceReaders.Remove(deviceReader);
                    if (TryGetClientAdapter(deviceReader.Settings.ClientID, out ClientAdapter adapter))
                    {
                        lock (this.editLock)
                        {
                            this.clientAdapters.Remove(adapter);
                            adapter?.Dispose();
                        }
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        private bool UpdateDeviceReaderIfChanged(string deviceName, DeviceSettings deviceSettings)
        {
            try
            {
                var index = this.deviceReaders.FindIndex(x =>
                    x.DeviceName == deviceName &&
                    x.Settings.IpAddress == deviceSettings.IpAddress &&
                    x.Settings.Port == deviceSettings.Port &&
                    x.Settings.BlockSettings != deviceSettings.BlockSettings);
                if (index > -1)
                {
                    lock (editLock)
                    {
                        this.deviceReaders[index].Settings = deviceSettings;
                        this.deviceReaders[index].DeviceID = DeviceID;
                        this.deviceReaders[index].Initialize();
                    }
                }
                return true;
            }
            catch { }
            return false;
        }

        #endregion
    }
}