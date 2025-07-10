using IEC60870.Enum;

namespace IEC60870Driver
{
    public class IOAddress
    {
        public int InformationObjectAddress { get; set; }
        public TypeId TypeId { get; set; }
        public DataType DataType { get; set; }
        public string Description { get; set; }

        public IOAddress(int ioa, TypeId typeId, DataType dataType)
        {
            InformationObjectAddress = ioa;
            TypeId = typeId;
            DataType = dataType;
        }
    }
}