using System;
using System.Threading;
using ATDriver_Server;

namespace IEC60870Driver
{
    /// <summary>
    /// Example sử dụng timeout configuration và missing tag handling
    /// </summary>
    public class TimeoutConfigExample
    {
        public static void RunExample()
        {
            Console.WriteLine("=== IEC104 Driver - Timeout & Missing Tag Example ===\n");

            // 1. TẠO DEVICE SETTINGS VỚI TIMEOUT TÙY CHỈNH
            var deviceSettings = CreateCustomDeviceSettings();
            Console.WriteLine("Device Settings:");
            Console.WriteLine(deviceSettings.GetDetailedDescription());
            Console.WriteLine();

            // 2. TẠO DRIVER VỚI SETTINGS
            var driver = new ATDriver();
            string deviceID = deviceSettings.GenerateDeviceID();
            
            Console.WriteLine($"Generated DeviceID: {deviceID}");
            Console.WriteLine();

            // 3. CẤU HÌNH DRIVER
            driver.ChannelName = "IEC104_Channel";
            driver.ChannelAddress = "3600"; // Lifetime 1 hour
            driver.DeviceName = "TestDevice";
            driver.DeviceID = deviceID;

            // 4. TEST ĐỌC CÁC TAG KHÁC NHAU
            TestReadOperations(driver);

            // 5. TEST GHI DỮ LIỆU
            TestWriteOperations(driver);

            // 6. CLEANUP
            driver.Disconnect();
            driver.Dispose();
            
            Console.WriteLine("\n=== Example completed ===");
        }

        private static DeviceSettings CreateCustomDeviceSettings()
        {
            return new DeviceSettings
            {
                IpAddress = "192.168.1.100",
                Port = 2404,
                CommonAddress = 1,
                OriginatorAddress = 0,
                CotFieldLength = 1,
                CommonAddressFieldLength = 1,
                IoaFieldLength = 2,
                MaxReadTimes = 1,
                BlockSettings = "M_SP_NA_1-1-100/M_ME_NC_1-101-200",

                // TIMEOUT CONFIGURATIONS - Tùy chỉnh cho môi trường network chậm
                ConnectionTimeout = 15000,      // 15 giây
                ReadTimeout = 8000,             // 8 giây
                WriteTimeout = 5000,            // 5 giây
                InterrogationTimeout = 12000,   // 12 giây
                PingTimeout = 5000,             // 5 giây
                MaxRetryCount = 5,              // 5 lần retry
                RetryDelay = 1000,              // 1 giây delay

                // MISSING TAG HANDLING
                SkipMissingTags = true,         // Bỏ qua tag lỗi
                MissingTagValue = "OFFLINE"     // Giá trị cho tag lỗi
            };
        }

        private static void TestReadOperations(ATDriver driver)
        {
            Console.WriteLine("=== Testing Read Operations ===");

            // Test case 1: Đọc tag tồn tại (giả sử)
            TestReadTag(driver, "1", "Bool", "Digital Input 1");

            // Test case 2: Đọc tag có thể không tồn tại
            TestReadTag(driver, "999", "Bool", "Non-existent Digital Input");

            // Test case 3: Đọc analog tag
            TestReadTag(driver, "101", "Float", "Analog Input 1");

            // Test case 4: Đọc tag với IOA lớn
            TestReadTag(driver, "16385", "Int", "Large IOA Tag");

            Console.WriteLine();
        }

        private static void TestReadTag(ATDriver driver, string tagAddress, string tagType, string description)
        {
            Console.WriteLine($"Reading {description} (IOA: {tagAddress}, Type: {tagType})");
            
            driver.TagAddress = tagAddress;
            driver.TagType = tagType;
            driver.TagName = description;

            try
            {
                var result = driver.Read();
                if (result != null)
                {
                    Console.WriteLine($"  ✓ Success: {result.Value}");
                    
                    // Kiểm tra nếu là missing tag value
                    if (result.Value == "OFFLINE")
                    {
                        Console.WriteLine($"  ⚠ Warning: Tag returned missing value");
                    }
                }
                else
                {
                    Console.WriteLine($"  ✗ Failed: No data returned");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Exception: {ex.Message}");
            }

            Thread.Sleep(100); // Tránh spam quá nhanh
        }

        private static void TestWriteOperations(ATDriver driver)
        {
            Console.WriteLine("=== Testing Write Operations ===");

            // Test case 1: Ghi digital output
            TestWriteTag(driver, "1001", "Bool", "1", "Digital Output 1");

            // Test case 2: Ghi analog output
            TestWriteTag(driver, "2001", "Float", "123.45", "Analog Output 1");

            // Test case 3: Ghi tag có thể không tồn tại
            TestWriteTag(driver, "9999", "Bool", "0", "Non-existent Output");

            Console.WriteLine();
        }

        private static void TestWriteTag(ATDriver driver, string tagAddress, string tagType, string value, string description)
        {
            Console.WriteLine($"Writing {description} (IOA: {tagAddress}, Type: {tagType}, Value: {value})");

            var sendPack = new SendPack
            {
                ChannelAddress = driver.ChannelAddress,
                DeviceID = driver.DeviceID,
                TagAddress = tagAddress,
                TagType = tagType,
                Value = value
            };

            try
            {
                string result = driver.Write(sendPack);
                if (result != "Bad")
                {
                    Console.WriteLine($"  ✓ Success: {result}");
                }
                else
                {
                    Console.WriteLine($"  ✗ Failed: Write operation failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Exception: {ex.Message}");
            }

            Thread.Sleep(100); // Tránh spam quá nhanh
        }

        /// <summary>
        /// Demo các scenario timeout khác nhau
        /// </summary>
        public static void DemoTimeoutScenarios()
        {
            Console.WriteLine("=== Timeout Scenarios Demo ===\n");

            // Scenario 1: Network nhanh
            Console.WriteLine("1. Fast Network Configuration:");
            var fastSettings = DeviceSettings.CreateDefault();
            fastSettings.ConnectionTimeout = 5000;
            fastSettings.ReadTimeout = 3000;
            fastSettings.MaxRetryCount = 2;
            fastSettings.RetryDelay = 200;
            Console.WriteLine(fastSettings.GetDetailedDescription());
            Console.WriteLine();

            // Scenario 2: Network chậm
            Console.WriteLine("2. Slow Network Configuration:");
            var slowSettings = DeviceSettings.CreateDefault();
            slowSettings.ConnectionTimeout = 20000;
            slowSettings.ReadTimeout = 10000;
            slowSettings.MaxRetryCount = 5;
            slowSettings.RetryDelay = 2000;
            Console.WriteLine(slowSettings.GetDetailedDescription());
            Console.WriteLine();

            // Scenario 3: Production với skip missing tags
            Console.WriteLine("3. Production Configuration (Skip Missing Tags):");
            var prodSettings = DeviceSettings.CreateDefault();
            prodSettings.SkipMissingTags = true;
            prodSettings.MissingTagValue = "OFFLINE";
            prodSettings.MaxRetryCount = 3;
            prodSettings.RetryDelay = 500;
            Console.WriteLine(prodSettings.GetDetailedDescription());
            Console.WriteLine();
        }
    }
}
