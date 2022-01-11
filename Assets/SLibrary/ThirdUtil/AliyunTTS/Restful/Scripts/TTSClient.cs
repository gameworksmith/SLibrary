using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TTSClient
{
    
    private String _accessToken;
    private String _appkey;
    private System.Action<AudioClip> _onGetClip;
    private System.Action<string> _onGetFailed;
    
    public static string[] defaultVoices = new string[]
    {
        "xiaoyun",//标准女声
        "xiaogang",//标准男声
        "ruoxi",//温柔女声
        "siqi",//温柔女声
        "sijia",//标准女声
        "sicheng",//标准男声
        "aiqi",//温柔女声
        "aijia",//标准女声
        "aicheng",//标准男声
        "aida",//标准男声
        "ninger",//标准女声
        "ruilin",//标准女声
        "siyue",//温柔女声
        "aiya",//严厉女声
        "aixia",//亲和女声
        "aimei",//甜美女声
        "aiyu",//自然女声
        "aiyue",//温柔女声

        "aijing",//严厉女声
        "xiaomei",//甜美女声
        "aina",//浙普女声
        
        "yina",//浙普女声
        "sijing",//严厉女声
        "sitong",//儿童音
        "xiaobei",//萝莉女声
        "aitong",//儿童音

        "aiwei",//萝莉女声
        "aibao",//萝莉女声
        "harry",//英音男声

        "abby",//美音女声


        "andy",//美音男声
        "eric",//英音男声
        "emily",//英音女声
        "luna",//英音女声
        "luca",//英音男声
        "wendy",//英音女声
        "william",//英音男声
        "olivia",//英音女声
        "shanshan",//粤语女声
        "chuangirl",//四川话女声
        "lydia",//英中双语女声
        "aishuo",//自然男声
        "qingqing",//中国台湾话女声
        "cuijie",//东北话女声
        "xiaoze",//湖南重口音男声

        
        "tomoka",//日语女声
        "tomoya",//日语男声
        "annie",//美语女声


        "jiajia",//粤语女声
        "indah",//印尼语女声
        "taozi",//粤语女声
        "guijie",//亲切女声


        "stella",//知性女声
                   

        "stanley",//沉稳男声


        "kenny",//沉稳男声
        "rosa",//自然女声
        "farah",////马来语女声
        "mashu",////儿童剧男声
        "xiaoxian",////亲切女声


        "yueer",////儿童剧女声


        "maoxiaomei",//活力女声


        "aifei",//激昂解说


        "yaqun",//卖场广播


        "qiaowei",//卖场广播


        "dahu",//东北话男声


        "ava",//美语女声


        "ailun",// 悬疑解说


        "jielidou",//治愈童声


        "laotie",//东北老铁

        "laomei",//吆喝女声
        
        "aikan",//天津话男声
        "tala",// 菲律宾语女声
        "annie_saodubi",// 美式英文女声
        "zhitian",//精品版 
        "zhiqing",//精品版 中国台湾话女生 方言场景
    };
    
    public static string SERVER_URL =  "https://nls-gateway.cn-shanghai.aliyuncs.com/stream/v1/tts";

    public static TTSClient Create(string appKey, string accessToken,  System.Action<AudioClip> onGetClip = null,
        System.Action<string> onGetFailed = null)
    {
        TTSClient client = new TTSClient();
        client._accessToken = accessToken;
        client._appkey = appKey;
        client._onGetClip = onGetClip;
        client._onGetFailed = onGetFailed;
        return client;
    }

    // voice 发音人，可选，默认是xiaoyun。
    // url = url + "&voice=" + "xiaoyun";
    // volume 音量，范围是0~100，可选，默认50。
    // url = url + "&volume=" + String.valueOf(50);
    // speech_rate 语速，范围是-500~500，可选，默认是0。
    // url = url + "&speech_rate=" + String.valueOf(0);
    // pitch_rate 语调，范围是-500~500，可选，默认是0。
    // url = url + "&pitch_rate=" + String.valueOf(0);
    public void GetTTSAudio(
        string ttsContent,
        string format,
        string voice = "xiaoyun",
        
        System.Action<AudioClip> onClipGetSuccess = null, 
        System.Action<string> onClipGetFailed = null,
        int sampleRate = 16000,
        int volume = 50,
        int speechRate = 0,
        int pitchRate = 0,

        int timeout = 10)
    {
        string url = $"{SERVER_URL}?" +
                     $"appkey={_appkey}" +
                     $"&token={_accessToken}" +
                     $"&text={ttsContent}&" +
                     $"format={format}" +
                     $"&voice={voice}" +
                     $"&sample_rate={sampleRate}" +
                     $"&volume={volume}" +
                     $"&speed_rate={speechRate}" +
                     $"&pitch_rate={pitchRate}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        // 如果不是wav的话就用mp3格式
        AudioType audioType = format == "wav" ? AudioType.WAV : AudioType.MPEG;
        request.downloadHandler = new DownloadHandlerAudioClip(url, audioType);
        request.timeout = timeout;
        request.SendWebRequest().completed += operation =>
        {
            OnReceive(request, onClipGetSuccess, onClipGetFailed);
        };

    }

    public void GetTTSAudioPersonal(string ttsContent,
        string nickName,
        string format,
        int sampleRate = 16000,
        System.Action<AudioClip> onClipGetSuccess = null, 
        System.Action<string> onClipGetFailed = null,
        int volume = 50,
        int speechRate = 0,
        int pitchRate = 0,

        int timeout = 10)
    {

        string voice = $"pt_{_appkey}_{nickName}";
        GetTTSAudio(ttsContent, format, voice, onClipGetSuccess, onClipGetFailed, sampleRate, volume, speechRate, pitchRate, timeout);
    }

    private void OnReceive(UnityWebRequest request, Action<AudioClip> onClipGetSuccess, Action<string> onGetFailed)
    {
        System.Action<string> failed = onGetFailed ?? _onGetFailed;
        if (!string.IsNullOrEmpty(request.error))
        {
            // Debug.Log($"{request.responseCode}");
            var datas = request.downloadHandler.data;
            var text = System.Text.Encoding.Default.GetString(datas);
            failed?.Invoke($"请求失败{request.url}\n{request.error}\n{text}");
            return;
        }


        String contentType = request.GetResponseHeader("Content-Type");
        if (contentType == "audio/mpeg")
        {
            var clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null || Math.Abs(clip.length) < 0.1f)
            {
                // Debug.LogError("获取音乐失败，播放下一首");
                failed?.Invoke($"获取音乐{request.url}失败，字节长度为0");
            }
            else
            {
                System.Action<AudioClip> onGet = onClipGetSuccess ?? _onGetClip;
                onGet?.Invoke(clip);
            }
        }
        else
        {
            failed?.Invoke($"获取音乐{request.url}失败, 错误{request.downloadHandler.text}");
            
        }
    }
}
