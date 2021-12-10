using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using SLibrary.NetSocket.Codec;
using SLibrary.NetSocket.Message;
using SLibrary.NetSocket.Others;
using UnityEngine;

namespace SLibrary.NetSocket {
    public class NetworkManager {
        private static NetworkManager _instance;

        public static NetworkManager Instance
        {
            get
            {
                return _instance ?? (_instance = new NetworkManager());
            }
        }
        private AsyncSocket mSocket = null;

        private static int MAGIC_NUMBER = 888;
        /**
         * 初始mByteBuffer大小
        **/
        private static int RECEIVE_MAX_SIZE = 1024 * 30;
        private ByteBuffer mByteBuffer = new ByteBuffer(RECEIVE_MAX_SIZE, true);
        private static byte[] buffer = new byte[4096];
        //
        // public BlockingQueue<LuaPbCache> PbByteQueue = new BlockingQueue<LuaPbCache>();
        //
        // private List<LuaPbCache> _pbBytePool = new List<LuaPbCache>();
        private object _kLock = new object();

        // public LuaPbCache DequeuePbByte() {
        //     if (PbByteQueue.size() == 0) {
        //         return null;
        //     }
        //     var ret = PbByteQueue.get();
        //     int maxOutPutLength = 128;
        //     var data = ret.Data;//ret的Busy设置为false是在lua层设置的
        //     // int outPutLength = Mathf.Min(maxOutPutLength, data.Bytes.Length);
        //     // StringBuilder sb = new StringBuilder();
        //     // // if () {
        //     //     for (int i = 0; i < outPutLength; i++) {
        //     //         sb.Append(data.Bytes[i]);
        //     //         sb.Append(" ");
        //     //
        //     //     }
        //     //
        //     //     LogManager.Log($"pb byte数组出队列，id{data.Id} 总长度{data.Bytes.Length} 内容：{sb}");
        //     // // }
        //     //
        //
        //     return ret;
        // }
        //
        // public int PbQueueCount() {
        //     return PbByteQueue.size();
        // }

        // 包头
        [StructLayout(LayoutKind.Auto)]
        private struct PackageHead {
            public int Length;
            public long Times;
            public int Magic;
            public int MsgId;

            public byte[] toBytes() {
                byte[] ret = new byte[4 + 8 + 4 + 4];

                int start = 0;
                Array.Copy(BitConverter.GetBytes(Length.ToBigEndian()), 0, ret, start, 4);
                start += 4;

                Array.Copy(BitConverter.GetBytes(Times.ToBigEndian()), 0, ret, start, 8);
                start += 8;

                Array.Copy(BitConverter.GetBytes(Magic.ToBigEndian()), 0, ret, start, 4);
                start += 4;

                Array.Copy(BitConverter.GetBytes(MsgId.ToBigEndian()), 0, ret, start, 4);

                return ret;
            }
        }

        /**
         * 伪造消息
         * 内部消息抛
         */
        public static void InnerMsg(EventId eventId, string str) {
            CrossThreadQueue.Singleton.ReceiveMsg((int)eventId, Tools.stringToBytes(str));
        }

        /// <summary>
        /// 连接网络
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="type">连接的服务器类型</param>
        /// <returns></returns>
        public void Connect(string ip, int port, ServerType type = ServerType.GameServer) {
            //Thread thread = Thread.CurrentThread;

            //Debuger.LogFormat("Behaviour Connect: " + Thread.CurrentThread.ManagedThreadId.ToString());

            if (mSocket != null) {
                if (mSocket.isConnect()) {
#if UNITY
                    UnityEngine.Debug.Log("已经连接服务器");
#endif
                    OnConnect();
                    return;
                } else {
                    // 彻底关闭
                    mSocket.Close(false);
                    mSocket = null;
                }
            }

            mSocket = new AsyncSocket(ip, port, type);

            mSocket.SetConnectCallback(OnConnect);
            mSocket.SetDisconnectCallback(OnDisconnect);
            mSocket.SetReceiveCallback(OnReceive);

            mSocket.connect();
        }

        /// <summary>
        /// 网络是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() {
            return mSocket != null ? mSocket.isConnect() : false;
        }

        /**
         * 向网络层发送网络消息
         * 有同步锁
         */
        public void Send(SMessage sMsg) {
            int length = 0;
            if (sMsg.Content != null) {
                length = sMsg.Content.Length;
            }
            // LogManager.Log($"发送消息,消息类型{sMsg.MsgId}，消息内容长度{length}, 内容 {sMsg.Content}，时间{DateTime.Now.Ticks / 10000}");
            if (mSocket != null && mSocket.isConnect()) {
                // 确保发送逻辑与MAGIC_NUMBER的顺序一致性.
                //TODO 如果未来不需要跨线程的消息处理,则取消跨线程取消锁
                lock (mSocket) {
                    PackageHead head = new PackageHead();

                    int headLen = 4 + 8 + 4 + 4;

                    head.Times = TimeUtils.currentMillisecondsOnThread();
                    head.MsgId = sMsg.MsgId;
                    // 支线程调用会产生同步问题
                    //head.Magic = Interlocked.Increment(ref MAGIC_NUMBER);
                    head.Magic = MAGIC_NUMBER++;
                    //lock(mSocket){
                    //    head.Magic = MAGIC_NUMBER++;
                    //}

                    head.Length = sMsg.Content.Length + headLen;

                    //if (head.Length > RECEIVE_MAX_SIZE * 2){
                    //    Debuger.Log("发送数据长度超过限制:" + head.Length, LogLevel.Error);
                    //    return;
                    //}

                    head.Magic ^= (0xFE98 << 8);
                    head.Magic ^= head.Length;

                    byte[] sendBytes = new byte[head.Length];

                    Array.Copy(head.toBytes(), 0, sendBytes, 0, headLen);
                    Array.Copy(sMsg.Content, 0, sendBytes, headLen, sMsg.Content.Length);
                    mSocket.Send(sendBytes, sendBytes.Length);
                }
            } else {
#if UNITY
                
                UnityEngine.Debug.Log($"无网络连接，无法发送消息: {sMsg.MsgId}");
#endif
            }

        }

        /**
         * 消息回调
         */
        private void OnReceive(byte[] bytes, int len) {
            try {
                if (len > 0 && bytes != null) {
                    if (len > mByteBuffer.CanUse()) {
                        // TODO 数据包大小的设置不是很合理,待修改
                        ByteBuffer byteBuffer = new ByteBuffer(mByteBuffer.MaxSize() * 2 + len, true);
                        byteBuffer.CopyFrom(mByteBuffer);
                        mByteBuffer = byteBuffer;
                    }

                    mByteBuffer.Put(bytes, len);

                    do {
                        if (mByteBuffer.Remaining() < 4) {
                            break;
                        }

                        int tmp = mByteBuffer.ReadInt32();
                        Debug.Log($"数据长度 {tmp}");
                        int bufLen = tmp;
                        bool isZip = false;
                        //TODO 关于数据包的大小限制可能存在歧义
                        if (bufLen > mByteBuffer.MaxSize() || bufLen <= 0) {
                            InnerMsg(EventId.Net_Error, "接收数据长度超过限制: " + bufLen + ",MaxSize" + mByteBuffer.MaxSize());
                            break;
                        }

                        if (mByteBuffer.Remaining() < bufLen) {
                            mByteBuffer.ResetHead(4);
                            //Log(EventId.Net_Error, "remaining 长度不够：" + bufLen);
                            break;
                        }

                        int msgId = mByteBuffer.ReadInt32();
                        byte[] content = new byte[bufLen - 4];
                        mByteBuffer.ReadBytes(content);
                        if (isZip) {
                            int contetnLeng = content.Length;

                            byte[] unzipContent = getUnzipContent(content);
                            if (unzipContent != null) {
                                CrossThreadQueue.Singleton.ReceiveMsg(msgId, unzipContent);

                                //var heartBeat = new ReqCPing { sendTime = 1 * 1000 };
                                //Debuger.LogFormat("Behaviour Send heart beat, unix time:{0}", 1, LogLevel.None);
                                //SMessage sMsg = new SMessage((int)ReqCPing.MsgID.eMsgID, heartBeat);
                                //NetworkManager.Singleton.Send(sMsg);
                            }
                        } else {
                            CrossThreadQueue.Singleton.ReceiveMsg(msgId, content);
                        }
                    }
                    while (true);
                }

            } catch (Exception ex) {
                InnerMsg(EventId.Net_Error, ex.ToString());
            }
        }

        /*
         * 解压加密的消息
         */
        private byte[] getUnzipContent(byte[] content) {
            try {
                using (MemoryStream inputStream = new MemoryStream(content)) {
                    using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress)) {
                        using (MemoryStream outputStream = new MemoryStream(buffer)) {
                            gzipStream.CopyTo(outputStream);
                            return outputStream.ToArray();
                        }
                    }
                }
            } catch (Exception ex) {
                InnerMsg(EventId.Net_Error, ex.ToString());
            }
            return null;
        }

        /**
         * 给自己抛了一个断开连接消息
         */
        private void OnDisconnect() {
            //Thread thread = Thread.CurrentThread;
            //Debuger.LogFormat("Connect: " + Thread.CurrentThread.ManagedThreadId);

            CrossThreadQueue.Singleton.ReceiveMsg((int)EventId.Net_Disconnect);
        }

        /**
         * 给自己抛了一个连接成功消息
         */
        private void OnConnect() {
            //Thread thread = Thread.CurrentThread;
            //Debuger.LogFormat("Thread Connect: " + Thread.CurrentThread.ManagedThreadId);

            CrossThreadQueue.Singleton.ReceiveMsg((int)EventId.Net_Connected);
        }

        public void Close() {
#if UNITY
            UnityEngine.Debug.Log("<color=yellow>主动关闭连接</color>");

#endif
            if (mSocket != null) {
                mSocket.Close();
                mSocket = null;
            }
        }
    }
}
