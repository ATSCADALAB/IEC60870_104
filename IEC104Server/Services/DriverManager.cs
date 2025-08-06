// File: Services/DriverManager.cs - Cải tiến để đọc Tag từ iDriver1
using ATSCADA;
using System;
using System.Collections.Generic;
using System.Linq;

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
            LogMessage?.Invoke($"🔧 DriverManager.Initialize called with driver: {(driver != null ? "NOT NULL" : "NULL")}");

            _driver = driver;
            _defaultTaskName = defaultTaskName;

            if (_driver != null)
            {
                _isInitialized = true;
                LogMessage?.Invoke($" DriverManager initialized successfully with default task: '{_defaultTaskName}'");
                LogMessage?.Invoke($"   _driver: {(_driver != null ? "Available" : "NULL")}");
                LogMessage?.Invoke($"   _isInitialized: {_isInitialized}");
            }
            else
            {
                _isInitialized = false;
                LogMessage?.Invoke("❌ DriverManager initialization failed: Driver is null");
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
        /// <summary>
        ///  OPTIMIZED: Batch read multiple tags với performance cao
        /// </summary>
        public Dictionary<string, string> GetMultipleTagValues(IEnumerable<string> tagPaths)
        {
            var results = new Dictionary<string, string>();

            //  FIX: Check _driver null và log debug info
            if (!_isInitialized)
            {
                LogMessage?.Invoke($"❌ GetMultipleTagValues: Driver not initialized");
                return results;
            }

            if (_driver == null)
            {
                LogMessage?.Invoke($"❌ GetMultipleTagValues: _driver is null");
                return results;
            }

            var tagList = tagPaths.ToList();
            if (tagList.Count == 0) return results;

            var startTime = DateTime.Now;

            //  OPTIMIZATION 1: Group tags by task để tối ưu network calls
            var tagsByTask = new Dictionary<string, List<(string fullPath, string tagName)>>();

            foreach (var tagPath in tagList)
            {
                try
                {
                    var parts = tagPath.Split('.');
                    string taskName, tagName;

                    if (parts.Length >= 2)
                    {
                        taskName = parts[0];
                        tagName = parts.Length > 2 ? string.Join(".", parts, 1, parts.Length - 1) : parts[1];
                    }
                    else
                    {
                        taskName = _defaultTaskName ?? "DefaultTask";
                        tagName = tagPath;
                    }

                    if (!tagsByTask.ContainsKey(taskName))
                        tagsByTask[taskName] = new List<(string, string)>();

                    tagsByTask[taskName].Add((tagPath, tagName));
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Error parsing tag path '{tagPath}': {ex.Message}");
                    results[tagPath] = null;
                }
            }

            //  OPTIMIZATION 2: Batch read per task
            foreach (var taskGroup in tagsByTask)
            {
                var taskName = taskGroup.Key;
                var tags = taskGroup.Value;

                try
                {
                    var taskObj = _driver.Task(taskName);

                    //  OPTIMIZATION 3: Parallel read trong cùng task (nếu driver support)
                    foreach (var (fullPath, tagName) in tags)
                    {
                        try
                        {
                            var value = taskObj.Tag(tagName).Value?.ToString();
                            results[fullPath] = value;
                        }
                        catch (Exception ex)
                        {
                            LogMessage?.Invoke($"Error reading tag '{fullPath}': {ex.Message}");
                            results[fullPath] = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Error accessing task '{taskName}': {ex.Message}");
                    // Mark all tags in this task as failed
                    foreach (var (fullPath, _) in tags)
                    {
                        results[fullPath] = null;
                    }
                }
            }

            var elapsed = DateTime.Now - startTime;

            //  OPTIMIZATION 4: Performance monitoring
            if (elapsed.TotalMilliseconds > 500 || tagList.Count > 100)
            {
                LogMessage?.Invoke($" Batch read: {tagList.Count} tags in {elapsed.TotalMilliseconds:F0}ms " +
                                 $"({tagList.Count / elapsed.TotalSeconds:F0} tags/sec)");
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