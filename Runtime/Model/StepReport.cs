namespace P1SModule.TestRecorder {
    public enum StepResult { Pass, Execute, Fail, Aborted }

    public class StepReport {
        public readonly StepResult result;
        public readonly string comment;
        public StepReport(StepResult result, string comment = null) {
            this.result = result;
            this.comment = comment;
        }

        public static readonly StepReport Aborted = new StepReport(StepResult.Aborted);
        public static readonly StepReport Pass = new StepReport(StepResult.Pass);
        public static readonly StepReport FailInvalidStep = new StepReport(StepResult.Fail, nameof(FailInvalidStep));
        public static readonly StepReport FailInvalidFunc = new StepReport(StepResult.Fail, nameof(FailInvalidFunc));
        public static readonly StepReport FailToFindGameObject = new StepReport(StepResult.Fail, nameof(FailToFindGameObject));
        public static readonly StepReport FailToFindPreDesignedFunc = new StepReport(StepResult.Fail, nameof(FailToFindPreDesignedFunc));
    }
}