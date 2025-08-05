// File: Models/DataPoint.cs - Simple and Effective
using Newtonsoft.Json;
using System;

namespace IEC60870ServerWinForm.Models
{
    public class DataPoint
    {
        public int IOA { get; set; }
        public string Name { get; set; } = "";
        public DataType Type { get; set; }

        /// <summary>
        /// Tag path từ SmartTagComboBox (VD: "AT-TMSDevice.HighLevel" hoặc "HighLevel")
        /// </summary>
        public string DataTagName { get; set; } = "";

        public string Description { get; set; } = "";

        // Runtime properties - không lưu vào JSON
        [JsonIgnore]
        public string Value { get; set; } = "";

        [JsonIgnore]
        public bool IsValid { get; set; } = true;

        [JsonIgnore]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Convert string value thành object theo DataType
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
                            var boolVal = Value.ToLower();
                            return boolVal == "true" || boolVal == "1" || boolVal == "on";

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
                    return null;
                }
            }
        }

        public override string ToString()
        {
            return $"IOA: {IOA}, Name: {Name}, Tag: {DataTagName}, Value: {Value}";
        }
    }

    public enum DataType
    {
        Bool,
        Int,
        Float,
        Counter,
        Double,
        String
    }
}