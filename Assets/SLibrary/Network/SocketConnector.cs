using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SLibrary.Network
{
    public class MsgContent
    {
        public int MsgId;
        public int Length;
        public long Time;
        public byte[] Data;

        public override string ToString()
        {
            return $"id {MsgId} length {Length} Time {Time}";
        }
    }

    public class SocketConnector
    {
        // buff最大30k，如果
        public static int BUFFER_MAX_SIZE = 512 * 1024;
        public static int LENGTH_SIZE = 4;
        public static int TIME_SIZE = 8;
        public static int MSGID_SIZE = 4;

        private string _ip;
        private int _port;

        private SimpleSocketClient _client;

        // private MemoryStream _ms;
        /// <summary>
        /// 当前包差多少
        /// </summary>
        /// <summary>
        /// 当前包总的多少
        /// </summary>
        private byte[] _tempBuffer;

        private MemoryStream _ms;
        private MsgContent _readingContent;

        private Queue<MsgContent> _contents = new Queue<MsgContent>();
        private int _headLength;
        /// <summary>
        /// 剩余长度
        /// </summary>
        private int _waitReadLength;

        private object _lock = new object();
        private int _unReadCount;

        public SocketConnector(string ip, int port)
        {
            _client = _client = SimpleSocketClient.CreateClient(ip, port, (s, i) =>
            {
                Debug.Log($"连接成功 {s}:{i}");
                // _client.AsyncSend(System.Text.Encoding.UTF8.GetBytes("ok"));
            }, OnDataReceived);

            _headLength = LENGTH_SIZE + TIME_SIZE + MSGID_SIZE;
            _ms = new MemoryStream(BUFFER_MAX_SIZE);
        }

        public static int ToBigEndian(int value)
        {
            return System.Net.IPAddress.HostToNetworkOrder(value);
        }
        public static long ToBigEndian(long value)
        {
            return System.Net.IPAddress.HostToNetworkOrder(value);
        }

        public int GetQueueCount()
        {
            lock (_lock)
            {
                return _contents.Count;
            }
        }

        public MsgContent GetMsgContent()
        {
            lock (_lock)
            {
                return _contents.Dequeue();
            }
        }
        

        private void OnDataReceived(byte[] obj, int length)
        {
            try
            {
                if (obj.Length == 0 || length == 0)
                {
                    return;
                }

                // Debug.Log($"收到数据，长度{length}");
                // Debug.Log($"写入前数据量{_ms.Length}");
                // Debug.Log($"当前位置 00 {_ms.Position}");
                // 如果小于，必须写进去
                _ms.Write(obj, 0, length);
                _unReadCount += length;
                // Debug.Log($"当前位置 0011 {_ms.Position}");
                // Debug.Log($"写入后数据量{_ms.Length}");

                // // 每次读取都全部读完
                // if (_readingContent == null && _ms.Length < _headLength)
                // {
                //     return;
                // }
                //
                // if (_readingContent != null && _ms.Length  < _remainLength)
                // {
                //     return;
                // }

                while (ContinueReading())
                {
                    // Debug.Log($"当前位置 11 {_ms.Position} 长度 {_ms.Length}");
                    // 回到最开始开始读
                    _ms.Seek(0, SeekOrigin.Begin);
                    // 说明当前正在读取
                    if (_readingContent == null)
                    {
                        _readingContent = new MsgContent();
                        byte[] data = new byte[LENGTH_SIZE];
                        _ms.Read(data, 0, LENGTH_SIZE);
                        _readingContent.Length = ToBigEndian(BitConverter.ToInt32(data, 0));
                        byte[] timeData = new byte[TIME_SIZE];
                        _ms.Read(timeData, 0, TIME_SIZE);
                        _readingContent.Time = ToBigEndian(BitConverter.ToInt64(timeData, 0));
                        _ms.Read(data, 0, LENGTH_SIZE);
                        _readingContent.MsgId = ToBigEndian(BitConverter.ToInt32(data, 0));
                        _readingContent.Data = new byte[_readingContent.Length];
                        // Debug.Log($"当前位置 1111 {_ms.Position} 长度 {_ms.Length}");
                        _waitReadLength = _readingContent.Length;
                        _unReadCount -= _headLength;
                    }

                    // 获取剩下的长度，如果比所需的长，直接处理掉，然后下一次的时候再接着处理，如果比所需的短，就存入msStream;
                    // Debug.Log($"当前位置 22 {_ms.Position} 剩余长度 {_waitReadLength} 可读长度{_unReadCount}");
                    int contentPosition = _readingContent.Length - _waitReadLength;
                    if ( _unReadCount < _waitReadLength)
                    {
                        _ms.Read(_readingContent.Data, contentPosition, _unReadCount);
                        _waitReadLength -= _unReadCount;
                        _unReadCount = 0;
                        // Debug.Log($"长度不足以解析 剩余 {remainLength} 所需 {_readingContent.Length}");
                        // Debug.Log($"当前位置 33 {_ms.Position} 不足，继续写");
                        break;
                    }

                    // Debug.Log($"当前位置 3300 {_unReadCount}  {_waitReadLength}");
                    _ms.Read(_readingContent.Data, contentPosition, _waitReadLength);
                    // Debug.Log($"当前位置 44 {_unReadCount} {_waitReadLength}");
                    _unReadCount -= _waitReadLength;
                    _waitReadLength = 0;
                    // _ms.Seek(_readingContent.Length, SeekOrigin.Begin);
                    // _ms.Position = _ms.Length - (_readingContent.Length + _headLength);
                    // Debug.Log($"当前位置 55 {_ms.Position} {_unReadCount}");
                    lock (_lock)
                    {
                        _contents.Enqueue(_readingContent);
                    }

                    // Debug.Log($"读取完成，读下一个 {_contents.Count} 当前数据量{_ms.Length}");
                    _readingContent = null;
                }
                // 已读完当前可读部分
                // Debug.Log($"当前可读部分已读完 {_readingContent == null} {_ms.Position} {_ms.Length} {_waitReadLength} {_unReadCount}");
                // 读取完成，移动到最前面重新写，写入长度为剩余未读的写入长度了
                _ms.Seek(_unReadCount, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                Debug.Log($"出现错误 {e}");
            }
        }

        private bool ContinueReading()
        {
            if (_readingContent == null && _unReadCount < _headLength)
            {
                return false;
            }
            
            if (_readingContent != null && _unReadCount < _waitReadLength) 
            {
                return false;
            }

            return true;
        }

        public void Connect()
        {
            _client.Connect();
        }

        public void Stop()
        {
            _client.Stop();
        }
    }
}