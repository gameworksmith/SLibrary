using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SLibrary.Network
{
    public class HttpHelper
    {
        public const int METHOD_GET = 1;
        public const int METHOD_POST = 2;
        public const int METHOD_PUT = 3;
        public const int METHOD_DELETE = 4;

        public static void SendHttpRequest(string url, int method, string data = null, WWWForm formData = null,
            Tuple<string, string>[] headers = null, System.Action<UnityWebRequest> onComplete = null,
            DownloadHandler handler = null, int timeout = 10)
        {
            UnityWebRequest request = null;
            if (method == METHOD_GET)
            {
                request = UnityWebRequest.Get(url);
            }
            else if (method == METHOD_POST)
            {
                if (formData != null)
                {
                    request = UnityWebRequest.Post(url, formData);
                }
                else
                {
                    request = UnityWebRequest.Post(url, data);
                }
            }
            else if (method == METHOD_PUT)
            {
                request = UnityWebRequest.Put(url, data);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Item1, header.Item2);
                }
            }

            if (request == null)
            {
                onComplete?.Invoke(null);
                return;
            }


            if (handler != null)
            {
                request.downloadHandler = handler;
            }

            request.timeout = timeout;
            request.SendWebRequest().completed += operation => { onComplete?.Invoke(request); };
        }
    }
}