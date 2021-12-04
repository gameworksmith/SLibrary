using System;
using System.Collections.Generic;

namespace SLibrary.LightFsm
{
    public class LightFsm : IFsm
    {
        public LightFsm(int defaultState = -1)
        {
            CurrentState = defaultState;
        }

        private readonly Dictionary<int, Tuple<System.Action<int>, System.Action<int>, System.Action<float>>> _actions =
            new Dictionary<int, Tuple<Action<int>, Action<int>, Action<float>>>();

        public int CurrentState { get; private set; }

        public bool AddState(int state, Action<int> onEnter, Action<int> onExit, Action<float> onUpdate)
        {
            if (_actions.ContainsKey(state))
            {
                throw new Exception($"不能重复添加状态{state}行为");
            }

            _actions.Add(state, new Tuple<Action<int>, Action<int>, Action<float>>(onEnter, onExit, onUpdate));
            return true;
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

        public void Update(float time, Action<float> beforeUpdate, Action<float> afterUpdate)
        {
            if (_actions.TryGetValue(CurrentState, out var actions))
            {
                beforeUpdate?.Invoke(time);
                actions.Item3?.Invoke(time);
                afterUpdate?.Invoke(time);
            }
        }
    }
}