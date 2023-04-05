using UnityEngine;

namespace P1SModule.TestRecorder {
    public class DefaultWriterEvent : IWriterEvent {
        public void OnStart() {
            ColoredDebug.Log($"### Start Recording", DebugColor.Orange);
        }

        public void OnEnd() {
            ColoredDebug.Log($"### End Recording", DebugColor.Orange);
        }
        
        public string OnClick(GameObject gameObject) {
            ColoredDebug.Log($"### Click {gameObject.GetFullPath()}, DebugColor.Orange");
            return null;
        }
    }
}