using UnityEngine;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 테스트 레코드에 extraParam을 남길 수 있는 이벤트.
    /// </summary>
    public interface IWriterEvent {
        string OnClick(GameObject gameObject);
    }
}