// File: Services/IEC60870ServerService.cs (Phiên bản An Toàn, Quản lý Luồng Trực tiếp)
using System;
using System.Threading; // <-- Thêm namespace quan trọng này
using IEC60870.Object;
using IEC60870.SAP;
using IEC60870ServerWinForm.Models;

namespace IEC60870ServerWinForm.Services
{
    public class IEC60870ServerService
    {
        private ServerSAP _server;
        private Thread _serverThread; // Biến để quản lý trực tiếp luồng của server
        private readonly object _serverLock = new object();

        public bool IsRunning { get; private set; }

        public event Action<string> OnLogMessage;
        public event Action<ASdu> OnAsduReceived;

        /// <summary>
        /// Bắt đầu server bằng cách tạo và quản lý một luồng (Thread) riêng.
        /// </summary>
        public void Start(ServerConfig config)
        {
            Log("Attempting to start server...");

            lock (_serverLock)
            {
                if (IsRunning)
                {
                    Log("Start command ignored: Server is already running.");
                    return;
                }

                try
                {
                    _server = new ServerSAP(config.IPAddress, config.Port);

                    // Cấu hình các tham số server
                    _server.SetCotFieldLength((byte)config.CotFieldLength);
                    _server.SetCommonAddressFieldLength((byte)config.CaFieldLength);

                    //  SỬA LỖI: Config đã là milliseconds, không cần nhân 1000
                    // Validate và fix timeout values trước khi set
                    int t1 = ValidateTimeout(config.TimeoutT1, 15000, "T1");
                    int t2 = ValidateTimeout(config.TimeoutT2, 10000, "T2");
                    int t3 = ValidateTimeout(config.TimeoutT3, 20000, "T3");

                    _server.SetMaxTimeNoAckReceived(t1);
                    _server.SetMaxTimeNoAckSent(t2);
                    _server.SetMaxIdleTime(t3);

                    _server.NewASdu += OnNewAsduReceivedHandler;

                    // Tạo một luồng mới để chạy vòng lặp lắng nghe của server
                    _serverThread = new Thread(() =>
                    {
                        try
                        {
                            // Hàm này sẽ block luồng cho đến khi có lỗi hoặc bị dừng
                            _server.StartListen(10);
                        }
                        catch (Exception ex)
                        {
                            // Khi Stop() được gọi và Dispose() được thực thi,
                            // một ngoại lệ sẽ xảy ra ở đây, giúp kết thúc luồng.
                            if (IsRunning) // Chỉ ghi log nếu lỗi xảy ra bất ngờ
                            {
                                Log($"Server listener thread crashed: {ex.Message}");
                            }
                        }
                    });

                    _serverThread.IsBackground = true; // Đảm bảo luồng tự tắt khi ứng dụng đóng
                    _serverThread.Start();

                    IsRunning = true;
                    Log($"Server started successfully on {config.IPAddress}:{config.Port}");
                }
                catch (Exception ex)
                {
                    Log($"FATAL ERROR starting server: {ex.Message}");
                    _server = null;
                    IsRunning = false;
                }
            }
        }

        /// <summary>
        /// Dừng server một cách an toàn và triệt để.
        /// Hàm này sẽ block cho đến khi luồng server thực sự kết thúc.
        /// </summary>
        public void Stop()
        {
            Log("Stop command received. Forcing server to terminate...");

            lock (_serverLock)
            {
                if (!IsRunning)
                {
                    Log("Stop command ignored: Server is not running.");
                    return;
                }

                try
                {
                    // Thực hiện chu trình tắt ép buộc và an toàn
                    if (_server != null)
                    {
                        _server.NewASdu -= OnNewAsduReceivedHandler;
                        if (_server is IDisposable disposable)
                        {
                            disposable.Dispose(); // Gây ra exception trong luồng server để nó thoát ra
                        }
                    }

                    if (_serverThread != null && _serverThread.IsAlive)
                    {
                        // Dùng Join() để đợi luồng kết thúc một cách tự nhiên.
                        Log("Waiting for server thread to exit...");
                        bool stoppedGracefully = _serverThread.Join(TimeSpan.FromSeconds(2)); // Đợi tối đa 2 giây

                        if (stoppedGracefully)
                        {
                            Log("Server thread exited gracefully.");
                        }
                        else
                        {
                            // Nếu vẫn không xong, dùng biện pháp mạnh nhất
                            Log("Thread did not exit gracefully. Aborting...");
                            _serverThread.Abort();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"An error occurred during shutdown: {ex.Message}");
                }
                finally
                {
                    // Dọn dẹp cuối cùng, đảm bảo sẵn sàng cho lần Start tiếp theo
                    _server = null;
                    _serverThread = null;
                    IsRunning = false;
                    Log("Server has been fully stopped.");
                }
            }
        }

        /// <summary>
        /// Gửi dữ liệu đi một cách an toàn.
        /// </summary>
        public void BroadcastAsdu(ASdu asdu)
        {
            if (!IsRunning || _server == null)
            {
                // Không ghi log ở đây để tránh làm phiền khi gửi nhiều
                return;
            }
            try
            {
                _server.SendASdu(asdu);
            }
            catch (Exception ex)
            {
                Log($"Failed to broadcast ASDU: {ex.Message}");
            }
        }

        private void OnNewAsduReceivedHandler(ASdu asdu)
        {
            if (!IsRunning) return;
            OnAsduReceived?.Invoke(asdu);
        }

        /// <summary>
        ///  THÊM MỚI: Validate timeout values
        /// </summary>
        private int ValidateTimeout(int value, int defaultValue, string timeoutName)
        {
            // T1, T2: 1000ms - 255000ms
            // T3: 1000ms - 172800000ms (48 hours)
            int maxValue = timeoutName == "T3" ? 172800000 : 255000;

            if (value < 1000 || value > maxValue)
            {
                Log($"⚠️  Invalid {timeoutName} timeout: {value}ms. Using default: {defaultValue}ms");
                return defaultValue;
            }
            return value;
        }

        private void Log(string message)
        {
            OnLogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}