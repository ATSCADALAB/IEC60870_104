using System;

namespace IEC60870Driver
{
    public class DeviceSettings
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int CommonAddress { get; set; }
        public int OriginatorAddress { get; set; }
        public int CotFieldLength { get; set; }
        public int CommonAddressFieldLength { get; set; }
        public int IoaFieldLength { get; set; }
        public int TimeOut { get; set; } = 5000;
        public int MaxReadTimes { get; set; } = 1;
        public string BlockSettings { get; set; }
        public string ClientID => $"{IpAddress}:{Port}";

        public static DeviceSettings Initialize(string deviceID)
        {
            try
            {
                var parts = deviceID.Split('|');
                if (parts.Length < 7) return null;

                var settings = new DeviceSettings
                {
                    IpAddress = parts[0],
                    Port = int.Parse(parts[1]),
                    CommonAddress = int.Parse(parts[2]),
                    OriginatorAddress = int.Parse(parts[3]),
                    CotFieldLength = int.Parse(parts[4]),
                    CommonAddressFieldLength = int.Parse(parts[5]),
                    IoaFieldLength = int.Parse(parts[6]),
                    MaxReadTimes = parts.Length > 7 ? int.Parse(parts[7]) : 1,
                    BlockSettings = parts.Length > 8 ? parts[8] : ""
                };

                return settings;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}