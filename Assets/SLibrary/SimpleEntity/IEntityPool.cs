namespace SLibrary.SimpleEntity {
    public interface IEntityPool {
        void Register (IEntity entity);
        void UnRegister (IEntity entity);
        void Deal(int actionId, EntityParam param, bool autoRelease);
    }
}