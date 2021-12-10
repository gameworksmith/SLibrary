using System;
using System.IO;

/// <summary>
/// 向网络层发送的消息
/// </summary>
public class SMessage {
    /// <summary>
    /// 消息id
    /// </summary>
    public int MsgId { get; private set; }

    /// <summary>
    /// 数据流
    /// </summary>
    public Byte[] Content { get; private set; }


    /// <summary>
    /// 构造函数，xlua当中不能调用这个方法，因为xlua不能识别overload
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="data"></param>
    private SMessage (int msgId, byte[] data) {
        this.MsgId = msgId;
        this.Content = data;
    }

    public static SMessage Create(int msgId, byte[] data) {
        return new SMessage(msgId, data);
    }

}

