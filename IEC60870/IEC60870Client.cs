using System;
using System.Collections.Generic;
using System.Threading;
using IEC60870.SAP;
using IEC60870.Object;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
using System.Threading.Tasks;

namespace IEC60870Driver
{
    public class IEC60870Client : IDisposable
    {
        #region FIELDS

        private ClientSAP clientSAP;
        private readonly Dictionary<int, object> dataBuffer;
        private readonly object lockObject = new object();
        private bool isConnected = false;
        private readonly AutoResetEvent responseReceived = new AutoResetEvent(false);
        private readonly Dictionary<int, TypeId> ioaTypeMapping = new Dictionary<int, TypeId>();

        // THÊM: Chờ xác nhận lệnh ghi
        private class CommandAwaiter
        {
            public readonly ManualResetEventSlim Ev = new ManualResetEventSlim(false);
            public bool Success;
        }
        private readonly object pendingLock = new object();
        private readonly Dictionary<string, CommandAwaiter> pending = new Dictionary<string, CommandAwaiter>();
        private string Key(TypeId t, int ioa) => $"{t}:{ioa}";
        #endregion

        #region PROPERTIES

        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int CommonAddress { get; set; }
        public int OriginatorAddress { get; set; }
        public bool IsConnected => isConnected && clientSAP != null;

        // Hint timeout để chờ xác nhận lệnh (ms), được set từ DeviceSettings.WriteTimeout
        public int WriteTimeoutHintMs { get; set; } = 3000;

        // Cho phép bật/tắt việc CHỜ xác nhận lệnh từ thiết bị
        public bool RequireCommandConfirmation { get; set; } = false; // false => hành vi giống console: gửi là xong

        // Cho phép cấu hình độ dài trường COT/CA/IOA từ DeviceSettings
        public int CotFieldLength { get; set; } = 2;           // 1 hoặc 2
        public int CommonAddressFieldLength { get; set; } = 2; // 1 hoặc 2
        public int IoaFieldLength { get; set; } = 3;           // 2 hoặc 3

        #endregion

        #region CONSTRUCTORS

        public IEC60870Client()
        {
            dataBuffer = new Dictionary<int, object>();
        }

        #endregion

        #region CONNECTION

        public bool Connect()
        {
            try
            {
                if (isConnected) return true;

                clientSAP = new ClientSAP(IpAddress, Port);

                // THÊM: Set connection parameters từ cấu hình
                clientSAP.SetCotFieldLength(CotFieldLength);
                clientSAP.SetCommonAddressFieldLength(CommonAddressFieldLength);
                clientSAP.SetIoaFieldLength(IoaFieldLength);
                clientSAP.SetOriginatorAddress(OriginatorAddress);

                // Set timeouts
                clientSAP.SetMaxTimeNoAckReceived(15000);
                clientSAP.SetMaxTimeNoAckSent(10000);
                clientSAP.SetMaxIdleTime(20000);

                // Setup event handlers
                clientSAP.NewASdu += OnNewASdu;
                clientSAP.ConnectionClosed += OnConnectionClosed;

                // Connect to server
                clientSAP.Connect();
                isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                isConnected = false;
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                isConnected = false;
                if (clientSAP != null)
                {
                    clientSAP.ConnectionClosed -= OnConnectionClosed;
                    clientSAP.NewASdu -= OnNewASdu;
                    try { clientSAP.Disconnect(); } catch { }
                }
                clientSAP = null;
            }
            catch { }
        }

        #endregion

        #region EVENT HANDLERS

        private void OnNewASdu(ASdu asdu)
        {
            try
            {
                lock (lockObject)
                {
                    ProcessASdu(asdu);
                }
                responseReceived.Set(); // wake any waiters
            }
            catch (Exception)
            {
                // Log error
            }
        }

        private void OnConnectionClosed(Exception exception)
        {
            isConnected = false;
            // Do not auto-reconnect here; let outer ClientAdapter control reconnection
        }
        public bool GetValueSmart(int ioa, out object value, out TypeId detectedTypeId)
        {
            lock (dataBufferLock)
            {
                if (ioaTypeMapping.ContainsKey(ioa) && dataBuffer.ContainsKey(ioa))
                {
                    detectedTypeId = ioaTypeMapping[ioa];
                    value = dataBuffer[ioa];
                    return true;
                }
                else
                {
                    detectedTypeId = default(TypeId); // TypeId.M_SP_NA_1 (giá trị đầu tiên trong enum)
                    value = null;
                    return false;
                }
            }
        }
        private void TryReconnect()
        {
            if (!isConnected)
            {
                Connect();
            }
        }

        private readonly object dataBufferLock = new object(); // THÊM FIELD

        //private void ProcessASdu(ASdu asdu)
        //{
        //    try
        //    {
        //        Console.WriteLine($"[DEBUG] Processing ASDU: {asdu.GetTypeIdentification()}, COT: {asdu.GetCauseOfTransmission()}");

        //        var informationObjects = asdu.GetInformationObjects();

        //        foreach (var informationObject in informationObjects)
        //        {
        //            var ioa = informationObject.GetInformationObjectAddress();
        //            var elements = informationObject.GetInformationElements();

        //            Console.WriteLine($"[DEBUG] Processing IOA: {ioa}");

        //            if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
        //            {
        //                var element = elements[0][0];
        //                var typeId = asdu.GetTypeIdentification(); // SỬA: GetTypeId() → GetTypeIdentification()

        //                object extractedValue = ExtractValueSafely(element, typeId);
        //                if (extractedValue != null)
        //                {
        //                    lock (dataBufferLock) // THÊM THREAD SAFETY
        //                    {
        //                        dataBuffer[ioa] = extractedValue;
        //                        Console.WriteLine($"[DEBUG] Stored IOA {ioa}: {extractedValue}");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ERROR] ProcessASdu failed: {ex.Message}");
        //    }
        //}
        private void ProcessASdu(ASdu asdu)
        {
            try
            {
                var typeId = asdu.GetTypeIdentification();
                var cot = asdu.GetCauseOfTransmission();
                var informationObjects = asdu.GetInformationObjects();

                // XỬ LÝ: Xác nhận lệnh ghi
                bool isCommandType = typeId == TypeId.C_SC_NA_1 || typeId == TypeId.C_DC_NA_1 ||
                                     typeId == TypeId.C_SE_NC_1 || typeId == TypeId.C_SE_NB_1;
                if (isCommandType && (cot == CauseOfTransmission.ACTIVATION_CON || cot == CauseOfTransmission.ACTIVATION_TERMINATION))
                {
                    foreach (var informationObject in informationObjects)
                    {
                        var ioa = informationObject.GetInformationObjectAddress();
                        var key = Key(typeId, ioa);
                        CommandAwaiter aw = null;
                        lock (pendingLock) pending.TryGetValue(key, out aw);
                        if (aw != null)
                        {
                            aw.Success = !asdu.IsNegativeConfirm();
                            aw.Ev.Set();
                        }
                    }
                    return; // Không đưa vào buffer dữ liệu
                }

                // MẶC ĐỊNH: Lưu các bản tin dữ liệu vào buffer
                foreach (var informationObject in informationObjects)
                {
                    var ioa = informationObject.GetInformationObjectAddress();
                    var elements = informationObject.GetInformationElements();

                    if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
                    {
                        var element = elements[0][0];
                        object extractedValue = ExtractValueSafely(element, typeId);

                        if (extractedValue != null)
                        {
                            lock (dataBufferLock)
                            {
                                dataBuffer[ioa] = extractedValue;
                                ioaTypeMapping[ioa] = typeId; //  LƯU MAPPING
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ProcessASdu failed: {ex.Message}");
            }
        }
        private object ExtractValueSafely(InformationElement element, TypeId typeId)
        {
            try
            {

                switch (typeId)
                {
                    case TypeId.M_SP_NA_1: // Single point information
                        if (element is IeSinglePointWithQuality singlePoint)
                        {
                            var value = singlePoint.IsOn();
                            return value;
                        }
                        break;

                    case TypeId.M_ME_NA_1: // Normalized value
                        if (element is IeNormalizedValue normalizedValue)
                        {
                            var value = normalizedValue.GetValue();
                            return value;
                        }
                        break;

                    case TypeId.M_ME_NC_1: // Short Float - THÊM MỚI
                        if (element is IeShortFloat shortFloat)
                        {
                            var value = shortFloat.GetValue();
                            return value;
                        }
                        break;

                    case TypeId.M_DP_NA_1: // Double point
                        if (element is IeDoublePointWithQuality doublePoint)
                        {
                            var value = doublePoint.GetDoublePointInformation();
                            return value;
                        }
                        break;

                    case TypeId.M_ME_NB_1: // Scaled Value - THÊM MỚI
                        if (element is IeScaledValue scaledValue)
                        {
                            var value = scaledValue.GetValue();

                            return value;
                        }
                        break;
                }

                return element?.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region READ/WRITE OPERATIONS

        public void SendInterrogation(int commonAddress, int ioa = 0)
        {
            try
            {
                if (!IsConnected) return;

                // Use built-in method from ClientSAP
                var qualifier = new IeQualifierOfInterrogation(20);
                clientSAP.Interrogation(commonAddress, CauseOfTransmission.ACTIVATION, qualifier);
            }
            catch (Exception ex)
            {
            }
        }

        public bool GetValue(int ioa, out object value)
        {
            lock (dataBufferLock) // THÊM THREAD SAFETY
            {
                return dataBuffer.TryGetValue(ioa, out value);
            }
        }

        public bool SendCommand(int commonAddress, int ioa, TypeId typeId, string value)
        {
            try
            {
                if (!IsConnected) return false;

                ASdu commandAsdu = null;

                switch (typeId)
                {
                    case TypeId.C_SC_NA_1: // Single command
                        var singleCmd = new IeSingleCommand(value == "1", 0, false);
                        commandAsdu = new ASdu(TypeId.C_SC_NA_1, false, CauseOfTransmission.ACTIVATION,
                            false, false, OriginatorAddress, commonAddress,
                            new[] { new InformationObject(ioa, new[] { new InformationElement[] { singleCmd } }) });
                        break;

                    case TypeId.C_SE_NC_1: // Set point command, short floating point
                        if (float.TryParse(value, out float floatVal))
                        {
                            var shortFloat = new IeShortFloat(floatVal);
                            var qualifier = new IeQualifierOfSetPointCommand(0, false);
                            commandAsdu = new ASdu(TypeId.C_SE_NC_1, false, CauseOfTransmission.ACTIVATION,
                                false, false, OriginatorAddress, commonAddress,
                                new[] { new InformationObject(ioa, new[] { new InformationElement[] { shortFloat, qualifier } }) });
                        }
                        break;

                        // Add other command types as needed
                }

                if (commandAsdu != null)
                {
                    if (!RequireCommandConfirmation)
                    {
                        // Hành vi giống console trực tiếp: gửi là xong
                        clientSAP.SendASdu(commandAsdu);
                        return true;
                    }

                    // Require confirmation: đăng ký awaiter và chờ ACT_CON/ACT_TERM
                    var awaiter = new CommandAwaiter();
                    var key = Key(typeId, ioa);
                    lock (pendingLock) pending[key] = awaiter;

                    clientSAP.SendASdu(commandAsdu);

                    int timeoutMs = WriteTimeoutHintMs > 0 ? WriteTimeoutHintMs : 3000;
                    bool signaled = awaiter.Ev.Wait(timeoutMs);

                    lock (pendingLock) pending.Remove(key);
                    return signaled && awaiter.Success;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region DISPOSE

        public void Dispose()
        {
            try
            {
                Disconnect();
                responseReceived?.Dispose();
            }
            catch { }
        }

        #endregion
    }
}