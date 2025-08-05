// File: Services/DriverManager.cs
using ATSCADA;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System;
using System.Collections.Generic;

namespace IEC60870ServerWinForm.Services
{
    /// <summary>
    /// Quản lý ATSCADA Driver để đọc/ghi tag values
    /// </summary>
    public class DriverManager : IDisposable
    {
        private iDriver _driver;
        private bool _isInitialized = false;
        private readonly Dictionary<string, ITag> _tagCache = new Dictionary<string, ITag>();

        public event Action DriverReady;
        public event Action<string> LogMessage;

        public bool IsInitialized => _isInitialized;
        public iDriver Driver => _driver;

        public DriverManager()
        {
            // Constructor
        }

        /// <summary>
        /// Khởi tạo driver instance
        /// </summary>
        /// <param name="driver">ATSCADA Driver instance</param>
        public void Initialize(iDriver driver)
        {
            if (_driver != null)
            {
                _driver.ConstructionCompleted -= OnConstructionCompleted;
            }

            _driver = driver;

            if (_driver != null)
            {
                LogMessage?.Invoke("Driver instance set.");

                // Đăng ký event cho construction completed
                _driver.ConstructionCompleted += OnConstructionCompleted;

                // Kiểm tra nếu driver đã sẵn sàng (có thể construction đã complete rồi)
                try
                {
                    // Test basic functionality - nếu không throw exception thì driver đã sẵn sàng
                    CheckDriverReadiness();
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Driver not ready yet: {ex.Message}");
                    // Driver chưa sẵn sàng, chờ ConstructionCompleted event
                }
            }
            else
            {
                LogMessage?.Invoke("Warning: Driver instance is null.");
            }
        }

        private void CheckDriverReadiness()
        {
            if (_driver == null) return;

            try
            {
                // Test driver bằng cách thử call một method cơ bản
                // Nếu driver chưa ready, sẽ throw exception
                var testResult = _driver.ToString(); // Basic test

                // Nếu không có exception, driver có thể đã sẵn sàng
                if (!_isInitialized)
                {
                    LogMessage?.Invoke("Driver appears to be ready. Setting initialized state.");
                    _isInitialized = true;
                    DriverReady?.Invoke();
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Driver readiness test failed: {ex.Message}. Waiting for ConstructionCompleted event.");
            }
        }

        private void OnConstructionCompleted()
        {
            _isInitialized = true;
            LogMessage?.Invoke("Driver construction completed. Ready to read tags.");
            DriverReady?.Invoke();
        }

        /// <summary>
        /// Lấy tag value by name với caching - returns string value
        /// </summary>
        /// <param name="tagName">Tên tag</param>
        /// <returns>String value của tag hoặc null nếu không tìm thấy</returns>
        public string GetTagValue(string tagName)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(tagName))
                return null;

            try
            {
                // Kiểm tra cache trước
                if (!_tagCache.ContainsKey(tagName))
                {
                    ITag tag = _driver.GetTagByName(tagName);
                    if (tag != null)
                    {
                        _tagCache[tagName] = tag;
                    }
                    else
                    {
                        LogMessage?.Invoke($"Tag '{tagName}' not found in driver.");
                        return null;
                    }
                }

                var tagValue = _tagCache[tagName]?.Value;
                return tagValue?.ToString(); // Convert to string
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error getting tag value for '{tagName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy tag instance by name
        /// </summary>
        /// <param name="tagName">Tên tag</param>
        /// <returns>ITag instance hoặc null</returns>
        public ITag GetTag(string tagName)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(tagName))
                return null;

            try
            {
                if (!_tagCache.ContainsKey(tagName))
                {
                    ITag tag = _driver.GetTagByName(tagName);
                    if (tag != null)
                    {
                        _tagCache[tagName] = tag;
                    }
                }

                return _tagCache.ContainsKey(tagName) ? _tagCache[tagName] : null;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error getting tag '{tagName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra tag status
        /// </summary>
        /// <param name="tagName">Tên tag</param>
        /// <returns>Status string hoặc "Unknown"</returns>
        public string GetTagStatus(string tagName)
        {
            ITag tag = GetTag(tagName);
            return tag?.Status ?? "Unknown";
        }

        /// <summary>
        /// Kiểm tra tag có status Good không
        /// </summary>
        /// <param name="tagName">Tên tag</param>
        /// <returns>True nếu status là Good</returns>
        public bool IsTagGood(string tagName)
        {
            return GetTagStatus(tagName) == "Good";
        }

        /// <summary>
        /// Set tag value (nếu tag hỗ trợ write)
        /// </summary>
        /// <param name="tagName">Tên tag</param>
        /// <param name="value">Giá trị mới (string)</param>
        /// <returns>True nếu set thành công</returns>
        public bool SetTagValue(string tagName, string value)
        {
            try
            {
                ITag tag = GetTag(tagName);
                if (tag != null)
                {
                    tag.Value = value;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error setting tag value for '{tagName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clear tag cache
        /// </summary>
        public void ClearCache()
        {
            _tagCache.Clear();
            LogMessage?.Invoke("Tag cache cleared.");
        }

        /// <summary>
        /// Lấy thông tin tất cả tags đã cache
        /// </summary>
        /// <returns>Dictionary chứa tag name và string value</returns>
        public Dictionary<string, string> GetAllCachedTagValues()
        {
            var result = new Dictionary<string, string>();

            foreach (var kvp in _tagCache)
            {
                try
                {
                    result[kvp.Key] = kvp.Value?.Value?.ToString() ?? "";
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Error reading cached tag '{kvp.Key}': {ex.Message}");
                    result[kvp.Key] = "";
                }
            }

            return result;
        }

        /// <summary>
        /// Validate multiple tags exist
        /// </summary>
        /// <param name="tagNames">Danh sách tên tags</param>
        /// <returns>Dictionary với tag name và validation result</returns>
        public Dictionary<string, bool> ValidateTags(IEnumerable<string> tagNames)
        {
            var result = new Dictionary<string, bool>();

            foreach (string tagName in tagNames)
            {
                if (string.IsNullOrEmpty(tagName))
                {
                    result[tagName] = false;
                    continue;
                }

                try
                {
                    ITag tag = GetTag(tagName);
                    result[tagName] = tag != null;
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Error validating tag '{tagName}': {ex.Message}");
                    result[tagName] = false;
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (_driver != null)
            {
                _driver.ConstructionCompleted -= OnConstructionCompleted;
            }

            _tagCache.Clear();
            _isInitialized = false;

            LogMessage?.Invoke("DriverManager disposed.");
        }
    }
}