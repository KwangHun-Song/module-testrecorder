namespace P1SModule.TestRecorder {
    /// <summary>
    /// 로그만 등록한 간단한 이벤트
    /// </summary>
    public class SimpleLogPlayerEvent : IPlayerEvent {
        public void OnStart() {
            ColoredDebug.Log("### Start Playing", DebugColor.Orange);
        }

        public void OnAbort() {
            ColoredDebug.Log("### Abort Playing", DebugColor.Red);
        }

        public void OnExecuteStep(TestRecordStep step, StepReport stepReport) {
            ColoredDebug.Log($"### Play Step >> path:{step.gameObjectPath}, func:{step.preDesignedFunc}", DebugColor.Purple);
        }

        public void OnEnd(TestResult result) {
            ColoredDebug.Log($"### End Playing, Result: {result}", DebugColor.Orange);
        }
    }
}