// File: Models/DataPoint.cs - Cập nhật với mapping DataType và TypeId
using System;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
using IEC60870.Object;

namespace IEC104Server.Models
{
    /// <summary>
    /// Enum DataType tự định nghĩa - khác với TypeId của IEC60870
    /// </summary>
    public enum DataType
    {
        Bool,      // Kiểu boolean
        Int,       // Kiểu số nguyên  
        Float,     // Kiểu số thực
        Counter,   // Kiểu đếm
        Double,    // Kiểu double precision
        String     // Kiểu chuỗi
    }

    public class DataPoint
    {
        public int IOA { get; set; }                    // Information Object Address
        public string Name { get; set; }                // Tên hiển thị
        public string Description { get; set; }         // Mô tả

        //  QUAN TRỌNG: TypeId là chuẩn IEC60870, DataType là enum tự định nghĩa
        public TypeId Type { get; set; }                // TypeId theo chuẩn IEC60870
        public DataType DataType { get; set; }          // DataType tự định nghĩa

        public string DataTagName { get; set; }         // Tag path (Task.Tag)
        public string Value { get; set; }               // Giá trị hiện tại
        public object ConvertedValue { get; set; }      // Giá trị đã convert
        public bool IsValid { get; set; }               // Trạng thái hợp lệ
        public DateTime LastUpdated { get; set; }       // Thời gian cập nhật cuối

        public DataPoint()
        {
            Name = "";
            Description = "";
            DataTagName = "";
            Value = "";
            IsValid = false;
            LastUpdated = DateTime.Now;

            // Default values
            Type = TypeId.M_SP_NA_1;
            DataType = DataType.Bool;
        }

        /// <summary>
        ///  MAPPING: Convert DataType thành TypeId tương ứng
        /// </summary>
        public static TypeId GetTypeIdFromDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Bool:
                    return TypeId.M_SP_NA_1;        // Single point information

                case DataType.Int:
                    return TypeId.M_ME_NB_1;        // Measured value, scaled value

                case DataType.Float:
                case DataType.Double:
                    return TypeId.M_ME_NC_1;        // Measured value, short floating point

                case DataType.Counter:
                    return TypeId.M_IT_NA_1;        // Integrated totals

                case DataType.String:
                    return TypeId.M_ME_NB_1;        // Fallback to scaled value

                default:
                    return TypeId.M_SP_NA_1;        // Default fallback
            }
        }

        /// <summary>
        ///  MAPPING: Convert TypeId thành DataType tương ứng
        /// </summary>
        public static DataType GetDataTypeFromTypeId(TypeId typeId)
        {
            switch (typeId)
            {
                case TypeId.M_SP_NA_1:              // Single point
                case TypeId.M_SP_TA_1:              // Single point with time
                case TypeId.M_SP_TB_1:              // Single point with CP56Time2a
                    return DataType.Bool;

                case TypeId.M_DP_NA_1:              // Double point
                case TypeId.M_DP_TA_1:              // Double point with time
                case TypeId.M_DP_TB_1:              // Double point with CP56Time2a
                    return DataType.Int;

                case TypeId.M_ME_NB_1:              // Scaled value
                case TypeId.M_ME_TB_1:              // Scaled value with time
                case TypeId.M_ME_TE_1:              // Scaled value with CP56Time2a
                case TypeId.M_ST_NA_1:              // Step position
                case TypeId.M_ST_TA_1:              // Step position with time
                case TypeId.M_ST_TB_1:              // Step position with CP56Time2a
                    return DataType.Int;

                case TypeId.M_ME_NC_1:              // Short floating point
                case TypeId.M_ME_TC_1:              // Short floating point with time
                case TypeId.M_ME_TF_1:              // Short floating point with CP56Time2a
                case TypeId.M_ME_NA_1:              // Normalized value
                case TypeId.M_ME_TA_1:              // Normalized value with time
                case TypeId.M_ME_TD_1:              // Normalized value with CP56Time2a
                    return DataType.Float;

                case TypeId.M_IT_NA_1:              // Integrated totals
                case TypeId.M_IT_TA_1:              // Integrated totals with time
                case TypeId.M_IT_TB_1:              // Integrated totals with CP56Time2a
                    return DataType.Counter;

                case TypeId.M_BO_NA_1:              // Bitstring
                case TypeId.M_BO_TA_1:              // Bitstring with time
                case TypeId.M_BO_TB_1:              // Bitstring with CP56Time2a
                    return DataType.String;

                default:
                    return DataType.Bool;            // Default fallback
            }
        }

        /// <summary>
        ///  HELPER: Tự động set TypeId khi thay đổi DataType
        /// </summary>
        public void SetDataType(DataType dataType)
        {
            DataType = dataType;
            Type = GetTypeIdFromDataType(dataType);
        }

        /// <summary>
        ///  HELPER: Tự động set DataType khi thay đổi TypeId
        /// </summary>
        public void SetTypeId(TypeId typeId)
        {
            Type = typeId;
            DataType = GetDataTypeFromTypeId(typeId);
        }

        /// <summary>
        /// Convert giá trị string thành object theo DataType
        /// </summary>
        public object ConvertValueByDataType(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return null;

            try
            {
                switch (DataType)
                {
                    case DataType.Bool:
                        // Hỗ trợ nhiều format: "1"/"0", "true"/"false", "on"/"off"
                        if (stringValue == "1" || stringValue.ToLower() == "true" || stringValue.ToLower() == "on")
                            return true;
                        if (stringValue == "0" || stringValue.ToLower() == "false" || stringValue.ToLower() == "off")
                            return false;
                        return Convert.ToBoolean(stringValue);

                    case DataType.Int:
                        return Convert.ToInt32(stringValue);

                    case DataType.Float:
                        return Convert.ToSingle(stringValue);

                    case DataType.Double:
                        return Convert.ToDouble(stringValue);

                    case DataType.Counter:
                        return Convert.ToUInt32(stringValue); // Counter thường là unsigned

                    case DataType.String:
                        return stringValue;

                    default:
                        return stringValue;
                }
            }
            catch (Exception)
            {
                return null; // Conversion failed
            }
        }

        /// <summary>
        /// Validate giá trị theo DataType
        /// </summary>
        public bool IsValueValid(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return false;

            try
            {
                ConvertValueByDataType(stringValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy tên hiển thị của DataType
        /// </summary>
        public string GetDataTypeDisplayName()
        {
            switch (DataType)
            {
                case DataType.Bool: return "Boolean";
                case DataType.Int: return "Integer";
                case DataType.Float: return "Float";
                case DataType.Double: return "Double";
                case DataType.Counter: return "Counter";
                case DataType.String: return "String";
                default: return "Unknown";
            }
        }

        /// <summary>
        /// Lấy tên hiển thị của TypeId
        /// </summary>
        public string GetTypeIdDisplayName()
        {
            switch (Type)
            {
                case TypeId.M_SP_NA_1: return "Single Point";
                case TypeId.M_DP_NA_1: return "Double Point";
                case TypeId.M_ME_NC_1: return "Float Value";
                case TypeId.M_ME_NB_1: return "Scaled Value";
                case TypeId.M_ME_NA_1: return "Normalized Value";
                case TypeId.M_IT_NA_1: return "Counter";
                case TypeId.M_BO_NA_1: return "Bitstring";
                default: return Type.ToString();
            }
        }

        /// <summary>
        ///  Convert DataPoint to IEC104 InformationObject for transmission
        /// </summary>
        public InformationObject ToInformationObject()
        {
            try
            {
                if (!IsValid || string.IsNullOrEmpty(Value))
                {
                    return null;
                }

                InformationElement[][] elements = null;

                switch (Type)
                {
                    case TypeId.M_SP_NA_1: // Single point
                        {
                            bool boolValue = false;
                            if (bool.TryParse(Value, out boolValue) ||
                                (int.TryParse(Value, out int intVal) && intVal != 0))
                            {
                                var singlePoint = new IeSinglePointWithQuality(boolValue, false, false, false, false);
                                elements = new InformationElement[][] { new InformationElement[] { singlePoint } };
                            }
                        }
                        break;

                    case TypeId.M_ME_NC_1: // Short float
                        {
                            if (float.TryParse(Value, out float floatValue))
                            {
                                var shortFloat = new IeShortFloat(floatValue);
                                elements = new InformationElement[][] { new InformationElement[] { shortFloat } };
                            }
                        }
                        break;

                    case TypeId.M_ME_NB_1: // Scaled value (int)
                        {
                            if (int.TryParse(Value, out int intValue))
                            {
                                var scaledValue = new IeScaledValue(intValue);
                                elements = new InformationElement[][] { new InformationElement[] { scaledValue } };
                            }
                        }
                        break;

                    default:
                        // For unsupported types, try as float
                        if (float.TryParse(Value, out float defaultFloat))
                        {
                            var shortFloat = new IeShortFloat(defaultFloat);
                            elements = new InformationElement[][] { new InformationElement[] { shortFloat } };
                        }
                        break;
                }

                if (elements != null)
                {
                    return new InformationObject(IOA, elements);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return $"IOA:{IOA} - {Name} ({GetDataTypeDisplayName()}/{GetTypeIdDisplayName()}) = {Value}";
        }
    }
}