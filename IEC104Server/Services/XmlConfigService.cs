using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IEC60870ServerWinForm.Models;

namespace IEC60870ServerWinForm.Services
{
    /// <summary>
    /// ✅ THÊM MỚI: Service để import/export XML configuration
    /// </summary>
    public class XmlConfigService
    {
        /// <summary>
        /// Export data points to XML file
        /// </summary>
        public bool ExportToXml(List<DataPoint> dataPoints, string filePath, string serverName = "IEC104 Server")
        {
            try
            {
                var config = new IEC104Configuration
                {
                    ServerInfo = new ServerInfo
                    {
                        Name = serverName,
                        Version = "1.0",
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Description = "IEC 60870-5-104 Server Configuration"
                    },
                    DataPoints = dataPoints.Select(DataPointXml.FromDataPoint).ToList()
                };

                var serializer = new XmlSerializer(typeof(IEC104Configuration));
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false
                };

                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    serializer.Serialize(writer, config);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to XML: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Import data points from XML file
        /// </summary>
        public List<DataPoint> ImportFromXml(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var serializer = new XmlSerializer(typeof(IEC104Configuration));
                
                using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var config = (IEC104Configuration)serializer.Deserialize(reader);
                    return config.DataPoints.Select(dp => dp.ToDataPoint()).ToList();
                }
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
                    Type = IEC60870.Enum.TypeId.M_ME_NC_1,
                    DataType = DataType.Float,
                    DataTagName = "PLC1.Temperature",
                    Description = "Temperature sensor 1"
                },
                new DataPoint
                {
                    IOA = 2,
                    Name = "Pressure_01",
                    Type = IEC60870.Enum.TypeId.M_ME_NB_1,
                    DataType = DataType.Int,
                    DataTagName = "PLC1.Pressure",
                    Description = "Pressure sensor 1"
                },
                new DataPoint
                {
                    IOA = 3,
                    Name = "Pump_Status",
                    Type = IEC60870.Enum.TypeId.M_SP_NA_1,
                    DataType = DataType.Bool,
                    DataTagName = "PLC1.PumpStatus",
                    Description = "Pump running status"
                },
                new DataPoint
                {
                    IOA = 4,
                    Name = "Flow_Rate",
                    Type = IEC60870.Enum.TypeId.M_ME_NC_1,
                    DataType = DataType.Float,
                    DataTagName = "PLC1.FlowRate",
                    Description = "Water flow rate"
                },
                new DataPoint
                {
                    IOA = 5,
                    Name = "Tank_Level",
                    Type = IEC60870.Enum.TypeId.M_ME_NB_1,
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
