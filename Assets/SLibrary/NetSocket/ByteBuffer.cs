using System;
using System.Threading;
using SLibrary.NetSocket.Codec;

namespace SLibrary.NetSocket
{
    public class ByteBuffer
    {
        private bool mNetOrder;
        private int mMaxSize;
        private CircularBuffer<Byte> mBuffer;

        public ByteBuffer(int maxSize, bool netOrder)
        {
            mMaxSize = maxSize;
            mBuffer = new CircularBuffer<byte>(maxSize);
            mNetOrder = netOrder;
        }

        public void Clear()
        {
            mBuffer.Clear();
        }


        public int Put(byte[] bytes, int len)
        {
            return mBuffer.Put(bytes, 0, len);
        }

        public int Put(byte[] bytes)
        {
            return mBuffer.Put(bytes);
        }

        public void ResetHead(int size)
        {
            mBuffer.ResetHead(size);
        }


        public int ReadInt32()
        {
            byte[] data = new byte[4];
            mBuffer.Get(data);
            int ret = BitConverter.ToInt32(data, 0);
            if (mNetOrder)
                ret = ret.ToBigEndian();
            return ret;
        }

        public int ReadBytes(byte[] dst)
        {
            if (mBuffer.Size >= dst.Length)
            {
                mBuffer.Get(dst);
                return dst.Length;
            }
            else
            {
                //Logger.log2File("剩余数据不够！", Thread.CurrentThread.ManagedThreadId);
                throw new System.Exception($"剩余数据不够！{Thread.CurrentThread.ManagedThreadId}");
                return 0;
            }
        }

        public int Remaining()
        {
            return mBuffer.Size;
        }

        public int MaxSize()
        {
            return mMaxSize;
        }

        public void CopyFrom(ByteBuffer other)
        {
            if (other.mBuffer.Size <= 0)
                return;

            byte[] bytes = new byte[other.mBuffer.Size];
            other.ReadBytes(bytes);
            mBuffer.Put(bytes);
        }

        public int CanUse()
        {
            return mBuffer.Capacity - mBuffer.Size;
        }
    }
}