# IEC104 Driver - Timeout Configuration & Missing Tag Handling

## Tính năng mới đã thêm

### 1. **Configurable Timeouts**
Tất cả timeout values giờ đây có thể cấu hình thông qua DeviceSettings:

```csharp
var deviceSettings = new DeviceSettings
{
    IpAddress = "192.168.1.100",
    Port = 2404,
    CommonAddress = 1,
    OriginatorAddress = 0,
    
    // TIMEOUT CONFIGURATIONS
    ConnectionTimeout = 15000,      // Timeout kết nối (ms) - default: 10000
    ReadTimeout = 8000,             // Timeout đọc dữ liệu (ms) - default: 5000  
    WriteTimeout = 5000,            // Timeout ghi dữ liệu (ms) - default: 3000
    InterrogationTimeout = 12000,   // Timeout cho General Interrogation (ms) - default: 8000
    PingTimeout = 5000,             // Timeout cho ping test (ms) - default: 3000
    MaxRetryCount = 5,              // Số lần retry tối đa - default: 3
    RetryDelay = 1000,              // Delay giữa các lần retry (ms) - default: 500
    
    // MISSING TAG HANDLING
    SkipMissingTags = true,         // Bỏ qua tag không tồn tại - default: true
    MissingTagValue = "NULL"        // Giá trị trả về cho tag không tồn tại - default: "BAD"
};
```

### 2. **Missing Tag Handling**
Cơ chế xử lý tag không tồn tại hoặc lỗi đọc:

#### **Khi SkipMissingTags = true:**
- Tag không tồn tại → Trả về `MissingTagValue` (default: "BAD")
- Kết nối fail → Trả về `MissingTagValue` 
- Read timeout → Trả về `MissingTagValue`
- **Hệ thống không bị block**, tiếp tục đọc các tag khác

#### **Khi SkipMissingTags = false:**
- Tag không tồn tại → Trả về `null/default`
- Có thể gây block hệ thống nếu nhiều tag fail

## Cách sử dụng

### 1. **Cấu hình qua DeviceID string:**

```csharp
// Format mở rộng:
// "IP|Port|CA|OA|CotLen|CALen|IOALen|MaxRead|BlockSettings|ConnTimeout|ReadTimeout|WriteTimeout|InterrogationTimeout|PingTimeout|MaxRetry|RetryDelay|SkipMissing|MissingValue"

string deviceID = "192.168.1.100|2404|1|0|1|1|2|1||15000|8000|5000|12000|5000|5|1000|true|NULL";

var driver = new ATDriver();
driver.DeviceID = deviceID;
```

### 2. **Cấu hình qua DeviceSettings object:**

```csharp
var settings = DeviceSettings.CreateDefault();
settings.ConnectionTimeout = 15000;
settings.ReadTimeout = 8000;
settings.SkipMissingTags = true;
settings.MissingTagValue = "OFFLINE";

string deviceID = settings.GenerateDeviceID();
```

### 3. **Đọc tag với error handling:**

```csharp
var driver = new ATDriver();
driver.DeviceID = deviceID;
driver.TagAddress = "400001"; // IOA number

var result = driver.Read();
if (result != null)
{
    Console.WriteLine($"Value: {result.Value}");
    
    // Kiểm tra nếu là missing tag
    if (result.Value == "NULL") // hoặc giá trị MissingTagValue đã cấu hình
    {
        Console.WriteLine("Tag không tồn tại hoặc lỗi đọc");
    }
}
```

## Logging và Monitoring

Driver giờ đây có logging chi tiết:

```
[SUCCESS] ClientAdapter connected on attempt 1
[SUCCESS] Read IOA 400001 after 200ms
[TIMEOUT] Read IOA 400002 - No data after 8000ms
[SKIP] Tag IOA 400002 - All retries failed, returning NULL
[RETRY] Write IOA 500001 - Attempt 2
[SUCCESS] Write IOA 500001 = 123 on attempt 2
```

## Performance Tuning

### **Môi trường Network tốt:**
```csharp
ConnectionTimeout = 5000,
ReadTimeout = 3000,
MaxRetryCount = 2,
RetryDelay = 200
```

### **Môi trường Network chậm:**
```csharp
ConnectionTimeout = 20000,
ReadTimeout = 10000,
MaxRetryCount = 5,
RetryDelay = 2000
```

### **Production với nhiều tags:**
```csharp
SkipMissingTags = true,
MissingTagValue = "OFFLINE",
MaxRetryCount = 3,
RetryDelay = 500
```

## Backward Compatibility

- Tất cả timeout parameters đều có default values
- DeviceID format cũ vẫn hoạt động bình thường
- Không breaking changes với code hiện tại
