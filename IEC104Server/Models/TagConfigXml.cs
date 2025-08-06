using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using IEC60870.Enum;

namespace IEC60870ServerWinForm.Models
{
    /// <summary>
    /// ✅ THÊM MỚI: XML structure cho IEC104 tag configuration
    /// </summary>
    [XmlRoot("IEC104Configuration")]
    public class IEC104Configuration
    {
        [XmlElement("ServerInfo")]
        public ServerInfo ServerInfo { get; set; } = new ServerInfo();

        [XmlArray("DataPoints")]
        [XmlArrayItem("DataPoint")]
        public List<DataPointXml> DataPoints { get; set; } = new List<DataPointXml>();
    }

    /// <summary>
    /// Server information trong XML
    /// </summary>
    public class ServerInfo
    {
        [XmlAttribute("Name")]
        public string Name { get; set; } = "IEC104 Server";

        [XmlAttribute("Version")]
        public string Version { get; set; } = "1.0";

        [XmlAttribute("CreatedDate")]
        public string CreatedDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [XmlAttribute("Description")]
        public string Description { get; set; } = "IEC 60870-5-104 Server Configuration";
    }

    /// <summary>
    /// DataPoint structure cho XML serialization
    /// </summary>
    public class DataPointXml
    {
        [XmlAttribute("IOA")]
        public int IOA { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("DataType")]
        public string DataType { get; set; }

        [XmlAttribute("DataTagName")]
        public string DataTagName { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Convert từ DataPoint sang DataPointXml
        /// </summary>
        public static DataPointXml FromDataPoint(DataPoint dataPoint)
        {
            return new DataPointXml
            {
                IOA = dataPoint.IOA,
                Name = dataPoint.Name ?? "",
                Type = dataPoint.Type.ToString(),
                DataType = dataPoint.DataType.ToString(),
                DataTagName = dataPoint.DataTagName ?? "",
                Description = dataPoint.Description ?? "",
                Enabled = true
            };
        }

        /// <summary>
        /// Convert từ DataPointXml sang DataPoint
        /// </summary>
        public DataPoint ToDataPoint()
        {
            var dataPoint = new DataPoint
            {
                IOA = IOA,
                Name = Name,
                DataTagName = DataTagName,
                Description = Description,
                IsValid = false,
                LastUpdated = DateTime.Now
            };

            // Parse TypeId
            if (Enum.TryParse<TypeId>(Type, out var typeId))
            {
                dataPoint.Type = typeId;
            }
            else
            {
                dataPoint.Type = TypeId.M_SP_NA_1; // Default
            }

            // Parse DataType
            if (Enum.TryParse<DataType>(DataType, out var dataType))
            {
                dataPoint.DataType = dataType;
            }
            else
            {
                dataPoint.DataType = Models.DataType.Bool; // Default
            }

            return dataPoint;
        }
    }
}
