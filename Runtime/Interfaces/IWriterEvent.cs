namespace P1SModule.TestRecorder {
    public interface IWriterEvent {
        void OnStart();
        void OnEnd();
        void OnClick(string gameObjectPath);
        void OnPreDesignedFunc(string funcName);
    }
}