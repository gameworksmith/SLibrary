using System;
using System.IO;
using SLibrary.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.Network
{
    public class Example02_SockeClient : MonoBehaviour
    {
        public string ip;
        public int port;
        private SocketClient _client;

        public Transform Root;
        public GameObject Template;
        public Button TestButton;
        public Button StopButton;
        private string _content;

        private void Awake()
        {
            _client = SocketClient.CreateClient(ip, port, (s, i) =>
            {
                Debug.Log($"连接成功 {s}:{i}");
                _client.AsyncSend(System.Text.Encoding.UTF8.GetBytes("ok"));
            }, OnDataReceived);
            TestButton.onClick.AddListener(OnClick);
            StopButton.onClick.AddListener(() =>
            {
                _client?.Stop();
            });
        }

        private void OnClick()
        {
            _client.Connect();
        }

        private void OnDataReceived(byte[] obj, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(obj, result, length);
            _content = System.Text.Encoding.UTF8.GetString(result);
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(_content))
            {
                return;
            }
            
            Debug.Log($"字符串{_content}");
            GameObject ins = Instantiate(Template, Root);
            ins.SetActive(true);
            Text text = ins.GetComponentInChildren<Text>(true);
            text.text = _content;
            _content = null;
        }
    }
}