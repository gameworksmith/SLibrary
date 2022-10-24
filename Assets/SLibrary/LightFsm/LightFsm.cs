using System;
using System.Collections.Generic;

namespace SLibrary.LightFsm
{
    public class LightFsm : IFsm
    {
        public LightFsm(int defaultState = -1, System.Action<float> beforeUpdate = null, System.Action<float> afterUpdate = null)
        {
            CurrentState = defaultState;
            beforeUpdateCallback = beforeUpdate;
            afterUpdateCallback = afterUpdate;
        }

        private readonly Dictionary<int, Tuple<System.Action<int>, System.Action<int>, System.Action<float>>> _actions =
            new Dictionary<int, Tuple<Action<int>, Action<int>, Action<float>>>();

        public int CurrentState { get; private set; }
        private System.Action<float> beforeUpdateCallback;
        private System.Action<float> afterUpdateCallback;

        private List<(int currentState, int nextState, int eventCode)> _transitions =
            new List<(int currentState, int nextState, int eventCode)>();

        public bool AddState(int state, Action<int> onEnter, Action<int> onExit, Action<float> onUpdate)
        {
            if (_actions.ContainsKey(state))
            {
                throw new Exception($"不能重复添加状态{state}行为");
            }

            _actions.Add(state, new Tuple<Action<int>, Action<int>, Action<float>>(onEnter, onExit, onUpdate));
            return true;
        }

        public bool AddTransition(int from, int to, int triggerCode)
        {
            if (!_actions.ContainsKey(from) || !_actions.ContainsKey(to))
            {
                return false;
            }
            _transitions.Add(item:  (from, to, triggerCode));
            return true;
        }

        public bool TriggerEvent(int eventCode)
        {
            foreach (var transition in _transitions)
            {
                if (transition.currentState == CurrentState && transition.eventCode == eventCode)
                {
                    SwitchToState(transition.nextState);
                    return true;
                }
            }

            return false;
        }

        public bool RemoveState(int state)
        {
            if (_actions.ContainsKey(state))
            {
                return false;
            }

            _actions.Remove(state);
            return true;
        }

        /// <summary>
        /// 切换到新状态
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="forceSwitch"></param>
        /// TODO 切换过来的时候可能需要给参数
        /// <returns></returns>
        public bool SwitchToState(int stateId, bool forceSwitch = false)
        {
            bool hasState = _actions.ContainsKey(stateId);
            if (!hasState) return false;

            bool stateChanged = stateId != CurrentState;
            if (!stateChanged)
            {
                if (!forceSwitch) return false;
            }

            if (stateChanged)
            {
                if (_actions.TryGetValue(CurrentState, out var oldActions))
                {
                    oldActions.Item2?.Invoke(stateId);
                }
            }

            var oldStateId = CurrentState;
            var newActions = _actions[stateId];

            CurrentState = stateId;
            newActions.Item1?.Invoke(oldStateId);
            return true;
        }

        /// <summary>
        /// 不要在调用频率非常高的方法上使用委托传参，会造成大量gc
        /// </summary>
        /// <param name="time"></param>
        public void Update(float time)
        {
            if (_actions.TryGetValue(CurrentState, out var actions))
            {
                beforeUpdateCallback?.Invoke(time);
                actions.Item3?.Invoke(time);
                afterUpdateCallback?.Invoke(time);
            }
        }
    }
}