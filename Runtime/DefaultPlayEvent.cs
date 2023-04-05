namespace P1SModule.TestRecorder {
    /// <summary>
    /// 기본으로 지정되는 리스너로, 테스트 중간에 멈추지만 않고 끝까지 돌면 Pass를 반환.
    /// </summary>
    public class DefaultPlayEvent : IPlayerEvent {
        public void OnStart() {
            ColoredDebug.Log("### Test is Started!", DebugColor.Purple);
        }

        public void OnEnd() {
            ColoredDebug.Log("### Test is Ended!", DebugColor.Purple);
        }

        public void OnAbort() {
            ColoredDebug.Log("### Test is Aborted!", DebugColor.Purple);
        }

        public TestReport TestOnEnd() => TestReport.Pass;

        public TestReport TestOnPlay(TestRecord testRecord) {
            // knot을 사용하는 예시
            if (testRecord.customParam.StartsWith("knot")) {
                if (int.TryParse(testRecord.customParam.Replace("knot", ""), out var knotIndex)) {
                    return CheckKnot(knotIndex);
                }
            }
            
            ColoredDebug.Log($"### TestPlayer Click >> {testRecord.gameObjectPath}", DebugColor.Purple);
            return TestReport.Pass;
        }

        private TestReport CheckKnot(int knotIndex) {
            // 원하는 시점에 지정한 인덱스를 받아올 수 있으므로, 이 때 검사한다.
            // 예를 들어서 knotIndex가 1인 시점에 코인이 1000개여야 한다면 다음 방식으로 사용할 수 있다.
            /*
             * if (knotIndex == 1) {
             *     if (Wallet.GetItemCount("coin") != 1000) {
             *         return new TestReport(ReportResult.Fail, $"knot {knotIndex} 실패! 코인이 1000개여야 하는데 그렇지 않습니다.");
             *     }
             * }
             */

            return TestReport.Pass;
        }
    }
}