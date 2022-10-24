namespace SLibrary.LightFsm
{
    public interface IFsm
    {
        int CurrentState { get; }
        bool AddState(int state, System.Action<int> onEnter, System.Action<int> onExit, System.Action<float> onUpdate);
        bool RemoveState(int state);
    
        void Update(float time);

        bool AddTransition(int from, int to, int triggerCode);
        bool TriggerEvent(int eventCode);
        bool SwitchToState(int stateId, bool forceSwitch);
    }
}
