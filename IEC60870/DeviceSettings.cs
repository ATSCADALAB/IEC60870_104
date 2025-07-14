using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

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
                if (string.IsNullOrWhiteSpace(deviceID))
                    return null;

                // Format: "IP|Port|CommonAddress|OriginatorAddress|CotFieldLength|CAFieldLength|IOAFieldLength|MaxReadTimes|BlockSettings"
                // Example: "192.168.1.100|2404|1|0|1|1|2|1|M_SP_NA_1-1-100/M_ME_NC_1-101-200"
                var parts = deviceID.Split('|');
                if (parts.Length < 7)
                    return null;

                var settings = new DeviceSettings
                {
                    IpAddress = parts[0].Trim(),
                    Port = int.Parse(parts[1]),
                    CommonAddress = int.Parse(parts[2]),
                    OriginatorAddress = int.Parse(parts[3]),
                    CotFieldLength = int.Parse(parts[4]),
                    CommonAddressFieldLength = int.Parse(parts[5]),
                    IoaFieldLength = int.Parse(parts[6]),
                    MaxReadTimes = parts.Length > 7 ? int.Parse(parts[7]) : 1,
                    BlockSettings = parts.Length > 8 ? parts[8] : ""
                };

                // Validate the settings
                if (!IsValidDeviceSettings(settings))
                    return null;

                return settings;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsValidDeviceSettings(DeviceSettings settings)
        {
            if (settings == null)
                return false;

            // Validate IP Address
            if (!IPAddress.TryParse(settings.IpAddress, out _))
                return false;

            // Validate Port range
            if (settings.Port < 1 || settings.Port > 65535)
                return false;

            // Validate Common Address range (IEC 60870-5-104 standard)
            if (settings.CommonAddress < 0 || settings.CommonAddress > 65535)
                return false;

            // Validate Originator Address range
            if (settings.OriginatorAddress < 0 || settings.OriginatorAddress > 255)
                return false;

            // Validate COT Field Length (must be 1 or 2)
            if (settings.CotFieldLength != 1 && settings.CotFieldLength != 2)
                return false;

            // Validate Common Address Field Length (must be 1 or 2)
            if (settings.CommonAddressFieldLength != 1 && settings.CommonAddressFieldLength != 2)
                return false;

            // Validate IOA Field Length (must be 1, 2, or 3)
            if (settings.IoaFieldLength < 1 || settings.IoaFieldLength > 3)
                return false;

            // Validate Max Read Times
            if (settings.MaxReadTimes < 1)
                return false;

            // Validate Block Settings format if provided
            if (!string.IsNullOrEmpty(settings.BlockSettings))
            {
                if (!IsValidBlockSettings(settings.BlockSettings))
                    return false;
            }

            return true;
        }

        private static bool IsValidBlockSettings(string blockSettings)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blockSettings))
                    return true;

                // Format: "TypeId-From-To/TypeId-From-To/..."
                // Example: "M_SP_NA_1-1-100/M_ME_NC_1-101-200"
                var blocks = blockSettings.Split('/');

                foreach (var block in blocks)
                {
                    if (string.IsNullOrWhiteSpace(block))
                        continue;

                    var parts = block.Split('-');
                    if (parts.Length != 3)
                        return false;

                    // Validate TypeId
                    if (!Enum.TryParse<IEC60870.Enum.TypeId>(parts[0], out _))
                        return false;

                    // Validate From and To addresses
                    if (!int.TryParse(parts[1], out int from) ||
                        !int.TryParse(parts[2], out int to))
                        return false;

                    // Validate address range
                    if (from < 1 || to < 1 || from > to || to > 16777215) // 3 bytes max for IOA
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateDeviceID()
        {
            var parts = new[]
            {
                IpAddress,
                Port.ToString(),
                CommonAddress.ToString(),
                OriginatorAddress.ToString(),
                CotFieldLength.ToString(),
                CommonAddressFieldLength.ToString(),
                IoaFieldLength.ToString(),
                MaxReadTimes.ToString(),
                BlockSettings ?? ""
            };

            // Remove empty block settings from the end
            if (string.IsNullOrEmpty(parts[8]))
            {
                return string.Join("|", parts, 0, 8);
            }

            return string.Join("|", parts);
        }

        public override string ToString()
        {
            return $"IEC60870 Device - {IpAddress}:{Port} (CA: {CommonAddress})";
        }

        /// <summary>
        /// Gets detailed description of device configuration
        /// </summary>
        public string GetDetailedDescription()
        {
            return $"IEC60870-5-104 Device Configuration:\n" +
                   $"IP Address: {IpAddress}\n" +
                   $"Port: {Port}\n" +
                   $"Common Address: {CommonAddress}\n" +
                   $"Originator Address: {OriginatorAddress}\n" +
                   $"COT Field Length: {CotFieldLength} byte(s)\n" +
                   $"CA Field Length: {CommonAddressFieldLength} byte(s)\n" +
                   $"IOA Field Length: {IoaFieldLength} byte(s)\n" +
                   $"Max Read Times: {MaxReadTimes}\n" +
                   $"Block Settings: {(string.IsNullOrEmpty(BlockSettings) ? "None" : BlockSettings)}";
        }

        /// <summary>
        /// Validates block settings format and content
        /// </summary>
        /// <param name="blockSettings">Block settings string to validate</param>
        /// <returns>Error message if invalid, null if valid</returns>
        public static string ValidateBlockSettings(string blockSettings)
        {
            if (string.IsNullOrWhiteSpace(blockSettings))
                return null; // Valid (optional)

            try
            {
                var blocks = blockSettings.Split('/');
                foreach (var block in blocks)
                {
                    if (string.IsNullOrWhiteSpace(block))
                        continue;

                    var parts = block.Split('-');
                    if (parts.Length != 3)
                        return $"Invalid block format: '{block}'. Expected format: TypeId-From-To";

                    if (!Enum.TryParse<IEC60870.Enum.TypeId>(parts[0], out _))
                        return $"Invalid TypeId: '{parts[0]}'";

                    if (!int.TryParse(parts[1], out int from) || !int.TryParse(parts[2], out int to))
                        return $"Invalid address range in block: '{block}'";

                    if (from < 1 || to < 1 || from > to)
                        return $"Invalid address range: from={from}, to={to}. From must be <= To and both > 0";

                    if (to > 16777215)
                        return $"Address too large: {to}. Maximum IOA is 16777215 (3 bytes)";
                }

                return null; // Valid
            }
            catch (Exception ex)
            {
                return $"Block settings validation error: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets predefined common block settings for dropdown
        /// </summary>
        public static string[] GetCommonBlockSettings()
        {
            return new string[]
            {
                "", // Empty option
                "M_SP_NA_1-1-100", // Digital inputs
                "M_ME_NC_1-1-50",  // Analog inputs (float)
                "M_SP_NA_1-1-100/M_ME_NC_1-101-200", // Mixed monitoring
                "M_DP_NA_1-1-20/M_ME_NA_1-101-150/C_SC_NA_1-1001-1050", // Complex setup
                "M_SP_NA_1-1-500/M_ME_NC_1-1001-1100/C_SC_NA_1-2001-2100" // Large system
            };
        }

        /// <summary>
        /// Parses block settings and returns structured data
        /// </summary>
        public static List<BlockInfo> ParseBlockSettings(string blockSettings)
        {
            var result = new List<BlockInfo>();

            if (string.IsNullOrWhiteSpace(blockSettings))
                return result;

            try
            {
                var blocks = blockSettings.Split('/');
                foreach (var block in blocks)
                {
                    if (string.IsNullOrWhiteSpace(block))
                        continue;

                    var parts = block.Split('-');
                    if (parts.Length == 3 &&
                        Enum.TryParse<IEC60870.Enum.TypeId>(parts[0], out var typeId) &&
                        int.TryParse(parts[1], out int from) &&
                        int.TryParse(parts[2], out int to))
                    {
                        result.Add(new BlockInfo
                        {
                            TypeId = typeId,
                            FromIOA = from,
                            ToIOA = to,
                            Count = to - from + 1
                        });
                    }
                }
            }
            catch
            {
                // Return empty list if parsing fails
                result.Clear();
            }

            return result;
        }

        // Create default settings for common scenarios
        public static DeviceSettings CreateDefault()
        {
            return new DeviceSettings
            {
                IpAddress = "192.168.1.100",
                Port = 2404,
                CommonAddress = 1,
                OriginatorAddress = 0,
                CotFieldLength = 1,
                CommonAddressFieldLength = 1,
                IoaFieldLength = 2,
                TimeOut = 5000,
                MaxReadTimes = 1,
                BlockSettings = ""
            };
        }

        public DeviceSettings Clone()
        {
            return new DeviceSettings
            {
                IpAddress = this.IpAddress,
                Port = this.Port,
                CommonAddress = this.CommonAddress,
                OriginatorAddress = this.OriginatorAddress,
                CotFieldLength = this.CotFieldLength,
                CommonAddressFieldLength = this.CommonAddressFieldLength,
                IoaFieldLength = this.IoaFieldLength,
                TimeOut = this.TimeOut,
                MaxReadTimes = this.MaxReadTimes,
                BlockSettings = this.BlockSettings
            };
        }
    }

    /// <summary>
    /// Helper class for parsed block information
    /// </summary>
    public class BlockInfo
    {
        public IEC60870.Enum.TypeId TypeId { get; set; }
        public int FromIOA { get; set; }
        public int ToIOA { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return $"{TypeId}: {FromIOA}-{ToIOA} ({Count} points)";
        }
    }
}