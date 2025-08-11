using System;
using System.Threading;
using IEC60870Driver;

class Program
{
    static void Main(string[] args)
    {
        // 1) Cấu hình kết nối (chỉnh lại theo thiết bị của bạn)
        var settings = new DeviceSettings
        {
            IpAddress = "192.168.1.113",
            Port = 2404,

            CommonAddress = 1,
            OriginatorAddress = 0,

            CotFieldLength = 2,
            CommonAddressFieldLength = 2,
            IoaFieldLength = 3,

            ConnectionTimeout = 10000,
            ReadTimeout = 5000,
            InterrogationTimeout = 8000,
            PingTimeout = 3000,
            MaxRetryCount = 3,
            RetryDelay = 500,

            // Tránh kẹt nếu IOA/tag không có dữ liệu
            SkipMissingTags = true,
            MissingTagValue = "OFFLINE"
        };

        string deviceID = settings.GenerateDeviceID();

        // 2) Tag cần đọc (truyền qua args, mặc định "1")
        //    Có thể dùng "1" (IOA) hoặc "M_SP_NA_1:1" (TypeId:IOA)
        string tag = args != null && args.Length > 0 ? args[0] : "1";

        bool running = true;
        Console.CancelKeyPress += (s, e) => { e.Cancel = true; running = false; };

        // 3) Khởi tạo driver và đọc liên tục bằng Read()
        using (var driver = new ATDriver
        {
            ChannelName = "ConsoleReader",
            ChannelAddress = "60", // Lifetime giây (tùy chọn)
            DeviceName = "MyDevice",
            DeviceID = deviceID
        })
        {
            Console.WriteLine($"Reading tag \"{tag}\" continuously... Press Ctrl+C to stop.\n");

            while (running)
            {
                try
                {
                    driver.TagAddress = tag;           // Cấu hình tag cho lần đọc
                    var result = driver.Read();        // Đọc theo TagAddress

                    // Read() trả default khi lỗi/không kết nối.
                    // Kiểm tra đơn giản dựa trên Value/TagType.
                    var hasData = !(string.IsNullOrEmpty(result.TagType) && string.IsNullOrEmpty(result.Value));

                    if (hasData)
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Tag={result.TagAddress} Type={result.TagType} Value={result.Value}");
                    else
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Tag={tag} read failed/offline");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Error: {ex.Message}");
                }

                Thread.Sleep(200); // Tần suất đọc ~5 lần/giây (tùy chỉnh)
            }
        }
    }
}