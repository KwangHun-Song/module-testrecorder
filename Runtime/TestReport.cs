namespace P1SModule.TestRecorder {
    public enum ReportResult { Pass, Fail, Aborted }

    public class TestReport {
        public readonly ReportResult result;
        public readonly string failReason;
        public TestReport(ReportResult result, string failReason = "") {
            this.result = result;
            this.failReason = failReason;
        }

        public static TestReport Aborted = new TestReport(ReportResult.Aborted);
        public static TestReport Pass = new TestReport(ReportResult.Pass);
        public static TestReport FailNotValidStart = new TestReport(ReportResult.Fail, "Not Valid Start.");
        public static TestReport FailToFindGameObject = new TestReport(ReportResult.Fail, "Failed to find gameObject.");
    }
}