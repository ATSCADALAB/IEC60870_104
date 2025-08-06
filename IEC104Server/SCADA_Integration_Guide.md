# SCADA Integration Guide - IEC 60870 Server

## Tổng quan

IEC 60870 Server đã được tích hợp với hệ thống SCADA thông qua `iDriver1` để đọc dữ liệu real-time từ các tags và chuyển đổi thành các data points IEC 60870-5-104.

## Kiến trúc tích hợp

```
SCADA System (iDriver1) → DriverManager → DataPoint → IEC60870 Server → IEC Clients
```

### Các thành phần chính:

1. **DriverManager**: Quản lý kết nối với SCADA system
2. **DataPoint**: Model chứa mapping giữa SCADA tags và IEC data points  
3. **SCADATagManagerForm**: Giao diện quản lý và monitor SCADA tags
4. **MainForm**: Giao diện chính với tích hợp SCADA

## Cách sử dụng

### 1. Khởi tạo SCADA Driver

```csharp
// Trong MainForm constructor hoặc Load event
_driverManager.Initialize(iDriver1, "DefaultTaskName");
```

### 2. Thêm Data Point với SCADA Tag

#### Cách 1: Sử dụng giao diện
- Click "Add Point" button
- Nhập thông tin IOA, Name, Description
- Chọn Data Type (Bool, Int, Float, Counter, Double, String)
- Nhập Tag Path theo format: `Task.Tag` hoặc chỉ `Tag`
- Click "Test Tag" để kiểm tra kết nối
- Click OK để lưu

#### Cách 2: Sử dụng code
```csharp
// Thêm với DataType (tự động mapping TypeId)
AddDataPointByDataType(16385, "Temperature", DataType.Float, "PLC1.Temperature");

// Thêm với TypeId cụ thể
AddDataPoint(16386, "Pump Status", TypeId.M_SP_NA_1, "PLC1.Pump01.Status");
```

### 3. Format Tag Path

#### Format chuẩn: `Task.Tag`
- `PLC1.Temperature` - Task: PLC1, Tag: Temperature
- `Zone1.Pressure.Value` - Task: Zone1, Tag: Pressure.Value

#### Format đơn giản: `Tag`
- `Temperature` - Sử dụng default task đã set trong DriverManager
- Cần set default task: `_driverManager.DefaultTaskName = "PLC1"`

### 4. Data Type Mapping

| DataType | TypeId IEC60870 | Mô tả |
|----------|-----------------|-------|
| Bool | M_SP_NA_1 | Single point information |
| Int | M_ME_NB_1 | Measured value, scaled |
| Float | M_ME_NC_1 | Measured value, short floating point |
| Counter | M_IT_NA_1 | Integrated totals |
| Double | M_ME_NC_1 | Measured value, short floating point |
| String | M_ME_NB_1 | Fallback to scaled value |

### 5. Monitoring và Debug

#### SCADA Tag Manager
- Menu: Tools → SCADA Tag Manager
- Xem tất cả tags real-time
- Test kết nối từng tag
- Export dữ liệu ra CSV
- Auto refresh mode

#### Log Messages
```
 SCADA Test OK: PLC1.Temperature = 25.5
⚠️  SCADA Test Failed: PLC1.Pressure - Value: null, Good: False
🔄 Updated Temperature (IOA:16385): PLC1.Temperature = 25.5 -> 25.5 (Float)
 SCADA Scan Summary: 15 Good, 2 Error, 17 Total Tags
```

## API Reference

### DriverManager Methods

```csharp
// Khởi tạo driver
void Initialize(iDriver driver, string defaultTaskName = "")

// Đọc single tag
string GetTagValue(string tagPath)

// Đọc multiple tags (hiệu quả hơn)
Dictionary<string, string> GetMultipleTagValues(IEnumerable<string> tagPaths)

// Kiểm tra tag status
bool IsTagGood(string tagPath)

// Test tag tồn tại
bool TestTag(string tagPath)

// Lấy thông tin chi tiết tag
string GetTagInfo(string tagPath)
```

### DataPoint Methods

```csharp
// Set DataType và auto mapping TypeId
void SetDataType(DataType dataType)

// Set TypeId và auto mapping DataType  
void SetTypeId(TypeId typeId)

// Convert value theo DataType
object ConvertValueByDataType(string value)

// Validation
bool IsValidTagPath()
string GetTaskName()
string GetTagName()
```

### MainForm Methods

```csharp
// Thêm data point
void AddDataPointByDataType(int ioa, string name, DataType dataType, string tagPath)
void AddDataPoint(int ioa, string name, TypeId type, string tagPath)

// SCADA operations
void TestAllSCADATags()
void ShowSCADAStatistics()
void ForceScanTags()
```

## Troubleshooting

### Lỗi thường gặp

1. **"Driver not initialized"**
   - Kiểm tra `iDriver1` có null không
   - Gọi `_driverManager.Initialize(iDriver1)`

2. **"Task not found"**
   - Kiểm tra task name có đúng không
   - Set default task nếu dùng format Tag only

3. **"Tag not found"**
   - Kiểm tra tag name có đúng không
   - Dùng SCADA Tag Manager để test

4. **"Value null hoặc Bad status"**
   - Kiểm tra SCADA system có running không
   - Kiểm tra tag có data không
   - Kiểm tra quyền truy cập

### Performance Tips

1. **Sử dụng GetMultipleTagValues** thay vì GetTagValue nhiều lần
2. **Set scan interval hợp lý** (default 1 giây)
3. **Chỉ add tags thực sự cần thiết**
4. **Sử dụng filter trong SCADA Tag Manager** khi có nhiều tags

## Ví dụ hoàn chỉnh

```csharp
// 1. Initialize SCADA driver
_driverManager.Initialize(iDriver1, "PLC1");

// 2. Add data points
AddDataPointByDataType(16385, "Temperature", DataType.Float, "PLC1.Temperature");
AddDataPointByDataType(16386, "Pressure", DataType.Float, "PLC1.Pressure");
AddDataPointByDataType(16387, "Pump Status", DataType.Bool, "PLC1.Pump01.Status");
AddDataPointByDataType(16388, "Flow Rate", DataType.Int, "PLC1.FlowMeter.Rate");

// 3. Start server
StartServer();

// 4. Monitor tags
TestAllSCADATags();
ShowSCADAStatistics();
```

## Cấu hình nâng cao

### Custom scan interval
```csharp
// Trong MainForm constructor
_scanTimer.Interval = 2000; // 2 seconds
```

### Custom data conversion
```csharp
// Override ConvertValueByDataType trong DataPoint
public override object ConvertValueByDataType(string value)
{
    // Custom conversion logic
    return base.ConvertValueByDataType(value);
}
```

---

**Lưu ý**: Đảm bảo SCADA system đang chạy và `iDriver1` đã được khởi tạo trước khi sử dụng các tính năng SCADA integration.
