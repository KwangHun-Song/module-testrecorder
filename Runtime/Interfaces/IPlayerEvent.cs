namespace P1SModule.TestRecorder {
    public interface IPlayerEvent {
        void OnStart();
        void OnAbort();
        void OnExecuteStep(TestRecordStep step, StepReport stepReport);
        void OnEnd(TestResult result);
    }
}