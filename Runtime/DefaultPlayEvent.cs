namespace P1SModule.TestRecorder {
    /// <summary>
    /// 기본으로 지정되는 리스너로, 테스트 중간에 멈추지만 않고 끝까지 돌면 Pass를 반환
    /// </summary>
    public class DefaultPlayEvent : IPlayerEvent {
        public ReportResult TestOnPlay(TestRecord testRecord) => ReportResult.Pass;
        public ReportResult TestOnEnd() => ReportResult.Pass;
    }
}