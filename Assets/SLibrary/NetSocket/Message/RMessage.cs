namespace SLibrary.NetSocket.Message
{
    /// <summary>
    /// 从网络层收到的消息
    /// </summary>
    public class RMessage
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public int MsgId { get; private set; }

        /// <summary>
        /// 消息数据
        /// </summary>
        public byte[] Content { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="data"></param>
        public RMessage(int msgId, byte[] data)
        {
            this.MsgId = msgId;
            this.Content = data;
        }
    }
}