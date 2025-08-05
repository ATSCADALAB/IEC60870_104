// File: Models/DataPoint.cs
using Newtonsoft.Json;
using System;

namespace IEC60870ServerWinForm.Models
{
    /// <summary>
    /// Đại diện cho một điểm dữ liệu trong hệ thống IEC 60870-5-104
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Information Object Address - Địa chỉ đối tượng thông tin
        /// </summary>
        public int IOA { get; set; }

        /// <summary>
        /// Tên mô tả của điểm dữ liệu
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Loại dữ liệu (Bool, Int, Float, Counter, etc.)
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// Tên Tag để lấy giá trị từ driver
        /// </summary>
        public string DataTagName { get; set; } = "";

        /// <summary>
        /// Giá trị thực tế được đọc từ Tag (string value từ ATSCADA)
        /// </summary>
        [JsonIgnore]
        public string Value { get; set; } = "";

        /// <summary>
        /// Giá trị đã được convert theo DataType để gửi qua IEC protocol
        /// </summary>
        [JsonIgnore]
        public object ConvertedValue
        {
            get
            {
                if (string.IsNullOrEmpty(Value)) return null;

                try
                {
                    switch (Type)
                    {
                        case DataType.Bool:
                            return bool.Parse(Value);
                        case DataType.Int:
                            return int.Parse(Value);
                        case DataType.Float:
                            return float.Parse(Value);
                        case DataType.Counter:
                            return int.Parse(Value);
                        case DataType.Double:
                            return double.Parse(Value);
                        case DataType.String:
                            return Value;
                        default:
                            return Value;
                    }
                }
                catch
                {
                    return null; // Invalid conversion
                }
            }
        }

        /// <summary>
        /// Mô tả chi tiết (tùy chọn)
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Thời gian cập nhật cuối cùng (không lưu vào JSON)
        /// </summary>
        [JsonIgnore]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Trạng thái chất lượng dữ liệu (không lưu vào JSON)
        /// </summary>
        [JsonIgnore]
        public bool IsValid { get; set; } = true;

        public override string ToString()
        {
            return $"IOA: {IOA}, Name: {Name}, Type: {Type}, Tag: {DataTagName}, Value: {Value}";
        }
    }

    /// <summary>
    /// Enum định nghĩa các loại dữ liệu hỗ trợ
    /// </summary>
    public enum DataType
    {
        Bool,       // Single Point - M_SP_NA_1
        Int,        // Scaled Value - M_ME_NB_1  
        Float,      // Short Float - M_ME_NC_1
        Counter,    // Binary Counter - M_IT_NA_1
        Double,     // Normalized Value - M_ME_NA_1
        String      // Text - (custom implementation)
    }
}