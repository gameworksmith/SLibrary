using System;
using SLibrary.NetSocket.Message;
using SLibrary.NetSocket.Others;
#if UNITY
using UnityEngine;

#endif

namespace SLibrary.NetSocket {
    public enum TryConnectState {
        NotStart,
        Connecting,
        ConnectFailed,
        Disconnected,
        Connected,
    }
    public class ConnectManager 
    {

        private static ConnectManager _instance;

        public static ConnectManager Instance
        {
            get
            {
                return _instance ?? (_instance = new ConnectManager());
            }
        }
        public ConnectManager() {
            mServerIP = String.Empty;
            mPort = 0;
            setConnectToNotStart();
            firstConnect = true;
        }

        public void ConnectToServer(string serverIP, int port) {
            AddListener();
            mPort = port;
            mServerIP = serverIP;
            doConnect = true;
        }

        public void Update(float deltaTime) {
            //修改，不再处理断线重连，在lua端处理
            if (doConnect) {
                mTryConnectState = TryConnectState.Connecting;
                NetworkManager.Instance.Connect(mServerIP, mPort);
                doConnect = false;
                // Debug.Log("登陆游戏连接网络");
            } else {
                //TODO 另开线程,发送逻辑不适合在主线程
                //SendHeartBeat();//心跳在lua端处理了
            }
        }

        public static float HeartBeatInterval = 60f;//每1分钟发一次心跳就对了

        private long lastSendunixTime = 0;


        private string mServerIP;
        private int mPort;
        private bool mConnected;
        private int mCurrentNum;

        private bool doConnect;
        private bool firstConnect;
        private float mConnectTime;
        private TryConnectState mTryConnectState = TryConnectState.NotStart;

        public TryConnectState ConnectState {
            get { return mTryConnectState; }
        }


        private void quitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		 UnityEngine.Application.Quit();
#endif
        }

        public bool IsConnected() {
            return mTryConnectState == TryConnectState.Connected;
        }

        /// <summary>
        /// 是否正在重连
        /// </summary>
        private bool IsConnecting {
            get {
                return mTryConnectState == TryConnectState.Connecting;
            }
        }

        private void disconnectCallback(RMessage rMsg) {
            mTryConnectState = TryConnectState.Disconnected;
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"与服务器[" + mServerIP + "]的连接已经断开.");
#endif
        }

        public void setConnectToNotStart() {
            mTryConnectState = TryConnectState.NotStart;
        }

        public void stop() {
            RemoveListener();
            doConnect = false;
            firstConnect = true;
            setConnectToNotStart();
            if (NetworkManager.Instance.IsConnected())
                NetworkManager.Instance.Close();
        }

        private void connectCallback(RMessage rMsg) {
#if UNITY
            Debug.Log("连接服务器[" + mServerIP + ":" + mPort + "]");
#endif

            //TODO 登陆状态下连接服务器，请求登陆 等各种状态处理

            firstConnect = false;
            mTryConnectState = TryConnectState.Connected;
            doConnect = false;

            //TODO 通过配置启动后自动注册
            // CrossThreadQueue.Singleton.AddHandlerOnThread((int)ResCPong.MsgID.eMsgID, OnReceiveHeartBeat);
            // CrossThreadQueue.Singleton.AddHandlerOnThread((int)ResSPing.MsgID.eMsgID, OnReceiveHeartBeatServer);
            //CrossThreadQueue.Singleton.AddHandler((int)ResBigData.MsgID.eMsgID, OnReceiveBigData);
        }


        private void AddListener() {
            CrossThreadQueue.Singleton.AddHandler((int)EventId.Net_ConnectFailed, OnConnectFailed);
            CrossThreadQueue.Singleton.AddHandler((int)EventId.Net_Error, OnNetError);
            CrossThreadQueue.Singleton.AddHandler((int)EventId.Net_Connected, connectCallback);
            CrossThreadQueue.Singleton.AddHandler((int)EventId.Net_Disconnect, disconnectCallback);
        }

        private void RemoveListener() {
            CrossThreadQueue.Singleton.RemoveHandler((int)EventId.Net_ConnectFailed, OnConnectFailed);
            CrossThreadQueue.Singleton.RemoveHandler((int)EventId.Net_Error, OnNetError);
            CrossThreadQueue.Singleton.RemoveHandler((int)EventId.Net_Connected, connectCallback);
            CrossThreadQueue.Singleton.RemoveHandler((int)EventId.Net_Disconnect, disconnectCallback);
        }

        private void OnConnectFailed(RMessage rMsg) {
            //mConnectTime -= AUTO_RECONNECT_FAILD_JUMP;
            mTryConnectState = TryConnectState.ConnectFailed;
#if UNITY
            Debug.Log(Tools.bytesToString(rMsg.Content));
#endif
        }

        private void OnNetError(RMessage rMsg) {
            //mConnectTime -= AUTO_RECONNECT_FAILD_JUMP;
            mTryConnectState = TryConnectState.ConnectFailed;
#if UNITY
            Debug.Log(Tools.bytesToString(rMsg.Content));
#endif
        }

        /// <summary>
        /// 收到客户端的主动心跳的服务端回执
        /// </summary>
        /// <param name="rmsg"></param>
        // private void OnReceiveHeartBeat(RMessage rmsg) {
        //     //Thread thread = Thread.CurrentThread;
        //     //Debuger.LogFormat("OnReceiveHeartBeat, id:" + Thread.CurrentThread.ManagedThreadId.ToString());
        //
        //     ResCPong _serverPong = rmsg.deserialize<ResCPong>();
        //     //Debuger.LogFormat("OnReceiveHeartBeat:{0}", _serverPong.sendTime);
        // }


        // /// <summary>
        // /// 收到服务端心跳
        // /// </summary>
        // /// <param name="rmsg"></param>
        // private void OnReceiveHeartBeatServer(RMessage rmsg) {
        //     Thread thread = Thread.CurrentThread;
        //     //Debuger.LogFormat("Thread OnReceiveHeartBeatServer, id:" + Thread.CurrentThread.ManagedThreadId.ToString());
        //
        //     ResSPing _serverPing = rmsg.deserialize<ResSPing>();
        //        
        //     var heartBeat = new ReqSPong { sendTime = _serverPing.sendTime };
        //     //Debuger.LogFormat("Thread Send heart beat, unix time:{0}", _serverPing.sendTime, LogLevel.None);
        //     SMessage sMsg = new SMessage((int)ReqSPong.MsgID.eMsgID, heartBeat);
        //
        //     NetworkManager.Singleton.Send(sMsg);
        // }

        // /// <summary>
        // /// 客户端主动发送心跳 ReqCPing
        // /// </summary>
        // /// <returns></returns>
        // private void SendHeartBeat() {
        //     bool isConnected = NetworkManager.Singleton.IsConnected();
        //     if (isConnected) {
        //         // TODO 连接成功后需要与服务器同步时间 具体逻辑根据需求的时候再讨论
        //         long unixTime = Utility.ConvertDateTimeToUnixTime(DateTime.Now);
        //         if (unixTime - lastSendunixTime >= HeartBeatInterval) {
        //             var heartBeat = new ReqCPing { sendTime = unixTime };
        //             Debuger.LogFormat("Behaviour Send heart beat, unix time:{0}", unixTime, LogLevel.None);
        //             SMessage sMsg = new SMessage((int)ReqCPing.MsgID.eMsgID, heartBeat);
        //             NetworkManager.Singleton.Send(sMsg);
        //             lastSendunixTime = unixTime;
        //         }
        //     }
        // }
    }
}
