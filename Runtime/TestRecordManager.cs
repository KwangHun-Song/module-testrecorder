using Cysharp.Threading.Tasks;
using P1SModule.HotKeyView;
using UnityEngine;

namespace P1SModule.TestRecorder {
    public static class TestRecordManager {
        public static TestRecordPlayer Player { get; private set; } = new TestRecordPlayer(new DefaultPlayEvent());

        /// <summary>
        /// 임의의 Writer와 Player를 지정해주고 싶을 때 사용되는 함수입니다.
        /// </summary>
        public static void InitManually(TestRecordWriter customWriter, TestRecordPlayer customPlayer) {
            Player = customPlayer;
        }

        [GlobalHotKey("플레이 시작", KeyCode.F7)]
        public static void Play() {
            PlayAsync().Forget();

            async UniTask PlayAsync() {
                var report = await Player.Begin();
                ColoredDebug.Log($"{report.result} {report.failReason}", report.result == ReportResult.Pass ? DebugColor.Lime : DebugColor.Red);
            }
        }

        [GlobalHotKey("플레이 종료", KeyCode.F8)]
        public static void Stop() {
            Player.Stop();
        }
    }
}