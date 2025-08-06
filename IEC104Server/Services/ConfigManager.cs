// File: Services/ConfigManager.cs - Simple and Effective
using IEC104Server.Models;
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
                    var config = JsonConvert.DeserializeObject<ServerConfig>(json);

                    if (config != null)
                    {
                        //  Validate và fix timeout values
                        bool needsUpdate = ValidateAndFixTimeouts(config);

                        // Auto-save fixed config
                        if (needsUpdate)
                        {
                            SaveServerConfig(config);
                        }

                        return config;
                    }
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

        /// <summary>
        ///  THÊM MỚI: Validate và fix timeout values
        /// </summary>
        private bool ValidateAndFixTimeouts(ServerConfig config)
        {
            bool needsUpdate = false;

            // T1: NoACK received timeout (1000ms - 255000ms)
            if (config.TimeoutT1 < 1000 || config.TimeoutT1 > 255000)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️  Invalid T1 timeout: {config.TimeoutT1}ms. Fixed to 15000ms");
                config.TimeoutT1 = 15000;
                needsUpdate = true;
            }

            // T2: NoACK sent timeout (1000ms - 255000ms)
            if (config.TimeoutT2 < 1000 || config.TimeoutT2 > 255000)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️  Invalid T2 timeout: {config.TimeoutT2}ms. Fixed to 10000ms");
                config.TimeoutT2 = 10000;
                needsUpdate = true;
            }

            // T3: Test frame timeout (1000ms - 172800000ms)
            if (config.TimeoutT3 < 1000 || config.TimeoutT3 > 172800000)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️  Invalid T3 timeout: {config.TimeoutT3}ms. Fixed to 20000ms");
                config.TimeoutT3 = 20000;
                needsUpdate = true;
            }

            // T0: Connection timeout
            if (config.TimeoutT0 < 1000)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️  Invalid T0 timeout: {config.TimeoutT0}ms. Fixed to 30000ms");
                config.TimeoutT0 = 30000;
                needsUpdate = true;
            }

            return needsUpdate;
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

        /// <summary>
        ///  THÊM MỚI: Reset config về default values
        /// </summary>
        public void ResetToDefaultConfig()
        {
            try
            {
                var defaultConfig = CreateDefaultServerConfig();
                SaveServerConfig(defaultConfig);
                System.Diagnostics.Debug.WriteLine(" Server config reset to default values");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting config: {ex.Message}");
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
                //  SỬA LỖI: Timeout values phải là milliseconds
                TimeoutT0 = 30000,  // 30 seconds
                TimeoutT1 = 15000,  // 15 seconds
                TimeoutT2 = 10000,  // 10 seconds
                TimeoutT3 = 20000   // 20 seconds
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