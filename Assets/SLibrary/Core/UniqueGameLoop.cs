using System;
using System.Collections.Generic;
using SLibrary.Interfaces;
using UnityEngine;

namespace SLibrary.Core {

    public class UniqueGameLoop:  IGameLoop {

        private static readonly Dictionary<System.Action, int> _listeners = new Dictionary<Action, int>();

        private static readonly Dictionary<System.Action, List<System.Action>> _removeList = new Dictionary<Action, List<Action>>();
        private static readonly Queue<System.Action> _executeQueue = new Queue<Action>();
        private static int _index = 0;//队列第几次执行
        public const int MAX_UPDATE_COUNT = 20;//最多执行20次update，如果超过这个数，在下一帧执行
        private const float MAX_ALLOC_TIME = 0.03f;
        private static List<System.Action> _lateUpdateListeners = new List<Action>();

        public void AddListener(System.Action func, int frequence)
        {
            if (!_listeners.ContainsKey(func))
            {
                _listeners.Add(func, frequence);
            }
            else
            {
                _listeners[func] = frequence;
            }
        }

        public void RemoveListener(System.Action func)
        {
            _listeners.Remove(func);
        }

        public void AddLateListener(Action action)
        {
            _lateUpdateListeners.Add(action);
        }

        public void RemoveLateListener(Action action)
        {
            _lateUpdateListeners.Remove(action);
        }


        public static void RemoveAll()
        {
            _listeners.Clear();
            _removeList.Clear();
        }


        public void Update() {
            ExecuteQueueUpdateFunctions();
        }

        public void LateUpdate()
        {
            for (int i = _lateUpdateListeners.Count - 1; i >= 0; i--)
            {
                _lateUpdateListeners[i]?.Invoke();
            }
        }



        public void Invoke(System.Action func, float delay, bool ignoreTimeIgnore = false)
        {
            if (func == null)
            {
                Debug.LogErrorFormat("调用了空方法");
                return;
            }
            float time = ignoreTimeIgnore ? Time.realtimeSinceStartup : Time.time;
            System.Action temp = null;
            System.Action _func = func;
            temp = () => {
                float currentTime = ignoreTimeIgnore ? Time.realtimeSinceStartup : Time.time;
                if (currentTime - time >= delay)
                {
                    if (func != null)
                    {
                        func();
                    }
                    func = null;

                    RemoveListener(temp);
                }
                if (_removeList.ContainsKey(_func))
                {
                    _removeList[_func].Remove(temp);
                }
                func = null;
            };
            AddListener(temp, 1);
            if (!_removeList.ContainsKey(func))
            {
                _removeList.Add(func, new List<Action>());
            }
            _removeList[func].Add(temp);
        }

        public void CancelInvoke(System.Action func)
        {
            if (!_removeList.ContainsKey(func))
            {
                return;
            }
            for (int i = 0; i < _removeList[func].Count; i++)
            {
                RemoveListener(_removeList[func][i]);
            }
            _removeList[func].Clear();
        }

        private void ExecuteQueueUpdateFunctions()
        {
            if (_executeQueue.Count == 0)
            {//队列已经执行完毕，重新生成队列
                foreach (var funcPair in _listeners)
                {
                    var frequence = funcPair.Value <= 0 ? 1 : funcPair.Value;
                    if (_index % frequence == 0)
                    {
                        _executeQueue.Enqueue(funcPair.Key);
                    }
                }
                _index++;
            }
            int allCount = _executeQueue.Count;
            float startTime = Time.realtimeSinceStartup;
            float taskTime = startTime;
            for (int i = 0; i < allCount; i++)
            {
                var func = _executeQueue.Dequeue();
                func?.Invoke();
                func = null;
                float now = Time.realtimeSinceStartup;
                if (now - startTime > MAX_ALLOC_TIME)
                {
                    break;
                }
            }
            float endTime = Time.realtimeSinceStartup;
            if (endTime - startTime > MAX_ALLOC_TIME)
            {
                //Debuger.LogFormat("c#本帧超过阈值，耗时{0}", endTime - startTime, LogLevel.Error);
            }
        }


    }
}