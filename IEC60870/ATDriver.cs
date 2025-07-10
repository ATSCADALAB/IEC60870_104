using ATDriver_Server;
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
                // Tra ve ket qua null (BAD)
                if (this.currentReader is null ||
                    !this.currentReader.ConnectionStatus ||
                    !GetIOAddress(TagAddress, TagType, out IOAddress address)) return default;

                // Doc multi truoc....
                this.currentReader.ReadMulti();
                // ... Sau do, doc single
                // Gia tri doc tu Buffer hoac truc tiep tu Device
                if (this.currentReader.Read(address, out string value))
                    return new SendPack()
                    {
                        ChannelAddress = ChannelAddress,
                        DeviceID = DeviceID,
                        TagAddress = TagAddress,
                        TagType = TagType,
                        Value = value
                    };

                return default;
            }
            catch
            {
                return default;
            }
        }

        #endregion

        #region WRITE

        public string Write(SendPack sendPack)
        {
            try
            {
                if (this.currentReader is null ||
                    !this.currentReader.ConnectionStatus ||
                    !GetIOAddress(sendPack.TagAddress, sendPack.TagType, out IOAddress address))
                    return WriteBad;

                var deviceReader = this.deviceReaders.FirstOrDefault(x => x.DeviceID == deviceID);
                if (deviceReader is null) return WriteBad;

                return
                    deviceReader is null ? WriteBad :
                    deviceReader.Write(address, sendPack.Value.Trim()) ? WriteGood :
                    WriteBad;
            }
            catch
            {
                return WriteBad;
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