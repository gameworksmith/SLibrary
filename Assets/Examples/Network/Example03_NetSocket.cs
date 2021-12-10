using System;
using System.Collections;
using System.Collections.Generic;
using SLibrary.NetSocket;
using SLibrary.NetSocket.Message;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


[Serializable]
public struct TestDanmu
{
    public string nickname;
    public long time;
    public int size;
    public long color;
    public string msg;
    public override string ToString()
    {
        return $"nickName {nickname}, time {time}, size {size}, color {color}, msg {msg}";
    }
}
public class Example03_NetSocket : MonoBehaviour
{
    public int[] MsgIds;
    public Button TestButton;
    public string ip = "localhost";
    public int port = 8888;


    private LiteSocketConnector _connector;
    private string _receivedString;

    private void Awake()
    {
        TestButton.onClick.AddListener(OnClickTest);
        _connector = new LiteSocketConnector();
    }

    private void OnDestroy()
    {
        _connector.stop();
    }

    private void OnEnable()
    {
        for (int i = 0; i < MsgIds.Length; i++)
        {
            var msgId = MsgIds[i];
            CrossThreadQueue.Singleton.AddHandler(msgId, OnMsgReceived);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < MsgIds.Length; i++)
        {
            var msgId = MsgIds[i];
            CrossThreadQueue.Singleton.RemoveHandler(msgId, OnMsgReceived);
        }

    }

    private void OnMsgReceived(RMessage rmsg)
    {
        int msgId = rmsg.MsgId;
        if (msgId == 1001)
        {
            string danmujson = System.Text.Encoding.UTF8.GetString(rmsg.Content);
            Debug.Log($"收到的信息{danmujson}");
            TestDanmu danmu = JsonUtility.FromJson<TestDanmu>(danmujson);
            Debug.Log($"已解析{danmu.ToString()}");
        }
    }

    private void OnClickTest()
    {
        _connector.ConnectToServer(ip, port);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _connector.Update(Time.deltaTime);
    }
}