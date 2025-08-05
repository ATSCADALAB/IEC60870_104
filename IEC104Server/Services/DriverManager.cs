// File: Services/DriverManager.cs - Cải tiến để đọc Tag từ iDriver1
using ATSCADA;
using System;
using System.Collections.Generic;

namespace IEC60870ServerWinForm.Services
{
    /// <summary>
    /// Cải tiến ATSCADA Driver Manager - đọc tag values từ iDriver1
    /// Hỗ trợ format: Task.Tag hoặc chỉ Tag (sử dụng default task)
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
        /// Khởi tạo driver đơn giản từ iDriver1 trong FormMain
        /// </summary>
        public void Initialize(iDriver driver, string defaultTaskName = "")
        {
            _driver = driver;
            _defaultTaskName = defaultTaskName;

            if (_driver != null)
            {
                _isInitialized = true;
                LogMessage?.Invoke($"Driver initialized successfully with default task: '{_defaultTaskName}'");
            }
            else
            {
                LogMessage?.Invoke("Warning: Driver is null - cần kiểm tra iDriver1 trong FormMain");
            }
        }

        /// <summary>
        /// Đọc giá trị tag theo format: Task.Tag hoặc chỉ Tag
        /// Sử dụng: iDriver1.Task("Task").Tag("Tag").Value
        /// </summary>
        public string GetTagValue(string tagPath)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(tagPath))
            {
                LogMessage?.Invoke($"Cannot read tag: Driver not initialized or empty tag path");
                return null;
            }

            try
            {
                string taskName;
                string tagName;

                // Parse tag path theo format Task.Tag
                if (tagPath.Contains("."))
                {
                    var parts = tagPath.Split('.');
                    if (parts.Length >= 2)
                    {
                        taskName = parts[0];
                        tagName = parts[1];

                        // Nếu có nhiều dấu chấm, gộp phần sau thành tagName
                        if (parts.Length > 2)
                        {
                            tagName = string.Join(".", parts, 1, parts.Length - 1);
                        }
                    }
                    else
                    {
                        taskName = _defaultTaskName;
                        tagName = tagPath;
                    }
                }
                else
                {
                    // Chỉ có tag name, sử dụng default task
                    taskName = _defaultTaskName;
                    tagName = tagPath;
                }

                if (string.IsNullOrEmpty(taskName))
                {
                    LogMessage?.Invoke($"No task name for tag: {tagPath} - Cần set default task hoặc dùng format Task.Tag");
                    return null;
                }

                if (string.IsNullOrEmpty(tagName))
                {
                    LogMessage?.Invoke($"No tag name in path: {tagPath}");
                    return null;
                }

                // Thực hiện đọc tag: iDriver1.Task("Task").Tag("Tag").Value
                var task = _driver.Task(taskName);
                if (task == null)
                {
                    LogMessage?.Invoke($"Task '{taskName}' not found in driver");
                    return null;
                }

                var tag = task.Tag(tagName);
                if (tag == null)
                {
                    LogMessage?.Invoke($"Tag '{tagName}' not found in task '{taskName}'");
                    return null;
                }

                var value = tag.Value;

                // Log successful read (chỉ khi debug)
                // LogMessage?.Invoke($"Successfully read {taskName}.{tagName} = {value}");

                return value?.ToString();
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error reading tag '{tagPath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái tag có Good không
        /// </summary>
        public bool IsTagGood(string tagPath)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(tagPath))
                return false;

            try
            {
                string taskName;
                string tagName;

                // Parse tag path
                if (tagPath.Contains("."))
                {
                    var parts = tagPath.Split('.');
                    if (parts.Length >= 2)
                    {
                        taskName = parts[0];
                        tagName = parts[1];
                        if (parts.Length > 2)
                        {
                            tagName = string.Join(".", parts, 1, parts.Length - 1);
                        }
                    }
                    else
                    {
                        taskName = _defaultTaskName;
                        tagName = tagPath;
                    }
                }
                else
                {
                    taskName = _defaultTaskName;
                    tagName = tagPath;
                }

                if (string.IsNullOrEmpty(taskName) || string.IsNullOrEmpty(tagName))
                    return false;

                var task = _driver.Task(taskName);
                var tag = task?.Tag(tagName);

                if (tag == null) return false;

                // Kiểm tra status - có thể là "Good", "Bad", hoặc các giá trị khác tùy driver
                var status = tag.Status?.ToString() ?? "";
                return status.Equals("Good", StringComparison.OrdinalIgnoreCase) ||
                       status.Equals("OK", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error checking tag status '{tagPath}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test xem tag có tồn tại không
        /// </summary>
        public bool TestTag(string tagPath)
        {
            try
            {
                var value = GetTagValue(tagPath);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của tag (debug)
        /// </summary>
        public string GetTagInfo(string tagPath)
        {
            if (!_isInitialized || _driver == null || string.IsNullOrEmpty(tagPath))
                return "Driver not initialized";

            try
            {
                string taskName;
                string tagName;

                if (tagPath.Contains("."))
                {
                    var parts = tagPath.Split('.');
                    taskName = parts[0];
                    tagName = parts[1];
                }
                else
                {
                    taskName = _defaultTaskName;
                    tagName = tagPath;
                }

                var task = _driver.Task(taskName);
                var tag = task?.Tag(tagName);

                if (tag == null)
                    return $"Tag not found: {taskName}.{tagName}";

                return $"Tag: {taskName}.{tagName}, Value: {tag.Value}, Status: {tag.Status}, Type: {tag.GetType().Name}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Đọc nhiều tag cùng lúc - hiệu quả hơn
        /// </summary>
        public Dictionary<string, string> GetMultipleTagValues(IEnumerable<string> tagPaths)
        {
            var results = new Dictionary<string, string>();

            if (!_isInitialized || _driver == null)
                return results;

            foreach (var tagPath in tagPaths)
            {
                try
                {
                    var value = GetTagValue(tagPath);
                    results[tagPath] = value;
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Error reading tag '{tagPath}': {ex.Message}");
                    results[tagPath] = null;
                }
            }

            return results;
        }

        public void Dispose()
        {
            try
            {
                LogMessage?.Invoke("Disposing DriverManager...");
                _driver = null;
                _isInitialized = false;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error disposing DriverManager: {ex.Message}");
            }
        }
    }
}