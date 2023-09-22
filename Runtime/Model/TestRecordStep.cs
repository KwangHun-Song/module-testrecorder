using System;

namespace P1SModule.TestRecorder {
    [Serializable]
    public class TestRecordStep {
        /// <summary>
        /// 이전 기록으로부터 몇 초 후 실행되는지를 체크합니다.
        /// </summary>
        public float second;
        /// <summary>
        /// 선택한 게임오브젝트의 하이에라키상 경로를 반환합니다.
        /// </summary>
        public string gameObjectPath;
        /// <summary>
        /// 미리 지정한 함수 이름
        /// </summary>
        public string preDesignedFunc;
    }
}