// File: Models/ServerConfig.cs

namespace IEC60870ServerWinForm.Models
{
    /// <summary>
    /// Lưu trữ tất cả các thông số cấu hình cho IEC 60870-5-104 Server.
    /// </summary>
    public class ServerConfig
    {
        // --- Cấu hình mạng cơ bản ---
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 2404;

        // --- Cấu hình giao thức IEC ---
        public int CommonAddress { get; set; } = 1;
        public int CotFieldLength { get; set; } = 2;    // Cause of Transmission field length
        public int CaFieldLength { get; set; } = 2;     // Common Address field length
        public int IoaFieldLength { get; set; } = 3;    // Information Object Address field length

        // --- Cấu hình Timeouts (đơn vị: giây) ---
        /// <summary>
        /// Connection establishment timeout.
        /// </summary>
        public int TimeoutT0 { get; set; } = 30;

        /// <summary>
        /// Response timeout for sent messages.
        /// </summary>
        public int TimeoutT1 { get; set; } = 15;

        /// <summary>
        /// Timeout for receiving acknowledgements for S-frames.
        /// </summary>
        public int TimeoutT2 { get; set; } = 10;

        /// <summary>
        /// Timeout for receiving test frames in idle state (keep-alive).
        /// </summary>
        public int TimeoutT3 { get; set; } = 20;
    }
}