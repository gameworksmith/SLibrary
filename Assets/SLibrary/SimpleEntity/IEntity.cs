namespace SLibrary.SimpleEntity {
    public interface IEntity {
        void Register ();
        void UnRegister ();
        void Send(int actionId, EntityParam param, bool autoRelease);
        void Receive (int actionId, EntityParam param);

        void Destroy();
        
        EntityParamPool ParamPool { get; set; }
    }

}

