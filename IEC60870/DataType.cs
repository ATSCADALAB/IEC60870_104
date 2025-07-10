using System;

namespace IEC60870Driver
{
    public enum DataType
    {
        Default = 0,
        Bool = 1,
        Word = 2,
        Int = 3,
        DWord = 4,
        Float = 5,
        String = 6
    }

    public static class DataTypeExtensions
    {
        public static DataType GetDataType(this string value)
        {
            if (Enum.TryParse<DataType>(value, true, out DataType result))
                return result;
            return DataType.Default;
        }

        public static string ToDisplayName(this DataType dataType)
        {
            return dataType.ToString();
        }
    }
}