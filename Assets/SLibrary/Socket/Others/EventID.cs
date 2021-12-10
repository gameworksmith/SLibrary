using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 前2位为：模块ID，后4位为事件ID
/// </summary>
public enum EventId {

    // 自定义协议

    // 网络连接事件

    /*
     * 连接成功
     */
    Net_Connected = 0,

    /*
     * 连接断开
     */
    Net_Disconnect,

    /*
     * 连接失败
     */
    Net_ConnectFailed,

    /*
     * 连接错误
     */
    Net_Error,
    /*
    * IO错误，应该是服务器主动断开连接
    */
    Net_IOError,

    /*
     * 断线重连
     */
    Net_ReConnect,

    /*
     * 断线重连成功
     */
    Net_ReConnected,

    /*
     * 客户端本地报错
     */
    ClientResError

    // 前后端协议
}

/**
 * 新手引导事件
 */
public enum GuideEventId {
   

}