using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using P1SPlatform.Diagnostics;
using UnityEngine;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 플레이 화면에서 사용되는 인풋들을 기록한다
    /// 인풋 감지는 RaycastDetector에서 자동으로 해주고 기록은 IRaycastDetectorListener 인터페이스를 구현한 이벤트에서 진행한다.
    /// </summary>
    public class TestRecordWriter : IRaycastDetectorListener {
        public bool IsRecording { get; private set; }
        public RaycastDetector RaycastDetector { get; }
        public List<TestRecordStep> Records { get; } = new List<TestRecordStep>();
        public IWriterEvent Listener { get; }
        
        private UniTaskCompletionSource<List<TestRecordStep>> CompletionSource { get; set; }
        private float LastInputSecond { get; set; }

        public TestRecordWriter([NotNull] RaycastDetector raycastDetector, IWriterEvent listener = null) {
            RaycastDetector = raycastDetector;
            RaycastDetector.Listeners.Add(this);
            Listener = listener ?? new DefaultWriterEvent();
        }

        ~TestRecordWriter() {
            RaycastDetector.Listeners.Remove(this);
        }

        public void Begin() {
            if (IsRecording) {
                Debugger.Assert(false, "Already on recording");
                return;
            }

            IsRecording = true;
            LastInputSecond = Time.time; // 시간 기록을 초기화한다.
            Records.Clear();
            
            Listener?.OnStart();
            CompletionSource = new UniTaskCompletionSource<List<TestRecordStep>>();
        }

        public UniTask WaitUntilTestEnd() => CompletionSource?.Task ?? UniTask.CompletedTask;

        public List<TestRecordStep> Stop() {
            if (IsRecording == false) return null;
            IsRecording = false;
            Listener?.OnEnd();

            CompletionSource?.TrySetResult(Records);
            return Records;
        }

        public void AddPreDesignedFunc(string funcName) {
            var time = GetElapsedTimeAndRenew();
            ColoredDebug.Log($"### {funcName}이 기록되었습니다.", DebugColor.White);
            Records.Add(new TestRecordStep {
                second = time,
                preDesignedFunc = funcName
            });
        }
        
        // 이전 이벤트로부터 시간을 계산하고, 마지막 이벤트 시간을 갱신한다.
        protected virtual float GetElapsedTimeAndRenew() {
            var elapsed = Time.time - LastInputSecond;
            LastInputSecond = Time.time;
            return elapsed;
        }

        #region IRaycastDetectorListener

        /// <summary>
        /// RaycastDetector에 등록한 이벤트에서 레이케스팅으로 만난 게임오브젝트를 받아온다.
        /// </summary>
        public virtual void GetHit(GameObject gameObject) {
            var time = GetElapsedTimeAndRenew();
            var path = gameObject.GetFullPath();
            var customParam = Listener?.OnClick(gameObject);
            
            Records.Add(new TestRecordStep {
                second = time,
                gameObjectPath = path,
                preDesignedFunc = customParam
            });
        }

        #endregion
    }
}