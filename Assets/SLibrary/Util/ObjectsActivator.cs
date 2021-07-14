using System;
using System.Collections;
using UnityEngine;

namespace SLibrary.Util
{
    [Serializable]
    public class TimeObject
    {
        [Header("目标")] public GameObject Target;

        // 多长时间后展示
        [Header("延时显示时间")] public float DelayTime;

        [HideInInspector]
        // 消耗的时间
        public float ElapsedTime;

        //
        [Header("是否忽略TimeScale系数")] public bool ignoreTimeScale;
    }

    /// <summary>
    /// 用时间轴来规定所有组件的激活时间
    /// </summary>
    [DisallowMultipleComponent]
    public class ObjectsActivator : UnityEngine.MonoBehaviour
    {
        [Header("显示物体列表")]
        public TimeObject[] TimeObjects;

        [Header("是否每次设置为true时自动激活")]
        public bool PlayOnAwake = true;

        private void OnEnable()
        {
            if (PlayOnAwake)
            {
                Show();
            }
        }

        private void Awake()
        {
        }

        public void Show()
        {
            AllDeactivate();
            StartCoroutine(ActiveInSequence());
        }

        private void AllDeactivate()
        {
            foreach (TimeObject timeObject in TimeObjects)
            {
                if (timeObject.Target != null)
                {
                    timeObject.Target.SetActive(false);
                    timeObject.ElapsedTime = 0;
                }
            }
        }

        private IEnumerator ActiveInSequence()
        {
            while (true)
            {
                bool allActive = true;
                foreach (TimeObject timeObject in TimeObjects)
                {
                    if (ReferenceEquals(timeObject.Target, null))
                    {
                        continue;
                    }

                    if (timeObject.Target.activeSelf)
                    {
                        continue;
                    }

                    allActive = false;
                    if (timeObject.ElapsedTime <= timeObject.DelayTime)
                    {
                        timeObject.ElapsedTime +=
                            (timeObject.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
                    }
                    else
                    {
                        timeObject.Target.SetActive(true);
                    }
                }

                if (allActive)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}