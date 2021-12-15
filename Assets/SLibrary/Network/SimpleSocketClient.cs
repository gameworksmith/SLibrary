using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace SLibrary.Network
{
    public class SimpleSocketClient
    {
        public const int BUFFER_SIZE = 1024;

        private Socket _socket;
        private string _ip;
        private int _port;
        private System.Action<byte[], int> _onReceive;
        private System.Action<string, int> _onFirstConnected;
        private byte[] _buffer;
        private byte[] _totalBuffer;
        private object _state;
        private bool _reading;

        public static SimpleSocketClient CreateClient(string ip, int port, System.Action<string, int> onFistConnected = null,
            System.Action<byte[], int> onReceive = null)
        {
            SimpleSocketClient client = new SimpleSocketClient();
            client.Init(ip, port, onFistConnected, onReceive);

            return client;
        }


        private void Init(string ip, int port, Action<string, int> onFistConnected, Action<byte[], int> onReceive)
        {
            _ip = ip;
            _port = port;
            _onReceive = onReceive;
            _onFirstConnected = onFistConnected;
            _buffer = new byte[BUFFER_SIZE];
            _state = new object();
        }

        public bool IsConnected()
        {
            return _socket != null && !((_socket.Poll(1000, SelectMode.SelectRead) && (_socket.Available == 0)) || !_socket.Connected);
        }

        public void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveBufferSize = BUFFER_SIZE;
            _socket.ReceiveTimeout = 10;
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _socket.BeginConnect(ipe, OnAsyncConnected, null);
        }

        private void OnAsyncConnected(IAsyncResult ar)
        {
            _socket.EndConnect(ar);
            if (IsConnected())
            {
                _socket.BeginReceive(_buffer, 0, BUFFER_SIZE, SocketFlags.None, OnAsyncReceive, _state);
            }

            _onFirstConnected?.Invoke(_ip, _port);
        }

        private void OnAsyncReceive(IAsyncResult ar)
        {
            // Debug.Log($"接收数据，连接状态{IsConnected()}");
            if (_socket == null)
            {
                return;
            }
            if (!IsConnected())
            {
                _socket.Disconnect(false);
                _socket.Dispose();
                Connect();
            }
            else

            {
                int length = _socket.EndReceive(ar);
                if (!ar.IsCompleted) return;
                _onReceive?.Invoke(_buffer, length);
                // Debug.Log($"接收数据，是否完成{_socket} 当前可用数据长度{length} {_socket.Available} ");
                _socket.BeginReceive(_buffer, 0, BUFFER_SIZE, SocketFlags.None, OnAsyncReceive, _state);
            }
        }

        public void Stop()
        {
            if (_socket != null && _socket.Connected)
            {
                _socket.Disconnect(false);
                _socket.Dispose();
                for (int i = 0; i < _buffer.Length; i++)
                {
                    _buffer[i] = 0;

                }
                Debug.Log("成功停止");
            }
            else
            {
                Debug.Log("已断开连接，无需再断");
            }

            _socket = null;
        }

        public void AsyncSend(byte[] data)
        {
            try
            {
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成发送消息  
                    int length = _socket.EndSend(asyncResult);
                    // Debug.Log($"客户端发送消息:{data}, 长度{length}");
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);
            }
        }
    }
}