using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Example_RestfullTTS : MonoBehaviour
{
    [Header("可选语音")]
    public string[] voices;

    [Header("采样率")]
    public int SampleRate = 16000;
    [Header("音量")]
    public int SampleVolume = 50;

    public string AppKey;
    public string AccessToken;
    public string Format = "WAV";

    public string DingZhiAppKey = "3iEXVkheYeLbQQEr";
    public string DingZhiNickName = "13348875406chen";


    public InputField ContentInput;
    public Dropdown SelectVoice;
    public Button SendButton;
    public Button SendDingZhiButton;

    public AudioSource Source;

    private TTSClient _client;
    private void Awake()
    {
        SendButton.onClick.AddListener(OnClickSend);
        SendDingZhiButton.onClick.AddListener(OnClickSendDingZhi);
        _client = TTSClient.Create(AppKey, AccessToken, null, OnGetFailed);
    }

    private void OnClickSendDingZhi()
    {
        _client = TTSClient.Create(DingZhiAppKey, AccessToken, null, OnGetFailed);
        _client.GetTTSAudioPersonal(ContentInput.text, DingZhiNickName, Format, SampleRate,  OnGetClip, null, SampleVolume);
    }

    private void Start()
    {
        var options = new List<Dropdown.OptionData>();
        foreach (string voice in voices)
        {    
            options.Add(new Dropdown.OptionData(voice));
        }

        SelectVoice.AddOptions(options);
    }

    private void OnClickSend()
    {
        _client = TTSClient.Create(DingZhiAppKey, AccessToken, null, OnGetFailed);
        _client.GetTTSAudio(ContentInput.text, Format, voices[SelectVoice.value], OnGetClip, null, SampleRate, SampleVolume);
    }

    private void OnGetClip(AudioClip obj)
    {
        Source.clip = obj;
        Source.Play();
    }

    private void OnGetFailed(string obj)
    {
        Debug.Log(obj);
    }
}
