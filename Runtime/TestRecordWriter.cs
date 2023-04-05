using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using P1SModule.HotKeyView;
using P1SPlatform.Diagnostics;
using UnityEngine;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 플레이 화면에서 사용되는 인풋들을 기록한다
    /// 인풋 감지는 RaycastDetector에서 자동으로 해주고 기록은 IRaycastDetectorListener 인터페이스를 구현한 이벤트에서 진행한다.
    /// </summary>
    public class TestRecordWriter : IRaycastDetectorListener {
        public bool IsRecording { get; private set; }

        protected virtual string SavePath { get; } = "Assets/Resources/TestRecords.json";
        protected virtual RaycastDetector RaycastDetector { get; set; }
        protected virtual float LastInputSecond { get; set; }
        protected virtual List<TestRecord> Records { get; } = new List<TestRecord>();
        protected IWriterEvent Listener { get; }

        public TestRecordWriter(IWriterEvent listener = null) {
            Listener = listener;
        }

        public virtual void Begin() {
            if (IsRecording) {
                Debugger.Assert(false, "Already on recording");
                return;
            }
            
            IsRecording = true;
            LastInputSecond = Time.time; // 시간 기록을 초기화한다.
            Records.Clear();
            
            // 레이캐스트 디텍터를 만들고 활성화한다. 상속한 IRaycastDetectorListener 구현을 통해 이벤트를 받는다.
            RaycastDetector = CreateRaycastDetector();
            
            HotKeyHelper.AddKeyBindings(this);
            Listener?.OnStart();
        }

        public virtual void Stop() {
            File.WriteAllText(SavePath, JsonConvert.SerializeObject(Records, Formatting.Indented));
            
            IsRecording = false;
            Object.DestroyImmediate(RaycastDetector);
            LastInputSecond = default;
            Records.Clear();
            
            HotKeyHelper.RemoveKeyBindings(this);
            Listener?.OnEnd();
        }

        protected virtual RaycastDetector CreateRaycastDetector() {
            var raycastDetector = new GameObject().AddComponent<RaycastDetector>();
            Object.DontDestroyOnLoad(raycastDetector.transform);
            raycastDetector.Init(this);
            return raycastDetector;
        }

        #region IRaycastDetectorListener

        /// <summary>
        /// RaycastDetector에 등록한 이벤트에서 레이케스팅으로 만난 게임오브젝트를 받아온다.
        /// </summary>
        public virtual void GetHit(GameObject gameObject) {
            var time = GetElapsedTimeAndRenew();
            var path = gameObject.GetFullPath();
            var customParam = Listener?.OnClick(gameObject);
            
            Records.Add(new TestRecord {
                second = time,
                gameObjectPath = path,
                customParam = customParam
            });
        }
        
        // 이전 이벤트로부터 시간을 계산하고, 마지막 이벤트 시간을 갱신한다.
        protected virtual float GetElapsedTimeAndRenew() {
            var elapsed = Time.time - LastInputSecond;
            LastInputSecond = Time.time;
            return elapsed;
        }

        #endregion

        #region 핫키로 Knot을 등록하는 기능

        private int knotIndex = 0;
        [HotKey("Knot 등록", KeyCode.K)]
        public void AddKnot() {
            var time = GetElapsedTimeAndRenew();
            var knotName = $"knot{knotIndex++}";
            ColoredDebug.Log($"### 매듭 {knotName}이 기록되었습니다.", DebugColor.White);
            Records.Add(new TestRecord {
                second = time,
                customParam = knotName
            });
        }

        #endregion
    }
}