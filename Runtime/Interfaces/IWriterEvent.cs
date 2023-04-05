using UnityEngine;

namespace P1SModule.TestRecorder {
    public interface IWriterEvent {
        void OnStart();
        void OnEnd();
        
        /// <summary>
        /// 특정 게임오브젝트가 클릭되었을 때 커스텀 파라미터를 남길 수 있다.
        /// IPlayerEvent에서 특정 파라미터의 이벤트가 실행되었을 때 이벤트를 추가해서 사용할 수 있다.
        /// </summary>
        string OnClick(GameObject gameObject);
    }
}