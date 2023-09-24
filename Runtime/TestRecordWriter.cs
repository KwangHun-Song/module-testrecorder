using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 플레이 화면에서 사용되는 인풋들을 기록한다
    /// 인풋 감지는 RaycastDetector에서 자동으로 해주고 기록은 IRaycastDetectorListener 인터페이스를 구현한 이벤트에서 진행한다.
    /// </summary>
    public class TestRecordWriter : IRaycastDetectorListener, System.IDisposable {
        public bool IsRecording => completionSource != null && !completionSource.Task.Status.IsCompleted();

        public RaycastDetector RaycastDetector { get; }
        public List<TestRecordStep> Records { get; } = new List<TestRecordStep>();

        [CanBeNull] private IWriterEvent Listener { get; }

        private float lastInputSecond;
        private UniTaskCompletionSource<List<TestRecordStep>> completionSource;

        public TestRecordWriter([NotNull] RaycastDetector raycastDetector, IWriterEvent listener = null) {
            RaycastDetector = raycastDetector;
            RaycastDetector.Listeners.Add(this);
            Listener = listener;
        }

        public void Dispose() {
            RaycastDetector.Listeners.Remove(this);
        }

        public void Start() {
            if (IsRecording) {
                return;
            }

            RenewLastInputTime();
            Records.Clear();

            Listener?.OnStart();
            completionSource = new UniTaskCompletionSource<List<TestRecordStep>>();
        }

        public UniTask<List<TestRecordStep>> WaitUntilTestEnd() {
            return completionSource?.Task ?? UniTask.FromResult<List<TestRecordStep>>(null);
        }

        public List<TestRecordStep> Stop() {
            if (!IsRecording) return null;
            Listener?.OnEnd();

            completionSource?.TrySetResult(Records);
            return Records;
        }

        public void RecordPreDesignedFunc(string funcName) {
            var time = GetElapsedTimeAndRenew();
            Listener?.OnPreDesignedFunc(funcName);
            Records.Add(new TestRecordStep {
                second = time,
                preDesignedFunc = funcName
            });
        }

        public void RenewLastInputTime() {
            lastInputSecond = Time.realtimeSinceStartup;
        }

        // 이전 이벤트로부터 시간을 계산하고, 마지막 이벤트 시간을 갱신한다.
        private float GetElapsedTimeAndRenew() {
            var currentTime = Time.realtimeSinceStartup;
            var elapsed = currentTime - lastInputSecond;
            RenewLastInputTime();
            return elapsed;
        }

        #region IRaycastDetectorListener

        /// <summary>
        /// RaycastDetector에 등록한 이벤트에서 레이케스팅으로 만난 게임오브젝트를 받아온다.
        /// </summary>
        public void GetHit(GameObject gameObject) {
            var time = GetElapsedTimeAndRenew();
            var path = gameObject.GetFullPath();
            Listener?.OnClick(path);

            Records.Add(new TestRecordStep {
                second = time,
                gameObjectPath = path
            });
        }

        #endregion
    }
}
