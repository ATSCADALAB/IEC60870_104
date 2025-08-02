// File: Models/DataPoint.cs
using System;

namespace IEC60870ServerWinForm.Models
{
    /// <summary>
    /// Enum định nghĩa các loại dữ liệu được hỗ trợ.
    /// </summary>
    public enum DataType
    {
        Bool,    // Trạng thái bật/tắt (Single Point Information)
        Float,   // Giá trị thực (Measured Value, short floating point)
        Int,     // Giá trị số nguyên (Scaled Value)
        Counter  // Bộ đếm (Integrated Totals)
    }

    /// <summary>
    /// Đại diện cho một điểm dữ liệu trong server IEC 104.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Địa chỉ đối tượng thông tin (Information Object Address - IOA).
        /// Đây là định danh duy nhất của điểm dữ liệu trong giao thức.
        /// </summary>
        public int IOA { get; set; }

        /// <summary>
        /// Tên gợi nhớ của điểm dữ liệu để hiển thị trên giao diện.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả chi tiết về điểm dữ liệu.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Loại dữ liệu (Bool, Float, Int, Counter).
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// Giá trị hiện tại của điểm dữ liệu.
        /// Sử dụng kiểu 'object' để có thể lưu trữ nhiều loại giá trị khác nhau (true/false, 25.5f, 100, ...).
        /// </summary>
        public object Value { get; set; }
    }
}