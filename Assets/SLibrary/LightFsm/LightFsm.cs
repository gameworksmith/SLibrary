using System;
using System.Collections.Generic;

namespace SLibrary.LightFsm
{
    public interface IFsm
    {
        int CurrentState { get; }
        bool AddState(int state, System.Action<int> onEnter, System.Action<int> onExit, System.Action<float, float> onUpdate);
        bool RemoveState(int state);
    
        void Update(float time);

        bool AddTransition(int from, int to, int triggerCode);
        bool TriggerEvent(int eventCode);
        bool SwitchToState(int stateId, bool forceSwitch);
    }
    
    
    public class LightFsm : IFsm
    {
        public class StateInfo
        {
            public float TimeElapsed;
            public System.Action<int> OnEnter;
            public System.Action<int> OnExit;
            public System.Action<float, float> OnUpdate;

            public StateInfo(Action<int> onEnter, Action<int> onExit, Action<float, float> onUpdate)
            {
                OnEnter = onEnter;
                OnExit = onExit;
                OnUpdate = onUpdate;
            }
        }
        public LightFsm(int defaultState = -1, System.Action<float> beforeUpdate = null, System.Action<float> afterUpdate = null)
        {
            CurrentState = defaultState;
            beforeUpdateCallback = beforeUpdate;
            afterUpdateCallback = afterUpdate;
        }

        private readonly Dictionary<int, StateInfo> _states =
            new Dictionary<int, StateInfo>();

        public int CurrentState { get; private set; }
        private System.Action<float> beforeUpdateCallback;
        private System.Action<float> afterUpdateCallback;

        private List<(int currentState, int nextState, int eventCode)> _transitions =
            new List<(int currentState, int nextState, int eventCode)>();

        public bool AddState(int state, Action<int> onEnter, Action<int> onExit, Action<float, float> onUpdate)
        {
            if (_states.ContainsKey(state))
            {
                throw new Exception($"不能重复添加状态{state}行为");
            }

            _states.Add(state, new StateInfo(onEnter, onExit, onUpdate));
            return true;
        }

        public bool AddTransition(int from, int to, int triggerCode)
        {
            if (!_states.ContainsKey(from) || !_states.ContainsKey(to))
            {
                return false;
            }
            _transitions.Add(item:  (from, to, triggerCode));
            return true;
        }
        
        public bool AddAnyToTransition(int to, int triggerCode)
        {
            if (!_states.ContainsKey(to))
            {
                return false;
            }

            foreach (int key in _states.Keys)
            {
                _transitions.Add(item:  (key, to, triggerCode));
            }

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
            if (_states.ContainsKey(state))
            {
                return false;
            }

            _states.Remove(state);
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
            bool hasState = _states.ContainsKey(stateId);
            if (!hasState) return false;

            bool stateChanged = stateId != CurrentState;
            if (!stateChanged)
            {
                if (!forceSwitch) return false;
            }

            if (stateChanged)
            {
                if (_states.TryGetValue(CurrentState, out var oldActions))
                {
                    oldActions.OnExit?.Invoke(stateId);
                }
            }

            var oldStateId = CurrentState;
            var newActions = _states[stateId];

            CurrentState = stateId;
            newActions.TimeElapsed = 0;
            newActions.OnEnter?.Invoke(oldStateId);
            return true;
        }

        /// <summary>
        /// 不要在调用频率非常高的方法上使用委托传参，会造成大量gc
        /// </summary>
        /// <param name="time"></param>
        public void Update(float time)
        {
            if (_states.TryGetValue(CurrentState, out var actions))
            {
                beforeUpdateCallback?.Invoke(time);
                actions.TimeElapsed += time;
                actions.OnUpdate?.Invoke(time, actions.TimeElapsed);
                afterUpdateCallback?.Invoke(time);
            }
        }
    }
}