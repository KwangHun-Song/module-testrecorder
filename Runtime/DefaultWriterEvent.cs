using UnityEngine;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 레코드 등록시 로그만 남겨주는 이벤트
    /// </summary>
    public class DefaultWriterEvent : IWriterEvent {
        public string OnClick(GameObject gameObject) {
            ColoredDebug.Log($"Click {gameObject.GetFullPath()}");
            return null;
        }
    }
}