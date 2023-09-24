using System.Text;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 리포트를 출력할 수 있는 스트링 형태로 바꾸어준다.
    /// </summary>
    public class ReportWriter {
        public static string GetReportString(TestReport report) {
            var sb = new StringBuilder();
            sb.AppendLine($"테스트 이름 : {report.testName}");
            sb.AppendLine($"테스트 결과 : {report.result}");
            sb.AppendLine($"테스트 설명 : {report.description}");
            sb.AppendLine($"테스트 진행과정 :");
            foreach (var stepReport in report.stepReports) {
                if (stepReport.result == StepResult.Execute) {
                    sb.AppendLine($"\tExecute >> {stepReport.comment}");
                } else if (string.IsNullOrEmpty(stepReport.comment)) {
                    sb.AppendLine($"\t\t{stepReport.result}");
                } else {
                    sb.AppendLine($"\t{stepReport.result} :: {stepReport.comment}");
                }
            }

            return sb.ToString();
        }
    }
}