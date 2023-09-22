namespace P1SModule.TestRecorder {
    public interface IPlayerEvent {
        void OnStart();
        void OnEnd();
        void OnAbort();
        
        /// <summary>
        /// 테스트 도중 결과를 참고해서 중간 리포트 결과를 반환할 수 있다.
        /// Fail 반환시 즉시 테스트가 멈추며 테스트는 실패한다.
        /// </summary>
        /// <param name="testRecord">마지막으로 실행된 레코드</param>
        TestReport TestOnPlay(TestRecordStep testRecord);

        /// <summary>
        /// 테스트가 끝난 후 결과를 참고해서 리포트 결과를 반환할 수 있다.
        /// Fail 반환시 테스트는 실패한다.
        /// </summary>
        TestReport TestOnEnd();
    }
}