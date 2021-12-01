namespace SLibrary.Interfaces
{
    public interface IGameLoop
    {
        void AddListener(System.Action action, int frequency);
        void RemoveListener(System.Action action);

        void Update();
    }
}