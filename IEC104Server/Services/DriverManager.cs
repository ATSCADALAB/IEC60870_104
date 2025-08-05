// File: Services/DriverManager.cs - Simple and Effective
using ATSCADA;
using System;
using System.Collections.Generic;

namespace IEC60870ServerWinForm.Services
{
    /// <summary>
    /// Simple ATSCADA Driver Manager - chỉ focus vào đọc tag values
    /// </summary>
    public class DriverManager : IDisposable
    {
        private iDriver _driver;
        private string _defaultTaskName = "";
        private bool _isInitialized = false;

        public event Action<string> LogMessage;

        public bool IsInitialized => _isInitialized;
        public iDriver Driver => _driver;
        public string DefaultTaskName
        {
            get => _defaultTaskName;
            set => _defaultTaskName = value;
        }

        /// <summary>
        /// Khởi tạo driver đơn giản
        /// </summary>
        public void Initialize(iDriver driver, string defaultTaskName = "")
        {
            _driver = driver;
            _defaultTaskName = defaultTaskName;

            if (_driver != null)
            {
                _isInitialized = true;
                LogMessage?.Invoke($"Driver initialized with default task: '{_defaultTaskName}'");
            }
            else
            {
                LogMessage?.Invoke("Warning: Driver is null");
            }
        }

        /// <summary>
        /// Đọc giá trị tag - đơn giản và hiệu quả
        /// </summary>
        public string GetTagValue(string fullTagPath)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(fullTagPath))
                return null;

            try
            {
                string taskName;
                string tagName;

                // Parse tag path
                if (fullTagPath.Contains("."))
                {
                    var lastDotIndex = fullTagPath.LastIndexOf('.');
                    taskName = fullTagPath.Substring(0, lastDotIndex);
                    tagName = fullTagPath.Substring(lastDotIndex + 1);
                }
                else
                {
                    taskName = _defaultTaskName;
                    tagName = fullTagPath;
                }

                if (string.IsNullOrEmpty(taskName))
                {
                    LogMessage?.Invoke($"No task name for tag: {fullTagPath}");
                    return null;
                }

                // Đọc tag value trực tiếp
                var tag = _driver.Task(taskName).Tag(tagName);
                return tag?.Value?.ToString();
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error reading {fullTagPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra tag status - đơn giản
        /// </summary>
        public bool IsTagGood(string fullTagPath)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(fullTagPath))
                return false;

            try
            {
                string taskName;
                string tagName;

                if (fullTagPath.Contains("."))
                {
                    var lastDotIndex = fullTagPath.LastIndexOf('.');
                    taskName = fullTagPath.Substring(0, lastDotIndex);
                    tagName = fullTagPath.Substring(lastDotIndex + 1);
                }
                else
                {
                    taskName = _defaultTaskName;
                    tagName = fullTagPath;
                }

                if (string.IsNullOrEmpty(taskName))
                    return false;

                var tag = _driver.Task(taskName).Tag(tagName);
                return tag != null && tag.Status == "Good";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Test tag existence - đơn giản
        /// </summary>
        public bool TestTag(string fullTagPath)
        {
            try
            {
                var value = GetTagValue(fullTagPath);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _driver = null;
            _isInitialized = false;
        }
    }
}