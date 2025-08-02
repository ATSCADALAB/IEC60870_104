// File: Models/ClientInfo.cs
using System;

namespace IEC60870ServerWinForm.Models
{
    /// <summary>
    /// Lưu trữ thông tin về một client đang kết nối.
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// ID định danh cho kết nối do server cấp.
        /// </summary>
        public int ConnectionId { get; set; }

        /// <summary>
        /// Địa chỉ IP của client.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Cổng (port) của client.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Thời điểm client kết nối tới server.
        /// </summary>
        public DateTime ConnectedTime { get; set; }
    }
}