/*
 * 辅助创建兼容Ipv6的Socket(Unity4.X版本.5.X有更好的实现方法)
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public class SocketHelper
{
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost, string mPort);
#endif

    //"192.168.1.1&&ipv4"
    public static string GetIPv6(string mHost, string mPort)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }

    private static void getIPType(String serverIp, String serverPorts, out String newServerIp, out AddressFamily ipType)
    {
        // IP 版本 4 地址
        ipType = AddressFamily.InterNetwork;
        newServerIp = serverIp;
        try
        {
            string mIPv6 = GetIPv6(serverIp, serverPorts);
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                        newServerIp = m_StrTemp[0];
                        ipType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        }
        catch (Exception e)
        {
            //Logger.err("GetIPv6 error:" + e);

#if UNITY
            UnityEngine.Debug.Log("GetIPv6 error:" + e);
#endif
        }
    }

    /**
     * Http(使用 Socket 方式)
     */
    public static Socket createSocket(ref string ip, string port)
    {
        String newServerIp = "";
        AddressFamily newAddressFamily = AddressFamily.InterNetwork;
        getIPType(ip, port, out newServerIp, out newAddressFamily);
        if (!string.IsNullOrEmpty(newServerIp))
        {
            ip = newServerIp;
        }

        var socket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
        return socket;
    }

    /**
     * Socket
     */
    public static TcpClient createTcpClient(ref string ip, string port)
    {
        String newServerIp = "";
        AddressFamily newAddressFamily = AddressFamily.InterNetwork;
        getIPType(ip, port, out newServerIp, out newAddressFamily);
        if (!string.IsNullOrEmpty(newServerIp))
        {
            ip = newServerIp;
        }

        var socket = new TcpClient(newAddressFamily);
        return socket;
    }
}