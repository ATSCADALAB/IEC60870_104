using System;

namespace IEC60870Driver
{
    /// <summary>
    /// Base exception for IEC60870 driver operations
    /// </summary>
    public class IEC60870Exception : Exception
    {
        public IEC60870Exception(string message) : base(message) { }
        public IEC60870Exception(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when a tag/IOA is not found or does not exist
    /// </summary>
    public class TagNotFoundException : IEC60870Exception
    {
        public int IOA { get; }
        public string TagAddress { get; }

        public TagNotFoundException(int ioa) 
            : base($"Tag with IOA {ioa} not found or does not exist")
        {
            IOA = ioa;
            TagAddress = ioa.ToString();
        }

        public TagNotFoundException(string tagAddress) 
            : base($"Tag '{tagAddress}' not found or does not exist")
        {
            TagAddress = tagAddress;
        }

        public TagNotFoundException(int ioa, string message) 
            : base($"Tag with IOA {ioa}: {message}")
        {
            IOA = ioa;
            TagAddress = ioa.ToString();
        }
    }

    /// <summary>
    /// Exception thrown when a read/write operation times out
    /// </summary>
    public class IEC60870TimeoutException : IEC60870Exception
    {
        public int TimeoutMs { get; }
        public string Operation { get; }

        public IEC60870TimeoutException(string operation, int timeoutMs) 
            : base($"{operation} operation timed out after {timeoutMs}ms")
        {
            Operation = operation;
            TimeoutMs = timeoutMs;
        }

        public IEC60870TimeoutException(string operation, int timeoutMs, Exception innerException) 
            : base($"{operation} operation timed out after {timeoutMs}ms", innerException)
        {
            Operation = operation;
            TimeoutMs = timeoutMs;
        }
    }

    /// <summary>
    /// Exception thrown when connection to device fails
    /// </summary>
    public class IEC60870ConnectionException : IEC60870Exception
    {
        public string IpAddress { get; }
        public int Port { get; }

        public IEC60870ConnectionException(string ipAddress, int port) 
            : base($"Failed to connect to {ipAddress}:{port}")
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public IEC60870ConnectionException(string ipAddress, int port, string message) 
            : base($"Connection to {ipAddress}:{port} failed: {message}")
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public IEC60870ConnectionException(string ipAddress, int port, Exception innerException) 
            : base($"Failed to connect to {ipAddress}:{port}", innerException)
        {
            IpAddress = ipAddress;
            Port = port;
        }
    }

    /// <summary>
    /// Exception thrown when trying to write to a read-only IOA
    /// </summary>
    public class ReadOnlyIOAException : IEC60870Exception
    {
        public int IOA { get; }

        public ReadOnlyIOAException(int ioa) 
            : base($"IOA {ioa} is read-only and cannot be written to")
        {
            IOA = ioa;
        }
    }

    /// <summary>
    /// Exception thrown when trying to read from a write-only IOA
    /// </summary>
    public class WriteOnlyIOAException : IEC60870Exception
    {
        public int IOA { get; }
        public string WriteOnlyValue { get; }

        public WriteOnlyIOAException(int ioa, string writeOnlyValue = "WRITE_ONLY") 
            : base($"IOA {ioa} is write-only and cannot be read from")
        {
            IOA = ioa;
            WriteOnlyValue = writeOnlyValue;
        }
    }

    /// <summary>
    /// Exception thrown when device configuration is invalid
    /// </summary>
    public class IEC60870ConfigurationException : IEC60870Exception
    {
        public string Parameter { get; }

        public IEC60870ConfigurationException(string parameter, string message) 
            : base($"Invalid configuration for {parameter}: {message}")
        {
            Parameter = parameter;
        }
    }

    /// <summary>
    /// Exception thrown when data type conversion fails
    /// </summary>
    public class IEC60870DataTypeException : IEC60870Exception
    {
        public string SourceType { get; }
        public string TargetType { get; }
        public object Value { get; }

        public IEC60870DataTypeException(object value, string sourceType, string targetType) 
            : base($"Cannot convert value '{value}' from {sourceType} to {targetType}")
        {
            Value = value;
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}
