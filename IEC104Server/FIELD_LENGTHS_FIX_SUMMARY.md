# IEC104Server Field Lengths Fix Summary

## Vấn đề đã phát hiện và sửa

### 🔍 **Vấn đề chính:**
- `IEC60870ServerService.cs` đang sử dụng `config.CotFieldLength` và `config.CaFieldLength` 
- Nhưng `ServerConfig.cs` model không có các properties này
- `ConfigManager.cs` tạo default config với field lengths nhưng model không support

###  **Các sửa chữa đã thực hiện:**

## 1. **ServerConfig.cs** - Thêm Field Lengths Properties

```csharp
//  THÊM MỚI: Field length properties
public int CotFieldLength { get; set; }    // COT field length (1 or 2 bytes)
public int CaFieldLength { get; set; }     // CA field length (1 or 2 bytes)  
public int IoaFieldLength { get; set; }    // IOA field length (1, 2 or 3 bytes)

//  Default values trong constructor
CotFieldLength = 2;                        // 2 bytes COT field (standard)
CaFieldLength = 2;                         // 2 bytes CA field (standard)
IoaFieldLength = 3;                        // 3 bytes IOA field (standard)
```

**Validation và Clone method cũng được cập nhật**

## 2. **IEC60870ServerService.cs** - Sử dụng Field Lengths đúng cách

```csharp
//  CẢI TIẾN: Cấu hình field lengths và timeouts đúng
_server.SetCotFieldLength((byte)config.CotFieldLength);
_server.SetCommonAddressFieldLength((byte)config.CaFieldLength);

//  SỬA LỖI: Config đã là milliseconds, không cần nhân 1000
_server.SetMaxTimeNoAckReceived(config.TimeoutT1);
_server.SetMaxTimeNoAckSent(config.TimeoutT2);
_server.SetMaxIdleTime(config.TimeoutT3);

//  THÊM MỚI: Cấu hình APDU parameters
if (config.MaxUnconfirmedAPDU > 0)
    _server.SetMaxUnconfirmedIPdusReceived(config.MaxUnconfirmedAPDU);
```

## 3. **ServerConfigForm** - Thêm UI Controls cho Field Lengths

### ServerConfigForm.cs:
```csharp
//  Load field lengths
txtCotFieldLength.Text = ServerConfig.CotFieldLength.ToString();
txtCaFieldLength.Text = ServerConfig.CaFieldLength.ToString();
txtIoaFieldLength.Text = ServerConfig.IoaFieldLength.ToString();

//  Validation
if (!int.TryParse(txtCotFieldLength.Text, out int cotLen) || cotLen < 1 || cotLen > 2)
{
    MessageBox.Show("COT Field Length must be 1 or 2 bytes.");
    return false;
}

//  Save field lengths
ServerConfig.CotFieldLength = int.Parse(txtCotFieldLength.Text);
ServerConfig.CaFieldLength = int.Parse(txtCaFieldLength.Text);
ServerConfig.IoaFieldLength = int.Parse(txtIoaFieldLength.Text);
```

### ServerConfigForm.Designer.cs:
- Thêm 6 controls mới: 3 TextBox + 3 Label
- Tăng kích thước Protocol Parameters group box
- Điều chỉnh vị trí các controls khác
- Thêm tooltips hướng dẫn

## 4. **ConfigManager.cs** - Sửa Default Config

```csharp
private ServerConfig CreateDefaultServerConfig()
{
    return new ServerConfig
    {
        IPAddress = "127.0.0.1",
        Port = 2404,
        CommonAddress = 1,
        OriginatorAddress = 0,
        
        //  SỬA LỖI: Timeout values phải là milliseconds
        TimeoutT0 = 30000,  // Thay vì 30
        TimeoutT1 = 15000,  // Thay vì 15
        TimeoutT2 = 10000,  // Thay vì 10
        TimeoutT3 = 20000,  // Thay vì 20
        
        //  THÊM MỚI: Field lengths và APDU parameters
        CotFieldLength = 2,
        CaFieldLength = 2,
        IoaFieldLength = 3,
        MaxUnconfirmedAPDU = 12,
        MaxUnacknowledgedAPDU = 8
    };
}
```

## 5. **MainForm.cs** - Sử dụng Config Values

```csharp
//  CẢI TIẾN: Sử dụng config values thay vì hardcode
return new ASdu(
    point.Type,
    false,
    CauseOfTransmission.SPONTANEOUS,
    false,
    false,
    _serverConfig.OriginatorAddress,     // Từ config
    _serverConfig.CommonAddress,         // Từ config
    new[] { infoObj }
);
```

## 📋 **Kết quả:**

###  **Đã sửa:**
1. **Field Lengths** - COT, CA, IOA field lengths được support đầy đủ
2. **Timeout Values** - Sử dụng đúng milliseconds thay vì seconds
3. **APDU Parameters** - MaxUnconfirmed và MaxUnacknowledged được config
4. **UI Controls** - Giao diện cấu hình hoàn chỉnh với validation
5. **Config Management** - Load/Save đầy đủ tất cả parameters
6. **Server Integration** - Sử dụng config values thay vì hardcode

### 🎯 **Lợi ích:**
- **Tuân thủ chuẩn IEC 60870-5-104** với field lengths configurable
- **Flexibility** - Có thể điều chỉnh field lengths theo yêu cầu hệ thống
- **Compatibility** - Tương thích với các thiết bị khác nhau
- **Maintainability** - Code rõ ràng, không hardcode values
- **User Experience** - Giao diện cấu hình trực quan với tooltips

### 📝 **Chuẩn IEC 60870-5-104:**
- **COT Field**: 1 hoặc 2 bytes (thường là 2)
- **CA Field**: 1 hoặc 2 bytes (thường là 2)  
- **IOA Field**: 1, 2 hoặc 3 bytes (thường là 3)
- **Timeouts**: T0=30s, T1=15s, T2=10s, T3=20s
- **APDU**: k=12, w=8 (recommended values)

## 🔧 **Cách sử dụng:**

1. **Mở Server Configuration**: Tools → Configure Server
2. **Điều chỉnh Field Lengths** trong Protocol Parameters section
3. **Set Timeouts** phù hợp với hệ thống
4. **Validate** - Form sẽ kiểm tra tính hợp lệ
5. **Apply** - Server sẽ sử dụng config mới khi restart

---

**Lưu ý**: Thay đổi field lengths cần restart server và đảm bảo client cũng sử dụng cùng cấu hình.
