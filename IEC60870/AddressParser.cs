using System;
using IEC60870.Enum;

namespace IEC60870Driver
{
    public static class AddressParser
    {
        public static IOAddress GetIOAddress(this string tagAddress, DataType dataType, out string description)
        {
            description = "";
            try
            {
                // Parse format: "TypeId:IOA" (e.g., "M_SP_NA_1:100", "M_ME_NA_1:200")
                var parts = tagAddress.Split(':');
                if (parts.Length != 2) return null;

                if (!Enum.TryParse<TypeId>(parts[0], out TypeId typeId))
                    return null;

                if (!int.TryParse(parts[1], out int ioa))
                    return null;

                description = $"{typeId} at IOA {ioa}";
                return new IOAddress(ioa, typeId, dataType);
            }
            catch
            {
                return null;
            }
        }
    }
}