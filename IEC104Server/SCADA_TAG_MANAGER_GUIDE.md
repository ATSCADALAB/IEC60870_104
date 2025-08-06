# SCADA Tag Manager - User Guide

## Tổng quan

SCADA Tag Manager là công cụ quản lý và monitor các SCADA tags trong IEC104Server. Nó cung cấp giao diện trực quan để theo dõi, test và debug các kết nối SCADA real-time.

## Cách mở SCADA Tag Manager

1. **Từ Menu**: Tools → SCADA Tag Manager...
2. **Hoặc**: Sử dụng shortcut key (nếu có)

## Giao diện chính

### 🎛️ **Controls Panel**

**Test All Tags**
- Test kết nối đến tất cả SCADA tags
- Hiển thị số lượng tags thành công/lỗi
- Tooltip: "Test connection to all SCADA tags"

**Refresh**
- Refresh dữ liệu tags thủ công
- Cập nhật values và status mới nhất
- Tooltip: "Refresh tag data manually"

**Auto Refresh: OFF/ON**
- Toggle auto refresh mỗi 2 giây
- Button màu xanh khi ON
- Tooltip: "Toggle automatic refresh every 2 seconds"

**Export**
- Export dữ liệu tags ra file CSV
- Bao gồm tất cả thông tin chi tiết
- Tooltip: "Export tag data to CSV file"

**Filter**
- Lọc tags theo IOA, Name, hoặc Tag Path
- Real-time filtering khi gõ
- Tooltip: "Filter tags by IOA, Name, or Tag Path"

**Status Label**
- Hiển thị trạng thái hiện tại
- Màu xanh cho thông tin bình thường
- Timestamp + message format

### 📊 **SCADA Tags Grid**

**Các cột hiển thị:**
- **IOA**: Information Object Address
- **Name**: Tên data point
- **TagPath**: Đường dẫn SCADA tag (Task.Tag)
- **DataType**: Kiểu dữ liệu (Bool, Int, Float, etc.)
- **TypeId**: IEC60870 Type ID
- **Value**: Giá trị raw từ SCADA
- **ConvertedValue**: Giá trị đã convert theo DataType
- **IsValid**: ✅ (Good) hoặc ❌ (Bad)
- **LastUpdated**: Thời gian cập nhật cuối (HH:mm:ss)
- **Status**: Trạng thái kết nối (Good/Bad/Unknown)

## Tính năng nâng cao

### 🖱️ **Context Menu (Right-click)**

**View Details**
- Hiển thị thông tin chi tiết tag
- Bao gồm SCADA connection test
- Driver info và status

**Test Connection**
- Test kết nối đến tag cụ thể
- Hiển thị kết quả trong popup
- Real-time value và status

**Refresh This Tag**
- Refresh chỉ tag được chọn
- Cập nhật value và status
- Nhanh hơn refresh toàn bộ

### 🖱️ **Double-click**
- Double-click vào row để xem chi tiết
- Tương tự "View Details" trong context menu
- Tooltip: "Double-click a row to view detailed tag information"

## Các trạng thái Tag

### ✅ **Good Status**
- Kết nối SCADA thành công
- Value hợp lệ và up-to-date
- IsValid = true

### ❌ **Bad Status**
- Kết nối SCADA thất bại
- Value null hoặc không hợp lệ
- IsValid = false

### ⚠️ **Unknown Status**
- Driver chưa initialize
- Tag chưa được test
- Chưa có dữ liệu

## Workflow sử dụng

### 🔄 **Monitoring thường xuyên**
1. Mở SCADA Tag Manager
2. Bật "Auto Refresh: ON"
3. Theo dõi real-time values
4. Sử dụng Filter để focus vào tags quan tâm

### 🔍 **Troubleshooting**
1. Click "Test All Tags" để kiểm tra tổng thể
2. Right-click → "Test Connection" cho tag cụ thể
3. Double-click để xem chi tiết lỗi
4. Kiểm tra Driver status và Tag path

### 📤 **Export dữ liệu**
1. Click "Export" button
2. Chọn vị trí lưu file CSV
3. File bao gồm timestamp trong tên
4. Mở bằng Excel để phân tích

## Keyboard Shortcuts

- **F5**: Refresh manual
- **Ctrl+E**: Export data
- **Ctrl+F**: Focus vào Filter textbox
- **Escape**: Clear filter
- **Enter**: Test selected tag

## Tips & Tricks

### 🎯 **Performance**
- Sử dụng Filter để giảm số tags hiển thị
- Tắt Auto Refresh khi không cần thiết
- Export data để phân tích offline

### 🔧 **Debugging**
- Kiểm tra Status column để identify issues
- Sử dụng "View Details" để xem driver info
- Test individual tags để isolate problems

### 📊 **Monitoring**
- Theo dõi LastUpdated để detect stale data
- So sánh Value vs ConvertedValue để check conversion
- Sử dụng IsValid để filter good/bad tags

## Error Messages

**"SCADA Driver not initialized"**
- Kiểm tra iDriver1 trong MainForm
- Gọi _driverManager.Initialize()

**"Tag not found"**
- Kiểm tra Tag Path format (Task.Tag)
- Verify tag tồn tại trong SCADA system

**"Export error"**
- Kiểm tra quyền ghi file
- Đảm bảo file không đang mở

## Integration với MainForm

SCADA Tag Manager sử dụng:
- **DriverManager**: Để đọc SCADA data
- **DataPoints List**: Từ MainForm
- **Real-time updates**: Sync với main application

Thay đổi trong Tag Manager sẽ reflect trong MainForm và ngược lại.

## Technical Notes

- **Refresh Interval**: 2 seconds (configurable)
- **Export Format**: CSV with UTF-8 encoding
- **Filter**: Case-insensitive, partial match
- **Grid**: Read-only, full row selection
- **Memory**: Efficient với large tag lists

---

**Lưu ý**: SCADA Tag Manager chỉ hoạt động khi SCADA Driver đã được initialize và có ít nhất một data point với Tag Path.
