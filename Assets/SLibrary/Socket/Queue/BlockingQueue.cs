using System;
using System.Collections.Generic;

namespace SLibrary.Socket.Queue
{
    /**
 * 同步阻塞队列
 */
    public class BlockingQueue<TVal> where TVal : class
    {
        private object mSyncObj;
        private List<TVal> mQueue;

        public BlockingQueue()
        {
            mSyncObj = new object();
            mQueue = new List<TVal>();
        }

        public void add(TVal val)
        {
            lock (mSyncObj)
            {
                mQueue.Add(val);
            }
        }

        public TVal get()
        {
            lock (mSyncObj)
            {
                TVal ret = null;
                if (mQueue.Count > 0)
                {
                    ret = mQueue[0];
                    mQueue.RemoveAt(0);
                }

                return ret;
            }
        }

        public void clear()
        {
            lock (mSyncObj)
            {
                mQueue.Clear();
            }
        }

        public void runAction(int len, Action<TVal> action)
        {
            lock (mSyncObj)
            {
                int cur = Math.Min(len, mQueue.Count);
                for (int a = 0; a < cur; ++a)
                {
                    TVal val = get();
                    if (val != null)
                    {
                        action(val);
                    }
                }
            }
        }

        public bool empty()
        {
            lock (mSyncObj)
            {
                return mQueue.Count <= 0;
            }
        }

        public int size()
        {
            lock (mSyncObj)
            {
                return mQueue.Count;
            }
        }
    }
}