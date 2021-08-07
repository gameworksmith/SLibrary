using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using UnityEngine;

public class AliyunOSSUploader : MonoBehaviour
{
    [Header("存储节点")]
    public string endPoint;
    [Header("站点")]
    public string host;
    [Header("oss的key")]
    public string key;
    [Header("oss的secret")]
    public string secret;
    // Start is called before the first frame update

    public static AliyunOSSUploader Instance;

    private void Awake()
    {
        Instance = this;
    }

    public BucketInfo GetBucketInfo(string bucketName)
    {
        OssClient client = new OssClient(endPoint, key, secret);
        return client.GetBucketInfo(bucketName);
    }

    public void Upload(byte[] data, string bucketName, string root, string fileName, System.Action<string> onComplete)
    {
        
        OssClient client = new OssClient(endPoint, key, secret);
        
        try {
            
            using (Stream stream = new MemoryStream(data))
            {
                string saveKey = root + "/" + fileName;
                var result = client.PutObject(bucketName, saveKey, stream);
                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    string url = host + "/" + root + "/" + fileName;
                    onComplete?.Invoke(url);
                    Debug.Log("上传成功");
                }
                else
                {
                    Debug.Log("上传失败");
                    onComplete?.Invoke(null);                   
                }
            }
        }
        catch (OssException e) {
            Debug.Log("字符串数据上传错误："+ e);
        }
        catch (Exception e)
        {
            Debug.Log("字符串数据上传错误：" + e);
        }

    }

}
