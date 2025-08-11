using IEC60870.Connections;
using IEC60870.Object;
using IEC60870.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;


namespace IEC60870.SAP
{
    internal class ConnectionHandler : ThreadBase
    {
        private readonly Socket _socket;
        private readonly ConnectionSettings _settings;
        private Connection _connection;
        private readonly ConnectionEventListener.NewASdu _newAsduEvent;
        private readonly PubSubHub _pubSubHub;

        public ConnectionHandler(Socket socket, ConnectionSettings settings,
            ConnectionEventListener.NewASdu newASduEvent, PubSubHub pubSubHub)
        {
            _socket = socket;
            _settings = settings;
            _newAsduEvent = newASduEvent;
            _pubSubHub = pubSubHub;

            _pubSubHub.Subscribe<ASdu>(this, "send", asdu =>
            {
                try
                {
                    _connection.Send(asdu);
                }
                catch (Exception e)
                {
                    _pubSubHub.Publish(this, "error", e);
                }
            });
        }

        public override void Run()
        {
            _connection = new Connection(_socket, _settings);
            _connection.ConnectionClosed += e => { _pubSubHub.Publish<Exception>(this, "error", e); };

            _connection.NewASdu += _newAsduEvent;

            _connection.WaitForStartDT(5000);
            // Notify ready state so publisher can start sending safely
            _pubSubHub.Publish(this, "ready", true);
        }
    }

    internal class ServerThread : ThreadBase
    {
        private readonly int _maxConnections;
        private readonly ConnectionSettings _settings;
        private readonly Socket _serverSocket;
        private readonly ConnectionEventListener.NewASdu _newAsduEvent;
        private readonly PubSubHub _pubSubHub;
        private int _connectionCount;

        public ServerThread(Socket serverSocket, ConnectionSettings settings, int maxConnections,
            ConnectionEventListener.NewASdu newASduEvent, PubSubHub pubSubHub)
        {
            _maxConnections = maxConnections;
            _serverSocket = serverSocket;
            _settings = settings;
            _newAsduEvent = newASduEvent;
            _pubSubHub = pubSubHub;
        }

        public override void Run()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var clientSocket = _serverSocket.Accept();
                        if (_connectionCount == _maxConnections)
                        {
                            clientSocket.Close();
                            continue;
                        }

                        var handler = new ConnectionHandler(clientSocket, _settings, _newAsduEvent, _pubSubHub);
                        _handlers.Add(handler);
                        handler.Start();
                        _connectionCount++;
                    }
                    catch (IOException e)
                    {
                        _pubSubHub.Publish<Exception>(this, "error", e);
                    }
                    catch (Exception e)
                    {
                        _pubSubHub.Publish(this, "error", e);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                Abort();
            }
        }
    }

    public class ServerSAP : IDisposable
    {
        private readonly ConnectionSettings _settings = new ConnectionSettings();
        private readonly PubSubHub _pubSubHub = new PubSubHub();
        private readonly IPAddress _host;
        private readonly int _port;
        private const int _maxConnections = 10;

        // Hold references to allow stopping/disposing
        private Socket _serverSocket;
        private ServerThread _serverThread;
        private readonly List<ConnectionHandler> _handlers = new List<ConnectionHandler>();

        public event Action ClientReady;

        public ConnectionEventListener.NewASdu NewASdu { get; set; }

        public ServerSAP(IPAddress host)
        {
            _host = host;
            _port = 2404;
        }

        public ServerSAP(IPAddress host, int port) : this(host)
        {
            _port = port;
        }

        public ServerSAP(string host, int port)
        {
            try
            {
                _host = IPAddress.Parse(host);
                _port = port;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public void StartListen(int backlog)
        {
            var remoteEp = new IPEndPoint(_host, _port);
            _serverSocket = new Socket(_host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _serverSocket.Bind(remoteEp);
            _serverSocket.Listen(backlog);
            _serverThread = new ServerThread(_serverSocket, _settings, _maxConnections, NewASdu, _pubSubHub);
            // Subscribe once to ready events to surface externally
            _pubSubHub.Subscribe<bool>(this, "ready", _ => ClientReady?.Invoke());

            _serverThread.Start();
        }


        public void Stop()
        {
            try
            {
                // Close listening socket to break out Accept()
                _serverSocket?.Close();

                // Abort all connection handlers immediately
                foreach (var h in _handlers)
                {
                    try { h.Abort(); } catch { }
                }
                _handlers.Clear();
            }
            catch { }
            finally
            {
                _serverSocket = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public void SendASdu(ASdu asdu)
        {
            _pubSubHub.Publish(this, "send", asdu);
        }

        public void SetMessageFragmentTimeout(int timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentException("Invalid message fragment timeout: " + timeout);
            }

            _settings.MessageFragmentTimeout = timeout;
        }

        public void SetCotFieldLength(int length)
        {
            if (length != 1 && length != 2)
            {
                throw new ArgumentException("Invalid COT length: " + length);
            }

            _settings.CotFieldLength = length;
        }

        public void SetCommonAddressFieldLength(int length)
        {
            if (length != 1 && length != 2)
            {
                throw new ArgumentException("Invalid CA length: " + length);
            }

            _settings.CommonAddressFieldLength = length;
        }

        public void SetIoaFieldLength(int length)
        {
            if (length < 1 || length > 3)
            {
                throw new ArgumentException("Invalid IOA length: " + length);
            }

            _settings.IoaFieldLength = length;
        }

        public void SetMaxTimeNoAckReceived(int time)
        {
            if (time < 1000 || time > 255000)
            {
                throw new ArgumentException("Invalid NoACK received timeout: " + time
                                            + ", time must be between 1000ms and 255000ms");
            }

            _settings.MaxTimeNoAckReceived = time;
        }

        public void SetMaxTimeNoAckSent(int time)
        {
            if (time < 1000 || time > 255000)
            {
                throw new ArgumentException("Invalid NoACK sent timeout: " + time
                                            + ", time must be between 1000ms and 255000ms");
            }

            _settings.MaxTimeNoAckSent = time;
        }

        public void SetMaxIdleTime(int time)
        {
            if (time < 1000 || time > 172800000)
            {
                throw new ArgumentException("Invalid idle timeout: " + time
                                            + ", time must be between 1000ms and 172800000ms");
            }

            _settings.MaxIdleTime = time;
        }

        public void SetMaxUnconfirmedIPdusReceived(int maxNum)
        {
            if (maxNum < 1 || maxNum > 32767)
            {
                throw new ArgumentException("invalid maxNum: " + maxNum + ", must be a value between 1 and 32767");
            }

            _settings.MaxUnconfirmedIPdusReceived = maxNum;
        }
    }
}