using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SLibrary.Socket;

/*
 * 需要连接的服务器类型
 */
public enum ServerType
{
    GameServer = 0

    //如需新增服务器类型,务必在对应逻辑中添加相关的特殊逻辑处理.
}


namespace SLibrary.Socket
{
    /**
 * 
 */
    public class AsyncSocket
    {
        // Socket
        private TcpClient mConnector;

        // 连接超时
        private int mTimeout = 5000;

        private object _kLock = new object();

        // 网络流
        private NetworkStream mStream;

        // 接收数据缓冲区
        private byte[] mReadBuffer;

        // 连接成功回调
        private Action mConnectCallback;

        // 连接断开回调
        private Action mDisconnectCallback;

        // 接收数据回调
        private Action<byte[], int> mReceiveCallback;

        // 连接端口
        private int mConnectPort;

        // 连接ip
        private string mConnectIP;

        private ServerType mType = 0;

        public AsyncSocket(string ip, int port, ServerType type = ServerType.GameServer)
        {
            mType = type;
            mConnectIP = ip;
            mConnectPort = port;

            //new TcpClient(SocketHelper.AddressFamily);
            mConnector = SocketHelper.createTcpClient(ref mConnectIP, port.ToString());
            // 禁用延迟
            mConnector.NoDelay = true;
            // 获取或设置接收缓冲区的大小
            mConnector.ReceiveBufferSize = 20 * 1024;
            // 获取或设置发送缓冲区的大小
            mConnector.SendBufferSize = 20 * 1024;
            mReadBuffer = new byte[20 * 1024];
        }

        /// <summary>
        /// 设置连接回调
        /// </summary>
        /// <param name="connectCallback"></param>
        public void SetConnectCallback(Action connectCallback)
        {
            mConnectCallback = connectCallback;
        }

        /// <summary>
        /// 设置断线回调
        /// </summary>
        /// <param name="disconnectCallback"></param>
        public void SetDisconnectCallback(Action disconnectCallback)
        {
            mDisconnectCallback = disconnectCallback;
        }


        public void SetReceiveCallback(Action<byte[], int> onReceive)
        {
            mReceiveCallback = onReceive;
        }

        /// <summary>
        /// 判断是否已经连接
        /// </summary>
        /// <returns></returns>
        public bool isConnect()
        {
            return !((mConnector.Client.Poll(1000, SelectMode.SelectRead) && (mConnector.Client.Available == 0)) ||
                     !mConnector.Client.Connected);
        }

        /// <summary>
        /// 返回连接的服务器IP
        /// </summary>
        /// <returns></returns>
        public string getRemoreAddress()
        {
            if (isConnect())
            {
                IPEndPoint endPoint = mConnector.Client.RemoteEndPoint as IPEndPoint;
                if (endPoint != null)
                {
                    return endPoint.Address.ToString();
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// 连接网络
        /// </summary>
        public void connect()
        {
            //Thread thread = Thread.CurrentThread;

            //Debuger.LogFormat("Behaviour socket connect:" + Thread.CurrentThread.ManagedThreadId.ToString());

            if (isConnect())
            {
                //Logger.log2File("已经连接服务器: " + getRemoreAddress(), Thread.CurrentThread.ManagedThreadId);
                return;
            }

            try
            {
                //Logger.log2File("开始尝试连接服务器: " + getRemoreAddress(), Thread.CurrentThread.ManagedThreadId);
                // 异步 从线程池中开启线程创建连接
                mConnector.BeginConnect(mConnectIP.Trim(), mConnectPort, AsyncConnectCallback, mConnector);
            }
            catch (Exception ex)
            {
                NetworkManager.InnerMsg(EventId.Net_ConnectFailed,
                    "无法连接服务器:[" + mConnectIP.Trim() + "][" + mConnectPort + "], error: " + ex);
            }
        }

        /// <summary>
        /// 异步接收数据调用
        /// </summary>
        /// <param name="ar">返回数据信息</param>
        private void asyncReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesToRead = mStream.EndRead(ar);
                if (bytesToRead != 0)
                {
                    // 抛出数据
                    HandleBuffer(mReadBuffer, bytesToRead);

                    lock (_kLock)
                    {
                        if (mStream.CanRead)
                        {
                            mStream.BeginRead(mReadBuffer, 0, mReadBuffer.Length, asyncReceiveCallback,
                                (TcpClient) ar.AsyncState);
                        }
                        else
                        {
                            NetworkManager.InnerMsg(EventId.Net_Error, "当前流不可读！");
                        }
                    }
                }
                else
                {
                    Close();
                }
            }
            catch (IOException ioException)
            {
                NetworkManager.InnerMsg(EventId.Net_Error, "接收数据失败: " + ioException);

                Close();
            }
            catch (Exception ex)
            {
                NetworkManager.InnerMsg(EventId.Net_Error, "接收数据失败: " + ex);
            }
        }

        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        /// <param name="readBuffer"></param>
        /// <param name="bytesToRead"></param>
        private void HandleBuffer(byte[] readBuffer, int bytesToRead)
        {
            if (bytesToRead > 0 && mReceiveCallback != null)
            {
                mReceiveCallback(readBuffer, bytesToRead);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close(bool callback = true)
        {
            if (isConnect())
            {
                mConnector.GetStream().Close();
                mConnector.Close();
            }

            // 关闭后调用
            if (mDisconnectCallback != null && callback)
            {
                mDisconnectCallback();
            }
        }

        /// <summary>
        /// 异步连接回调
        /// </summary>
        /// <param name="ar">返回链接信息</param>
        private void AsyncConnectCallback(IAsyncResult ar)
        {
            try
            {
                //Thread thread = Thread.CurrentThread;
                //Debuger.LogFormat("Thread AsyncConnectCallback : " + Thread.CurrentThread.ManagedThreadId.ToString());

                TcpClient client = (TcpClient) ar.AsyncState;
                client.EndConnect(ar);

                // 判断连接是否成功
                if (isConnect())
                {
                    lock (_kLock)
                    {
                        if (mConnectCallback != null)
                        {
                            mConnectCallback();
                        }

                        try
                        {
                            // 连接成功开始接收数据
                            mStream = mConnector.GetStream();
                        }
                        catch (Exception getStreamException)
                        {
                            NetworkManager.InnerMsg(EventId.Net_ConnectFailed, "无法获取Stream:" + getStreamException);

                            return;
                        }

                        try
                        {
                            mStream.BeginRead(mReadBuffer, 0, mReadBuffer.Length, asyncReceiveCallback, mConnector);
                        }
                        catch (Exception beginReadException)
                        {
                            NetworkManager.InnerMsg(EventId.Net_ConnectFailed, "异步读取失败: " + beginReadException);
                        }
                    }
                }
                else
                {
                    // 抛出消息触发重连机制
                    NetworkManager.InnerMsg(EventId.Net_ConnectFailed, "无法连接服务器!");
                }
            }
            catch (Exception ex)
            {
                NetworkManager.InnerMsg(EventId.Net_ConnectFailed, "连接服务器异常: " + ex);
            }
        }

        public void Send(byte[] buffer, int size)
        {
            try
            {
                if (isConnect())
                {
                    lock (_kLock)
                    {
                        mStream = mConnector.GetStream();
                        mStream.Write(buffer, 0, buffer.Length);
                    }
                }
                else
                {
                    NetworkManager.InnerMsg(EventId.Net_ConnectFailed, "连接已经断开!");
                }
            }
            catch (Exception ex)
            {
                NetworkManager.InnerMsg(EventId.Net_Error, "发送异常：" + ex);
            }
        }

        private void AsyncSendCallback(IAsyncResult ar)
        {
            try
            {
                lock (_kLock)
                {
                    mStream.EndWrite(ar);
                }
            }
            catch (Exception ex)
            {
                //Logger.log("数据发送异常: " + ex.ToString());
#if UNITY
                UnityEngine.Debug.Log("数据发送异常: " + ex);
#endif
                Close();
            }
        }
    }
}