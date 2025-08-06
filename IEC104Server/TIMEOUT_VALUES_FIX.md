# Timeout Values Fix Summary

## Vấn đề phát hiện

**Lỗi:** `FATAL ERROR starting server: Invalid NoACK received timeout: 15, time must be between 1000ms and 255000ms`

**Nguyên nhân:** Config file có timeout values sai (giây thay vì milliseconds)

##  **Root Cause Analysis:**

### **IEC60870 Library Requirements:**
- **T1 (NoACK received)**: 1000ms - 255000ms
- **T2 (NoACK sent)**: 1000ms - 255000ms  
- **T3 (Test frame)**: 1000ms - 172800000ms

### **Config file có thể chứa:**
```json
{
  "TimeoutT1": 15,    // ❌ SAI: 15 giây = 15ms (quá nhỏ)
  "TimeoutT2": 10,    // ❌ SAI: 10 giây = 10ms (quá nhỏ)
  "TimeoutT3": 20     // ❌ SAI: 20 giây = 20ms (quá nhỏ)
}
```

### **Cần phải là:**
```json
{
  "TimeoutT1": 15000,  //  ĐÚNG: 15000ms = 15 giây
  "TimeoutT2": 10000,  //  ĐÚNG: 10000ms = 10 giây
  "TimeoutT3": 20000   //  ĐÚNG: 20000ms = 20 giây
}
```

##  **Giải pháp đã triển khai:**

### 1. **Auto-Validation trong ConfigManager**

```csharp
public ServerConfig LoadServerConfig()
{
    // Load config từ file
    var config = JsonConvert.DeserializeObject<ServerConfig>(json);
    
    if (config != null)
    {
        //  Validate và fix timeout values
        bool needsUpdate = false;
        
        if (config.TimeoutT1 < 1000 || config.TimeoutT1 > 255000)
        {
            config.TimeoutT1 = 15000; // Fix to 15 seconds
            needsUpdate = true;
        }
        
        if (config.TimeoutT2 < 1000 || config.TimeoutT2 > 255000)
        {
            config.TimeoutT2 = 10000; // Fix to 10 seconds
            needsUpdate = true;
        }
        
        if (config.TimeoutT3 < 1000 || config.TimeoutT3 > 172800000)
        {
            config.TimeoutT3 = 20000; // Fix to 20 seconds
            needsUpdate = true;
        }
        
        // Auto-save fixed config
        if (needsUpdate)
        {
            SaveServerConfig(config);
        }
    }
}
```

### 2. **Runtime Validation trong IEC60870ServerService**

```csharp
//  Validate và fix timeout values trước khi set
int t1 = ValidateTimeout(config.TimeoutT1, 15000, "T1");
int t2 = ValidateTimeout(config.TimeoutT2, 10000, "T2");
int t3 = ValidateTimeout(config.TimeoutT3, 20000, "T3");

_server.SetMaxTimeNoAckReceived(t1);
_server.SetMaxTimeNoAckSent(t2);
_server.SetMaxIdleTime(t3);

private int ValidateTimeout(int value, int defaultValue, string timeoutName)
{
    if (value < 1000 || value > 255000)
    {
        Log($"⚠️  Invalid {timeoutName} timeout: {value}ms. Using default: {defaultValue}ms");
        return defaultValue;
    }
    return value;
}
```

### 3. **Manual Reset Method**

```csharp
/// <summary>
/// Reset config với giá trị mặc định đúng
/// </summary>
public void ResetServerConfig()
{
    _configManager.ResetToDefaultConfig();
    _serverConfig = _configManager.LoadServerConfig();
    LogMessage(" Server config reset to default values");
}
```

## 📋 **Default Values (Chuẩn IEC 60870-5-104):**

```csharp
TimeoutT0 = 30000,  // 30 seconds connection timeout
TimeoutT1 = 15000,  // 15 seconds ack timeout  
TimeoutT2 = 10000,  // 10 seconds ack idle timeout
TimeoutT3 = 20000,  // 20 seconds test frame timeout

MaxUnconfirmedAPDU = 12,      // k = 12 (default)
MaxUnacknowledgedAPDU = 8,    // w = 8 (default)

CotFieldLength = 2,           // 2 bytes COT field
CaFieldLength = 2,            // 2 bytes CA field  
IoaFieldLength = 3,           // 3 bytes IOA field
```

## 🎯 **Workflow Fix:**

### **Scenario 1: Auto-Fix**
1. Load config từ file
2. Detect invalid timeout values
3. Auto-correct với default values
4. Save corrected config
5. Continue với valid values

### **Scenario 2: Manual Reset**
```csharp
// Trong MainForm hoặc từ menu
mainForm.ResetServerConfig();
```

### **Scenario 3: Runtime Protection**
```csharp
// Nếu config vẫn sai, runtime validation sẽ fix
int validT1 = ValidateTimeout(invalidValue, 15000, "T1");
_server.SetMaxTimeNoAckReceived(validT1);
```

##  **Expected Results:**

### **Before Fix:**
```
[14:07:03] FATAL ERROR starting server: Invalid NoACK received timeout: 15, 
time must be between 1000ms and 255000ms
```

### **After Fix:**
```
[14:07:03]  Server config loaded successfully
[14:07:03] T1: 15000ms, T2: 10000ms, T3: 20000ms
[14:07:03]  IEC104 Server started successfully
[14:07:03] 📡 Server listening on 127.0.0.1:2404
```

## 🔧 **Manual Fix Commands:**

### **If still having issues:**
```csharp
// Reset config to defaults
mainForm.ResetServerConfig();

// Or delete config file to force recreation
// File: server_config.json in application folder
```

### **Verify config values:**
```csharp
LogMessage($"T1: {_serverConfig.TimeoutT1}ms");  // Should be >= 1000
LogMessage($"T2: {_serverConfig.TimeoutT2}ms");  // Should be >= 1000  
LogMessage($"T3: {_serverConfig.TimeoutT3}ms");  // Should be >= 1000
```

---

**Kết quả:** Server sẽ start thành công với timeout values hợp lệ, không còn FATAL ERROR!
