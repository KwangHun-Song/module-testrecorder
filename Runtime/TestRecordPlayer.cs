using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using P1SPlatform.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace P1SModule.TestRecorder {
    public enum ReportResult { Pass, Fail }
    
    public class TestRecordPlayer {
        public bool IsPlaying { get; set; }
        
        protected CancellationTokenSource tokenSource;
        protected virtual TestInputModule InputModule { get; set; }
        protected IPlayerEvent Listener { get; }
        
        public TestRecordPlayer(IPlayerEvent listener) {
            Debugger.Assert(listener != null, "테스트 결과를 정해주는 리스너를 구현해주세요.");
            Listener = listener;
        }
        
        public virtual async UniTask<ReportResult> Begin() {
            if (IsPlaying) {
                Debugger.Assert(false, "Already on playing");
                return ReportResult.Fail;
            }
            
            IsPlaying = true;
            tokenSource = new CancellationTokenSource();
            InitInputSystem();

            // 레코드 파일을 읽는다.
            var records = GetTestRecord();

            // 순서대로 실행한다.
            foreach (var record in records) {
                await UniTask.Delay((int)(record.second * 1000));
                await InvokeEventAsync(record);
                
                // 이벤트를 등록했으면 중간에 결과를 확인하고 테스트를 멈출 수 있다.
                if (Listener.TestOnPlay(record) == ReportResult.Fail) {
                    return ReportResult.Fail;
                }
            }

            // 등록한 이벤트를 사용해 리포트 결과를 정할 수 있다.
            return Listener.TestOnEnd();
        }

        public virtual void Stop() {
            DisposeInputSystem();
            tokenSource.Cancel();
            IsPlaying = false;
        }

        protected virtual void InitInputSystem() {
            // 기존 이벤트시스템은 비활성화하고 새로운 이벤트시스템을 만든다.
            if (InputModule == null || !(EventSystem.current.currentInputModule is TestInputModule)) {
                Object.Destroy(EventSystem.current.currentInputModule);
                InputModule = EventSystem.current.gameObject.AddComponent<TestInputModule>();
            }
        }

        protected virtual void DisposeInputSystem() { }

        protected virtual IEnumerable<TestRecord> GetTestRecord() {
            var recordsAssets = Resources.Load("TestRecords") as TextAsset;
            if (recordsAssets == null) return Enumerable.Empty<TestRecord>();
            return JsonConvert.DeserializeObject<List<TestRecord>>(recordsAssets.text);
        }

        protected virtual async UniTask InvokeEventAsync(TestRecord record) {
            var position = Camera.main.WorldToViewportPoint(GameObject.Find(record.gameObjectPath).transform.position);
            InputModule.TouchAt(position.x, position.y);
        }
    }
}