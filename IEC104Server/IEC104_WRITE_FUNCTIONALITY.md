# IEC104 Write Functionality Summary

## 🎯 **Bidirectional Communication Flow:**

```
IEC104 Client → IEC104 Server → SCADA System
     ↑                              ↓
     └── Confirmation ←── Write ←────┘
```

### **Read Direction:**
```
SCADA Tag → IEC104 Server → IEC104 Client
iDriver.Task("TASK").Tag("TAG").Value → IOA → Client
```

### **Write Direction:**
```
IEC104 Client → IEC104 Server → SCADA Tag
Client Command → IOA → iDriver.Task("TASK").Tag("TAG").Value = "value"
```

##  **Implemented Write Commands:**

### **1. Single Command (Bool) - C_SC_NA_1**
```csharp
// Client sends: Single Command to IOA 3
// Server processes: HandleSingleCommand()
// Server writes: iDriver.Task("PLC1").Tag("PumpStatus").Value = "true"
// Server confirms: Sends activation confirmation back to client
```

### **2. Set Point Float (Float) - C_SE_NC_1**
```csharp
// Client sends: Set Point Float to IOA 1
// Server processes: HandleSetPointFloatCommand()
// Server writes: iDriver.Task("PLC1").Tag("Temperature").Value = "25.5"
// Server confirms: Sends activation confirmation back to client
```

### **3. Set Point Int (Int) - C_SE_NB_1**
```csharp
// Client sends: Set Point Int to IOA 2
// Server processes: HandleSetPointIntCommand()
// Server writes: iDriver.Task("PLC1").Tag("Pressure").Value = "5"
// Server confirms: Sends activation confirmation back to client
```

## 🔧 **Core Write Function:**

### **WriteToSCADA Method:**
```csharp
private void WriteToSCADA(int ioa, object value)
{
    // 1. Find DataPoint by IOA
    var dataPoint = _dataPoints.FirstOrDefault(dp => dp.IOA == ioa);
    
    // 2. Parse DataTagName (format: "TaskName.TagName")
    var parts = dataPoint.DataTagName.Split('.');
    string taskName = parts[0];  // "PLC1"
    string tagName = parts[1];   // "Temperature"
    
    // 3.  WRITE TO SCADA using iDriver
    _driverManager.Driver.Task(taskName).Tag(tagName).Value = value.ToString();
    
    LogMessage($" SCADA Write: {dataPoint.DataTagName} = {value}");
}
```

##  **Command Processing Flow:**

### **1. Command Reception:**
```csharp
private void HandleReceivedAsdu(ASdu asdu)
{
    // Receive ASDU from IEC104 client
    HandleClientCommands(asdu);
}
```

### **2. Command Routing:**
```csharp
private void HandleClientCommands(ASdu asdu)
{
    switch (typeId)
    {
        case TypeId.C_SC_NA_1:  // Single Command
            HandleSingleCommand(asdu, informationObjects);
            break;
            
        case TypeId.C_SE_NC_1:  // Set Point Float
            HandleSetPointFloatCommand(asdu, informationObjects);
            break;
            
        case TypeId.C_SE_NB_1:  // Set Point Int
            HandleSetPointIntCommand(asdu, informationObjects);
            break;
    }
}
```

### **3. Value Extraction & Write:**
```csharp
private void HandleSingleCommand(ASdu asdu, InformationObject[] informationObjects)
{
    // Extract IOA and command value
    var ioa = infoObj.GetInformationObjectAddress();
    bool commandValue = scs.IsCommandStateOn();
    
    // Write back to SCADA
    WriteToSCADA(ioa, commandValue);
    
    // Send confirmation
    SendCommandConfirmation(ioa, TypeId.C_SC_NA_1, commandValue);
}
```

## 🎛️ **Example Scenarios:**

### **Scenario 1: Pump Control**
```
Configuration:
IOA: 3
Name: "Pump_Status"
DataTagName: "PLC1.PumpStatus"
Type: M_SP_NA_1 (Bool)

Write Command:
Client → Server: C_SC_NA_1, IOA=3, Value=true
Server → SCADA: iDriver.Task("PLC1").Tag("PumpStatus").Value = "true"
Server → Client: Activation Confirmation

Log Output:
🎛️  Single Command: IOA=3, Value=True
 SCADA Write: PLC1.PumpStatus = True
🔄 Written to SCADA: Task='PLC1', Tag='PumpStatus', Value='True'
 Command confirmation sent: IOA=3, Type=C_SC_NA_1, Value=True
```

### **Scenario 2: Temperature Setpoint**
```
Configuration:
IOA: 1
Name: "Temperature_Setpoint"
DataTagName: "PLC1.TempSetpoint"
Type: M_ME_NC_1 (Float)

Write Command:
Client → Server: C_SE_NC_1, IOA=1, Value=25.5
Server → SCADA: iDriver.Task("PLC1").Tag("TempSetpoint").Value = "25.5"
Server → Client: Activation Confirmation

Log Output:
 Set Point Float: IOA=1, Value=25.5
 SCADA Write: PLC1.TempSetpoint = 25.5
🔄 Written to SCADA: Task='PLC1', Tag='TempSetpoint', Value='25.5'
 Command confirmation sent: IOA=1, Type=C_SE_NC_1, Value=25.5
```

### **Scenario 3: Pressure Setpoint**
```
Configuration:
IOA: 2
Name: "Pressure_Setpoint"
DataTagName: "PLC1.PressureSetpoint"
Type: M_ME_NB_1 (Int)

Write Command:
Client → Server: C_SE_NB_1, IOA=2, Value=5
Server → SCADA: iDriver.Task("PLC1").Tag("PressureSetpoint").Value = "5"
Server → Client: Activation Confirmation

Log Output:
 Set Point Int: IOA=2, Value=5
 SCADA Write: PLC1.PressureSetpoint = 5
🔄 Written to SCADA: Task='PLC1', Tag='PressureSetpoint', Value='5'
 Command confirmation sent: IOA=2, Type=C_SE_NB_1, Value=5
```

## ⚠️ **Error Handling:**

### **Common Error Cases:**
```csharp
// IOA not found
❌ Write failed: IOA 999 not found in configuration

// No SCADA tag configured
❌ Write failed: IOA 3 has no SCADA tag configured

// Invalid tag format
❌ Write failed: Invalid tag format 'InvalidTag'. Expected 'TaskName.TagName'

// Driver not available
❌ Write failed: SCADA driver not available

// General write error
❌ Error writing to SCADA: Connection timeout
```

## 🔄 **Complete Write Cycle:**

### **1. Client Sends Command:**
```
IEC104 Client → C_SC_NA_1 (IOA=3, Value=true)
```

### **2. Server Processes:**
```
HandleClientCommands() → HandleSingleCommand()
Extract: IOA=3, Value=true
Find DataPoint: "PLC1.PumpStatus"
```

### **3. Server Writes to SCADA:**
```
iDriver.Task("PLC1").Tag("PumpStatus").Value = "true"
```

### **4. Server Confirms:**
```
SendCommandConfirmation() → ACTIVATION_CON
```

### **5. SCADA Updates:**
```
PLC1.PumpStatus = true (in SCADA system)
```

### **6. Next Read Cycle:**
```
Read: iDriver.Task("PLC1").Tag("PumpStatus").Value → "true"
Send: M_SP_NA_1 (IOA=3, Value=true) → Client
```

## 💡 **Key Benefits:**

### **1. True Bidirectional Communication:**
- Read: SCADA → IEC104 Client
- Write: IEC104 Client → SCADA

### **2. Automatic Tag Mapping:**
- IOA ↔ DataTagName mapping
- No manual configuration needed

### **3. Standard IEC104 Protocol:**
- C_SC_NA_1 for Bool commands
- C_SE_NC_1 for Float setpoints
- C_SE_NB_1 for Int setpoints

### **4. Robust Error Handling:**
- Validation at every step
- Clear error messages
- Graceful failure handling

### **5. Confirmation Protocol:**
- Standard IEC104 confirmations
- Client knows command was executed
- Proper protocol compliance

---

**Kết quả:** Hoàn chỉnh tính năng Write với bidirectional communication: IEC104 Client ↔ IEC104 Server ↔ SCADA System! 
