using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SLibrary.Presentation
{
    /// <summary>
    /// 用于触发
    /// </summary>
    public class ObjectClickActivator : MonoBehaviour
    {
        [Header("管理目标")]
        public GameObject[] Targets;

        [Header("是否激活时重播")]
        public bool PlayOnAwake = true;

        [Header("有效按钮")]
        public Button[] EnableButtons;

        [Header("完成后行为")] 
        public UnityEvent OnComplete;

        private int _currentIndex = 0;

        private void Awake()
        {
            foreach (Button enableButton in EnableButtons)
            {
                if (enableButton != null)
                {
                    enableButton.onClick.RemoveAllListeners();
                    enableButton.onClick.AddListener(OnClickNext);
                }
                
            }
        }

        /// <summary>
        /// 点击进入下一步
        /// </summary>
        private void OnClickNext()
        {
            if (_currentIndex >= Targets.Length)
            {
                return;
            }
            Targets[_currentIndex].SetActive(true);
            _currentIndex++;
            if (_currentIndex >= Targets.Length)
            {
                OnComplete?.Invoke();
            }
        }

        private void OnEnable()
        {
            if (PlayOnAwake)
            {
                Show();
            }
        }

        public void Show()
        {
            AllDeactivate();
            _currentIndex = 0;
        }
        
        private void AllDeactivate()
        {
            foreach (GameObject obj in Targets)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

    }
}