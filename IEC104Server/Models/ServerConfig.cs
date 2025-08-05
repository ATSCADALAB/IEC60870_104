// File: Models/ServerConfig.cs - Simple Configuration
namespace IEC60870ServerWinForm.Models
{
    public class ServerConfig
    {
        // Network settings
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 2404;

        // IEC protocol settings
        public int CommonAddress { get; set; } = 1;
        public int CotFieldLength { get; set; } = 2;
        public int CaFieldLength { get; set; } = 2;
        public int IoaFieldLength { get; set; } = 3;

        // Timeouts (seconds)
        public int TimeoutT0 { get; set; } = 30;
        public int TimeoutT1 { get; set; } = 15;
        public int TimeoutT2 { get; set; } = 10;
        public int TimeoutT3 { get; set; } = 20;
    }
}