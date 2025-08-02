// File: Services/ConfigManager.cs
using IEC60870ServerWinForm.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IEC60870ServerWinForm.Services
{
    /// <summary>
    /// Quản lý việc lưu và tải các file cấu hình của ứng dụng.
    /// </summary>
    public class ConfigManager
    {
        private readonly string _serverConfigPath;
        private readonly string _dataPointsPath;

        public ConfigManager(string serverConfigFile = "server_config.json", string dataPointsFile = "datapoints.json")
        {
            // Lưu file cấu hình trong cùng thư mục với file .exe của ứng dụng
            string basePath = Application.StartupPath;
            _serverConfigPath = Path.Combine(basePath, serverConfigFile);
            _dataPointsPath = Path.Combine(basePath, dataPointsFile);
        }

        #region Server Configuration

        /// <summary>
        /// Tải cấu hình server. Nếu file không tồn tại, tạo một file với cấu hình mặc định.
        /// </summary>
        public ServerConfig LoadServerConfig()
        {
            if (!File.Exists(_serverConfigPath))
            {
                var defaultConfig = new ServerConfig();
                SaveServerConfig(defaultConfig);
                return defaultConfig;
            }

            var json = File.ReadAllText(_serverConfigPath);
            return JsonConvert.DeserializeObject<ServerConfig>(json);
        }

        /// <summary>
        /// Lưu cấu hình server vào file JSON.
        /// </summary>
        public void SaveServerConfig(ServerConfig config)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(_serverConfigPath, json);
        }

        #endregion

        #region Data Points Configuration

        /// <summary>
        /// Tải danh sách các điểm dữ liệu. Trả về một danh sách rỗng nếu file không tồn tại.
        /// </summary>
        public List<DataPoint> LoadDataPoints()
        {
            if (!File.Exists(_dataPointsPath))
            {
                return new List<DataPoint>();
            }

            var json = File.ReadAllText(_dataPointsPath);
            return JsonConvert.DeserializeObject<List<DataPoint>>(json) ?? new List<DataPoint>();
        }

        /// <summary>
        /// Lưu danh sách các điểm dữ liệu vào file JSON.
        /// </summary>
        public void SaveDataPoints(List<DataPoint> dataPoints)
        {
            var json = JsonConvert.SerializeObject(dataPoints, Formatting.Indented);
            File.WriteAllText(_dataPointsPath, json);
        }

        #endregion
    }
}