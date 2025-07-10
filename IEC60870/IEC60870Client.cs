using System;
using System.Collections.Generic;
using System.Threading;
using IEC60870.SAP;
using IEC60870.Object;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;

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

        #endregion

        #region PROPERTIES

        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int CommonAddress { get; set; }
        public int OriginatorAddress { get; set; }
        public bool IsConnected => isConnected && clientSAP != null;

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

                // Setup event handlers
                clientSAP.NewASdu += OnNewASdu;
                clientSAP.ConnectionClosed += OnConnectionClosed;

                // Connect to server
                clientSAP.Connect();
                isConnected = true;

                return true;
            }
            catch (Exception)
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
                responseReceived.Set();
            }
            catch (Exception)
            {
                // Log error
            }
        }

        private void OnConnectionClosed(Exception exception)
        {
            isConnected = false;
            // Log connection closed
        }

        private void ProcessASdu(ASdu asdu)
        {
            try
            {
                var informationObjects = asdu.GetInformationObjects();

                foreach (var informationObject in informationObjects)
                {
                    var ioa = informationObject.GetInformationObjectAddress();
                    var elements = informationObject.GetInformationElements();

                    if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
                    {
                        var element = elements[0][0]; // First element of first array
                        var typeId = asdu.GetTypeId();

                        // Extract value based on type - using safe approach
                        object extractedValue = ExtractValueSafely(element, typeId);
                        if (extractedValue != null)
                        {
                            dataBuffer[ioa] = extractedValue;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log processing error
            }
        }

        private object ExtractValueSafely(InformationElement element, TypeId typeId)
        {
            try
            {
                // Use reflection or type checking to safely extract values
                // This approach is safer and will work regardless of exact class names

                switch (typeId)
                {
                    case TypeId.M_SP_NA_1: // Single point information
                        // Try to get boolean value from single point information
                        if (element != null)
                        {
                            var elementType = element.GetType();
                            var isOnMethod = elementType.GetMethod("IsOn");
                            if (isOnMethod != null)
                                return (bool)isOnMethod.Invoke(element, null);
                        }
                        break;

                    case TypeId.M_ME_NA_1: // Measured value, normalized value
                        if (element != null && element.GetType().Name.Contains("Normalized"))
                        {
                            var elementType = element.GetType();
                            var getValueMethod = elementType.GetMethod("GetValue");
                            if (getValueMethod != null)
                                return getValueMethod.Invoke(element, null);
                        }
                        break;

                    case TypeId.M_ME_NC_1: // Measured value, short floating point
                        if (element != null && element.GetType().Name.Contains("Float"))
                        {
                            var elementType = element.GetType();
                            var getFloatMethod = elementType.GetMethod("GetFloat");
                            if (getFloatMethod != null)
                                return (float)getFloatMethod.Invoke(element, null);
                        }
                        break;

                    default:
                        return element?.ToString();
                }
            }
            catch
            {
                // If extraction fails, return string representation
                return element?.ToString();
            }

            return null;
        }

        #endregion

        #region READ/WRITE OPERATIONS

        public void SendInterrogation(int commonAddress, int ioa = 0)
        {
            try
            {
                if (!IsConnected) return;

                // Create interrogation qualifier
                var qualifier = new IeQualifierOfInterrogation(20);

                // Create and send interrogation command
                var asdu = new ASdu(TypeId.C_IC_NA_1, false, CauseOfTransmission.ACTIVATION,
                    false, false, OriginatorAddress, commonAddress,
                    new[] { new InformationObject(ioa, new[] { new InformationElement[] { qualifier } }) });

                clientSAP.SendASdu(asdu);
            }
            catch (Exception)
            {
                // Log error
            }
        }

        public bool GetValue(int ioa, out object value)
        {
            lock (lockObject)
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
                    clientSAP.SendASdu(commandAsdu);
                    return true;
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