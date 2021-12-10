using System;
using System.Collections.Generic;
using System.Threading;
using SLibrary.Socket.Queue;


namespace SLibrary.Socket
{
/*
* 跨线程Socket消息处理队列
* 跨线程分发.主线程进行调用.
*/
    public class CrossThreadQueue : BaseBehaviour
    {
        public delegate void ServerReplyHandler(RMessage rMsg);

        /*
         * 单件，多线程不能使用compare(4.x)，因此不能继承单件模板
         */
        public static CrossThreadQueue Singleton { get; private set; }

        public override void Awake()
        {
            Singleton = this;
            mQueue = new BlockingQueue<RMessage>();
            mHandler = new Dictionary<int, ServerReplyHandler>(new IntComparer());
            mHandlerOnThread = new Dictionary<int, ServerReplyHandler>(new IntComparer());
        }

        /*
         * unity 线程
         */
        public override void Update()
        {
            if (mQueue != null && !mQueue.empty())
            {
                // Logger.dbg("当前队列长度:" + mQueue.size());
                mQueue.runAction(mMaxHandledPreUpdate, _MainProcessMsg);
            }
        }

        /**
     * 收到网络回调消息
     * 注意: 特殊消息会伪造一个事件触发
     */
        public void ReceiveMsg(int msgId, byte[] data = null)
        {
            //Thread thread = Thread.CurrentThread;
            //Debuger.LogFormat("Thread receiveMsg:" + Thread.CurrentThread.ManagedThreadId.ToString());
            RMessage rMsg = new RMessage(msgId, data);
            _ReceiveMsg(rMsg);
        }

        /**
     * 注册主线程的消息监听
     */
        public void AddHandler(int msgId, ServerReplyHandler handler)
        {
            if (mHandler.ContainsKey(msgId))
            {
                var enu = mHandler.Values.GetEnumerator();
                while (enu.MoveNext())
                {
                    if (enu.Current != null && enu.Current.Equals(handler))
                        return;
                }

                mHandler[msgId] += handler;
            }
            else
            {
                mHandler.Add(msgId, handler);
            }
        }

        /**
     * 注册支线程的消息监听
     * 慎用支线程逻辑处理
     */
        public void AddHandlerOnThread(int msgId, ServerReplyHandler handler)
        {
            if (mHandlerOnThread.ContainsKey(msgId))
            {
                var enu = mHandlerOnThread.Values.GetEnumerator();
                while (enu.MoveNext())
                {
                    if (enu.Current != null && enu.Current.Equals(handler))
                        return;
                }

                mHandlerOnThread[msgId] += handler;
            }
            else
            {
                mHandlerOnThread.Add(msgId, handler);
            }
        }

        /**
     * 移除主线程的消息监听
     */
        public void RemoveHandler(int msgId, ServerReplyHandler handler)
        {
            if (mHandler.ContainsKey(msgId))
            {
                mHandler[msgId] -= handler;
            }
        }

        /**
     * 移除支线程的消息监听
     */
        public void RemoveHandlerOnThread(int msgId, ServerReplyHandler handler)
        {
            if (mHandlerOnThread.ContainsKey(msgId))
            {
                mHandlerOnThread[msgId] -= handler;
            }
        }

        /// <summary>
        /// 退出清空
        /// </summary>
        protected override void OnDestroy()
        {
            if (mQueue != null)
                mQueue.clear();
            if (mHandler != null)
                mHandler.Clear();
            if (mHandlerOnThread != null)
                mHandlerOnThread.Clear();
        }

        /*
        * 每帧处理的消息个数
        */
        private int mMaxHandledPreUpdate = 100;

        /*
         * 消息队列
         */
        private BlockingQueue<RMessage> mQueue;

        /*
         * 主线程消息字典
         */
        private Dictionary<int, ServerReplyHandler> mHandler;

        /*
         * 支线程消息字典
         */
        private Dictionary<int, ServerReplyHandler> mHandlerOnThread;

        /**
     * 收到消息
     */
        private void _ReceiveMsg(RMessage rMsg)
        {
            mQueue.add(rMsg);

            //var allHandlerInfo = "";
            //foreach (var serverReplyHandler in mHandlerOnThread) {
            //    allHandlerInfo += string.Format("[{0}]", serverReplyHandler.Key);
            //}
            // Debuger.LogFormat("CrossThreadQueue.SendMsg(), MsgId:{0}, all add handler: {1}", rMsg.MsgId, allHandlerInfo, LogLevel.None);

            // 如果该消息有支线程的回调处理.则立即执行
            if (mHandlerOnThread.ContainsKey(rMsg.MsgId))
            {
                _OnHandle(rMsg, mHandlerOnThread);
                //Debuger.LogFormat("mHandlerOnThread, OnHandle Msg: {0}", rMsg.MsgId);
            }
        }

        /**
    * 主线程处理消息
    */
        private void _MainProcessMsg(RMessage rMsg)
        {
            //Debuger.LogFormat("Main ProcessMsg: {0}", rMsg.MsgId);

            // ScreenLocker.checkUnlock(rMsg.MsgId);
            _OnMainProgressMessage(rMsg);
        }

        /**
     * 消息处理
     */
        private void _OnHandle(RMessage rMsg, Dictionary<int, ServerReplyHandler> handles)
        {
            ServerReplyHandler handler = handles[rMsg.MsgId];
            if (handler != null)
            {
                handler.Invoke(rMsg);
#if UNITY
                UnityEngine.Debug.Log($"消息触发:{rMsg.MsgId}");
#endif
            }
            else
            {
#if UNITY
                UnityEngine.Debug.Log($"消息监听已经被移除:{rMsg.MsgId}");
#endif
            }
        }

        private void _OnMainProgressMessage(RMessage rMsg)
        {
            if (mHandler.ContainsKey(rMsg.MsgId))
            {
                _OnHandle(rMsg, mHandler);
            }
            else if (mHandlerOnThread.ContainsKey(rMsg.MsgId))
            {
                // 主线程不处理支线程的注册消息 1
            }
            else
            {
                // TODO 解锁消息未注册
                //Debuger.LogFormat("主线程未处理的消息: " + rMsg.MsgId, LogLevel.Error);
            }
        }
    }
}