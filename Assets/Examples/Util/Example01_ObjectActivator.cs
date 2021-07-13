using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example01_ObjectActivator : MonoBehaviour
{
    public Text NormalTimer;
    public Text IgnoreTimer;

    public Button PauseResumeButton;
    // Start is called before the first frame update

    private float normalTime = 0;
    private float ignoreTime = 0;

    private void Awake()
    {
        PauseResumeButton.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        Time.timeScale = 1 - Time.timeScale;
        PauseResumeButton.GetComponentInChildren<Text>().text = Time.timeScale < 1 ? "Resume" : "Pause";
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        normalTime += Time.deltaTime;
        ignoreTime += Time.unscaledDeltaTime;
        if (NormalTimer != null)
        {
            NormalTimer.text = $"normal: {normalTime}";
        }

        if (IgnoreTimer != null)
        {
            IgnoreTimer.text = $"real: {ignoreTime}";
        }
    }
}
