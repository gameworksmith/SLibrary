namespace SLibrary.LightFsm
{
    public interface IFsm
    {
        bool AddState(int state, System.Action<int> onEnter, System.Action<int> onExit, System.Action<float> onUpdate);
        bool RemoveState(int state);
    
        void Update(float time, System.Action<float> beforeUpdate, System.Action<float> afterUpdate);
        bool SwitchToState(int stateId, bool forceSwitch);
        int CurrentState { get; }
    }
}
