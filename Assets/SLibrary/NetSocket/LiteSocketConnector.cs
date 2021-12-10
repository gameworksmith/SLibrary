using System;
using SLibrary.NetSocket.Message;
using SLibrary.NetSocket.Others;
using UnityEngine;

namespace SLibrary.NetSocket
{
    public class LiteSocketConnector
    {
        public LiteSocketConnector()
        {
            mServerIP = String.Empty;
            mPort = 0;
            setConnectToNotStart();
            firstConnect = true;
        }

        public void ConnectToServer(string serverIP, int port)
        {
            AddListener();
            mPort = port;
            mServerIP = serverIP;
            doConnect = true;
        }
        

        public void Update(float deltaTime)
        {
            //修改，不再处理断线重连，在lua端处理
            if (doConnect)
            {
                mTryConnectState = TryConnectState.Connecting;
                NetworkManager.Instance.Connect(mServerIP, mPort);
                doConnect = false;
                // Debug.Log("登陆游戏连接网络");
            }
            else
            {
                //TODO 另开线程,发送逻辑不适合在主线程
                //SendHeartBeat();//心跳在lua端处理了
            }
        }

        public static float HeartBeatInterval = 60f; //每1分钟发一次心跳就对了

        private long lastSendunixTime = 0;


        private string mServerIP;
        private int mPort;
        private bool mConnected;
        private int mCurrentNum;

        private bool doConnect;
        private bool firstConnect;
        private float mConnectTime;
        private TryConnectState mTryConnectState = TryConnectState.NotStart;

        public TryConnectState ConnectState
        {
            get { return mTryConnectState; }
        }


        private void quitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
      
		 UnityEngine.Application.Quit();
#endif
        }

        public bool IsConnected()
        {
            return mTryConnectState == TryConnectState.Connected;
        }

        /// <summary>
        /// 是否正在重连
        /// </summary>
        private bool IsConnecting
        {
            get { return mTryConnectState == TryConnectState.Connecting; }
        }

        private void disconnectCallback(RMessage rMsg)
        {
            mTryConnectState = TryConnectState.Disconnected;
            UnityEngine.Debug.Log($"与服务器[" + mServerIP + "]的连接已经断开.");
            doConnect = true;
        }

        public void setConnectToNotStart()
        {
            mTryConnectState = TryConnectState.NotStart;
        }

        public void stop()
        {
            RemoveListener();
            doConnect = false;
            firstConnect = true;
            setConnectToNotStart();
            if (NetworkManager.Instance.IsConnected())
                NetworkManager.Instance.Close();
        }

        private void connectCallback(RMessage rMsg)
        {
            Debug.Log("连接服务器[" + mServerIP + ":" + mPort + "]");

            //TODO 登陆状态下连接服务器，请求登陆 等各种状态处理

            firstConnect = false;
            mTryConnectState = TryConnectState.Connected;
            doConnect = false;

            //TODO 通过配置启动后自动注册
            // CrossThreadQueue.Singleton.AddHandlerOnThread((int)ResCPong.MsgID.eMsgID, OnReceiveHeartBeat);
            // CrossThreadQueue.Singleton.AddHandlerOnThread((int)ResSPing.MsgID.eMsgID, OnReceiveHeartBeatServer);
            //CrossThreadQueue.Singleton.AddHandler((int)ResBigData.MsgID.eMsgID, OnReceiveBigData);
        }


        private void AddListener()
        {
            CrossThreadQueue.Singleton.AddHandler((int) EventId.Net_ConnectFailed, OnConnectFailed);
            CrossThreadQueue.Singleton.AddHandler((int) EventId.Net_Error, OnNetError);
            CrossThreadQueue.Singleton.AddHandler((int) EventId.Net_Connected, connectCallback);
            CrossThreadQueue.Singleton.AddHandler((int) EventId.Net_Disconnect, disconnectCallback);
        }

        private void RemoveListener()
        {
            CrossThreadQueue.Singleton.RemoveHandler((int) EventId.Net_ConnectFailed, OnConnectFailed);
            CrossThreadQueue.Singleton.RemoveHandler((int) EventId.Net_Error, OnNetError);
            CrossThreadQueue.Singleton.RemoveHandler((int) EventId.Net_Connected, connectCallback);
            CrossThreadQueue.Singleton.RemoveHandler((int) EventId.Net_Disconnect, disconnectCallback);
        }

        private void OnConnectFailed(RMessage rMsg)
        {
            //mConnectTime -= AUTO_RECONNECT_FAILD_JUMP;
            mTryConnectState = TryConnectState.ConnectFailed;
            Debug.Log(Tools.bytesToString(rMsg.Content));
            doConnect = true;
        }

        private void OnNetError(RMessage rMsg)
        {
            //mConnectTime -= AUTO_RECONNECT_FAILD_JUMP;
            mTryConnectState = TryConnectState.ConnectFailed;
            Debug.Log(Tools.bytesToString(rMsg.Content));
            // doConnect = true;
        }
    }
}