using Cysharp.Threading.Tasks;
using P1SModule.HotKeyView;
using UnityEngine;

namespace P1SModule.TestRecorder {
    public static class TestRecordManager {
        public static TestRecordWriter Writer { get; } = new TestRecordWriter(new DefaultWriterEvent());
        public static TestRecordPlayer Player { get; } = new TestRecordPlayer(new DefaultPlayEvent());

        [GlobalHotKey("녹화 시작", KeyCode.F5)]
        public static void StartRecord() {
            if (Player.IsPlaying) Player.Stop();
            Writer.Begin();
        }

        [GlobalHotKey("녹화 종료", KeyCode.F6)]
        public static void StopRecord() {
            Writer.Stop();
        }

        [GlobalHotKey("플레이 시작", KeyCode.F7)]
        public static void Play() {
            if (Writer.IsRecording) Writer.Stop();
            Player.Begin().Forget();
        }

        [GlobalHotKey("플레이 종료", KeyCode.F8)]
        public static void Stop() {
            Player.Stop();
        }
    }
}