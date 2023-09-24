using System.Collections.Generic;

namespace P1SModule.TestRecorder {
    public enum TestResult { Pass, Fail, Aborted }
    public class TestReport {
        public readonly string testName;
        public readonly string description;
        public TestResult result;
        public List<StepReport> stepReports = new List<StepReport>();
        
        public TestReport(string testName, string description) {
            this.testName = testName;
            this.description = description;
        }
    }
}