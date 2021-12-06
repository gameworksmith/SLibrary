namespace SLibrary.Interfaces
{
    public interface IGameLoop
    {
        void AddListener(System.Action action, int frequency);
        void RemoveListener(System.Action action);
        
        
        void AddLateListener(System.Action action);
        void RemoveLateListener(System.Action action);
        
        void Update();

        void LateUpdate();
    }
}