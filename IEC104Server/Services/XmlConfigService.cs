using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using IEC104Server.Models;
using IEC60870.Enum;
using IEC60870ServerWinForm.Models;

namespace IEC104Server.Services
{
    /// <summary>
    ///  THÊM MỚI: Service để import/export XML configuration
    /// </summary>
    public class XmlConfigService
    {
        /// <summary>
        ///  Export data points to simple XML format
        /// </summary>
        public void ExportToXml(List<DataPoint> dataPoints, string filePath, string projectName = "IEC104 Project")
        {
            try
            {
                var root = new XElement("IEC104Configuration",
                    new XAttribute("ProjectName", projectName),
                    new XAttribute("CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XAttribute("Version", "1.0"),
                    new XAttribute("TagCount", dataPoints.Count)
                );

                var tagsElement = new XElement("Tags");

                foreach (var dataPoint in dataPoints.OrderBy(dp => dp.IOA))
                {
                    var tagElement = new XElement("Tag",
                        new XAttribute("IOA", dataPoint.IOA),
                        new XAttribute("Name", dataPoint.Name ?? ""),
                        new XAttribute("Type", dataPoint.Type.ToString()),
                        new XAttribute("DataType", dataPoint.DataType.ToString()),
                        new XAttribute("DataTagName", dataPoint.DataTagName ?? ""),
                        new XAttribute("Description", dataPoint.Description ?? ""),
                        new XAttribute("Enabled", true)
                    );

                    // Add optional attributes if available
                    if (!string.IsNullOrEmpty(dataPoint.Value))
                    {
                        tagElement.Add(new XAttribute("LastValue", dataPoint.Value));
                    }

                    if (dataPoint.LastUpdated != DateTime.MinValue)
                    {
                        tagElement.Add(new XAttribute("LastUpdated", dataPoint.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss")));
                    }

                    tagsElement.Add(tagElement);
                }

                root.Add(tagsElement);

                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    root
                );

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to XML: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///  Import data points from simple XML format
        /// </summary>
        public List<DataPoint> ImportFromXml(string filePath)
        {
            try
            {
                var dataPoints = new List<DataPoint>();

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var doc = XDocument.Load(filePath);
                var root = doc.Root;

                if (root?.Name != "IEC104Configuration")
                {
                    throw new Exception("Invalid XML format. Expected root element 'IEC104Configuration'");
                }

                var tagsElement = root.Element("Tags");
                if (tagsElement == null)
                {
                    throw new Exception("No 'Tags' element found in XML");
                }

                foreach (var tagElement in tagsElement.Elements("Tag"))
                {
                    var dataPoint = new DataPoint();

                    // Required attributes
                    dataPoint.IOA = GetIntAttribute(tagElement, "IOA");
                    dataPoint.Name = GetStringAttribute(tagElement, "Name");
                    dataPoint.DataTagName = GetStringAttribute(tagElement, "DataTagName");

                    // Parse Type (TypeId enum)
                    if (Enum.TryParse<TypeId>(GetStringAttribute(tagElement, "Type"), out var typeId))
                    {
                        dataPoint.Type = typeId;
                    }
                    else
                    {
                        dataPoint.Type = TypeId.M_ME_NC_1; // Default to float
                    }

                    // Parse DataType enum
                    if (Enum.TryParse<DataType>(GetStringAttribute(tagElement, "DataType"), out var dataType))
                    {
                        dataPoint.DataType = dataType;
                    }
                    else
                    {
                        dataPoint.DataType = DataType.Float; // Default
                    }

                    // Optional attributes
                    dataPoint.Description = GetStringAttribute(tagElement, "Description");
                    dataPoint.Value = GetStringAttribute(tagElement, "LastValue");

                    // Parse LastUpdated
                    var lastUpdatedStr = GetStringAttribute(tagElement, "LastUpdated");
                    if (DateTime.TryParse(lastUpdatedStr, out var lastUpdated))
                    {
                        dataPoint.LastUpdated = lastUpdated;
                    }

                    // Initialize other properties
                    dataPoint.IsValid = !string.IsNullOrEmpty(dataPoint.Value);

                    dataPoints.Add(dataPoint);
                }

                return dataPoints;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing from XML: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate XML file structure
        /// </summary>
        public bool ValidateXmlFile(string filePath, out string errorMessage)
        {
            errorMessage = "";
            
            try
            {
                if (!File.Exists(filePath))
                {
                    errorMessage = "File does not exist";
                    return false;
                }

                // Try to load and parse XML
                var doc = new XmlDocument();
                doc.Load(filePath);

                // Check root element
                if (doc.DocumentElement?.Name != "IEC104Configuration")
                {
                    errorMessage = "Invalid root element. Expected 'IEC104Configuration'";
                    return false;
                }

                // Check for DataPoints section
                var dataPointsNode = doc.SelectSingleNode("//DataPoints");
                if (dataPointsNode == null)
                {
                    errorMessage = "Missing DataPoints section";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"XML validation error: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Create sample XML file
        /// </summary>
        public void CreateSampleXml(string filePath)
        {
            var sampleDataPoints = new List<DataPoint>
            {
                new DataPoint
                {
                    IOA = 1,
                    Name = "Temperature_01",
                    Type = TypeId.M_ME_NC_1,
                    DataType = DataType.Float,
                    DataTagName = "PLC1.Temperature",
                    Description = "Temperature sensor 1"
                },
                new DataPoint
                {
                    IOA = 2,
                    Name = "Pressure_01",
                    Type = TypeId.M_ME_NB_1,
                    DataType = DataType.Int,
                    DataTagName = "PLC1.Pressure",
                    Description = "Pressure sensor 1"
                },
                new DataPoint
                {
                    IOA = 3,
                    Name = "Pump_Status",
                    Type = TypeId.M_SP_NA_1,
                    DataType = DataType.Bool,
                    DataTagName = "PLC1.PumpStatus",
                    Description = "Pump running status"
                },
                new DataPoint
                {
                    IOA = 4,
                    Name = "Flow_Rate",
                    Type = TypeId.M_ME_NC_1,
                    DataType = DataType.Float,
                    DataTagName = "PLC1.FlowRate",
                    Description = "Water flow rate"
                },
                new DataPoint
                {
                    IOA = 5,
                    Name = "Tank_Level",
                    Type = TypeId.M_ME_NB_1,
                    DataType = DataType.Int,
                    DataTagName = "PLC1.TankLevel",
                    Description = "Water tank level"
                }
            };

            ExportToXml(sampleDataPoints, filePath, "Sample IEC104 Server");
        }

        /// <summary>
        /// Get XML file info
        /// </summary>
        public XmlFileInfo GetXmlFileInfo(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(IEC104Configuration));
                
                using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var config = (IEC104Configuration)serializer.Deserialize(reader);
                    
                    return new XmlFileInfo
                    {
                        ServerName = config.ServerInfo.Name,
                        Version = config.ServerInfo.Version,
                        CreatedDate = config.ServerInfo.CreatedDate,
                        Description = config.ServerInfo.Description,
                        DataPointCount = config.DataPoints.Count,
                        FilePath = filePath,
                        FileSize = new FileInfo(filePath).Length
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading XML file info: {ex.Message}", ex);
            }
        }

        #region Helper Methods

        /// <summary>
        /// Get string attribute value from XML element
        /// </summary>
        private string GetStringAttribute(XElement element, string attributeName)
        {
            return element.Attribute(attributeName)?.Value ?? "";
        }

        /// <summary>
        /// Get integer attribute value from XML element
        /// </summary>
        private int GetIntAttribute(XElement element, string attributeName)
        {
            var value = element.Attribute(attributeName)?.Value;
            return int.TryParse(value, out var result) ? result : 0;
        }

        /// <summary>
        /// Get boolean attribute value from XML element
        /// </summary>
        private bool GetBoolAttribute(XElement element, string attributeName)
        {
            var value = element.Attribute(attributeName)?.Value;
            return bool.TryParse(value, out var result) && result;
        }

        #endregion
    }

    /// <summary>
    /// XML file information
    /// </summary>
    public class XmlFileInfo
    {
        public string ServerName { get; set; }
        public string Version { get; set; }
        public string CreatedDate { get; set; }
        public string Description { get; set; }
        public int DataPointCount { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }

        public string FileSizeFormatted => $"{FileSize / 1024.0:F1} KB";
    }
}
