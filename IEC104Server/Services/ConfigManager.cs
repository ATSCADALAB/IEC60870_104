// File: Services/ConfigManager.cs - Simple and Effective
using IEC60870ServerWinForm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IEC60870ServerWinForm.Services
{
    public class ConfigManager
    {
        private readonly string _serverConfigPath;
        private readonly string _dataPointsPath;

        public ConfigManager()
        {
            var basePath = Application.StartupPath;
            _serverConfigPath = Path.Combine(basePath, "server_config.json");
            _dataPointsPath = Path.Combine(basePath, "datapoints.json");
        }

        #region Server Config

        public ServerConfig LoadServerConfig()
        {
            try
            {
                if (File.Exists(_serverConfigPath))
                {
                    var json = File.ReadAllText(_serverConfigPath);
                    return JsonConvert.DeserializeObject<ServerConfig>(json) ?? CreateDefaultServerConfig();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load config error: {ex.Message}");
            }

            // Return and save default
            var defaultConfig = CreateDefaultServerConfig();
            SaveServerConfig(defaultConfig);
            return defaultConfig;
        }

        public void SaveServerConfig(ServerConfig config)
        {
            try
            {
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_serverConfigPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save config error: {ex.Message}");
            }
        }

        private ServerConfig CreateDefaultServerConfig()
        {
            return new ServerConfig
            {
                IPAddress = "127.0.0.1",
                Port = 2404,
                CommonAddress = 1,
                CotFieldLength = 2,
                CaFieldLength = 2,
                IoaFieldLength = 3,
                TimeoutT0 = 30,
                TimeoutT1 = 15,
                TimeoutT2 = 10,
                TimeoutT3 = 20
            };
        }

        #endregion

        #region Data Points

        

        public void SaveDataPoints(List<DataPoint> dataPoints)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dataPoints, Formatting.Indented);
                File.WriteAllText(_dataPointsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save data points error: {ex.Message}");
            }
        }
        #endregion
    }
}