namespace P1SModule.TestRecorder {
    /// <summary>
    /// 로그만 등록한 간단한 이벤트
    /// </summary>
    public class SimpleLogWriterEvent : IWriterEvent {
        public void OnStart() {
            ColoredDebug.Log("### Start Recording", DebugColor.Orange);
        }

        public void OnEnd() {
            ColoredDebug.Log("### End Recording", DebugColor.Orange);
        }

        public void OnClick(string gameObjectPath) {
            ColoredDebug.Log($"### Click {gameObjectPath}", DebugColor.Purple);
        }

        public void OnPreDesignedFunc(string funcName) {
            ColoredDebug.Log($"### Record {funcName}", DebugColor.Purple);
        }
    }
}