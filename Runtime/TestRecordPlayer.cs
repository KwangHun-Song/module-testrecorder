using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace P1SModule.TestRecorder {
    public class TestRecordPlayer {
        public bool IsPlaying => completionSource != null && !completionSource.Task.Status.IsCompleted();
        private TestInputModule InputModule { get; set; }
        [CanBeNull] private IPlayerEvent Listener { get; }

        private CancellationTokenSource cancellationTokenSource;
        private Dictionary<string, MethodInfo> preDesignedFuncs;
        private UniTaskCompletionSource<TestReport> completionSource;
        
        public TestRecordPlayer([CanBeNull] IPlayerEvent listener) {
            Listener = listener;
        }

        public void Start(TestRecord record) {
            if (IsPlaying) return;
            
            completionSource = new UniTaskCompletionSource<TestReport>();

            Listener?.OnStart();
            StartInternal(record).Forget();
        }

        public UniTask<TestReport> WaitUntilTestEnd() {
            return completionSource.Task;
        }

        public void Abort() {
            DisposeTest();
            cancellationTokenSource.Cancel();
            Listener?.OnAbort();
        }

        private async UniTask StartInternal(TestRecord record) {
            var testReport = new TestReport(record.testName, record.testDescription);

            InitTest();

            foreach (var step in record.steps) {
                testReport.stepReports.Add(new StepReport(StepResult.Execute, step.ToString()));
                var stepReport = await ExecuteStepAsync(step);
                testReport.stepReports.Add(stepReport);
                Listener?.OnExecuteStep(step, stepReport);

                if (stepReport is { result: not StepResult.Pass }) {
                    testReport.result = TestResult.Fail;
                    DisposeTest();
                    completionSource.TrySetResult(testReport);
                    Listener?.OnEnd(testReport.result);
                    return;
                }
            }

            testReport.result = TestResult.Pass;
            DisposeTest();
            completionSource.TrySetResult(testReport);
            Listener?.OnEnd(testReport.result);
        }

        private void InitTest() {
            if (InputModule == null || EventSystem.current.currentInputModule is not TestInputModule) {
                Object.Destroy(EventSystem.current.currentInputModule);
                InputModule = EventSystem.current.gameObject.AddComponent<TestInputModule>();
            }
            
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private async UniTask<StepReport> ExecuteStepAsync(TestRecordStep step) {
            await UniTask.Delay(TimeSpan.FromSeconds(step.second), cancellationToken: cancellationTokenSource.Token);

            if (cancellationTokenSource.IsCancellationRequested) {
                return StepReport.Aborted;
            }

            if (!string.IsNullOrEmpty(step.gameObjectPath)) {
                return await InvokeClickAsync(step.gameObjectPath);
            }

            if (!string.IsNullOrEmpty(step.preDesignedFunc)) {
                return await InvokePreDesignedFuncAsync(step.preDesignedFunc);
            }

            return StepReport.FailInvalidStep;
        }

        private void DisposeTest() {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        private UniTask<StepReport> InvokeClickAsync(string gameObjectPath) {
            var gameObject = GameObject.Find(gameObjectPath);
            if (gameObject == null) {
                return UniTask.FromResult(StepReport.FailToFindGameObject);
            }
            
            var position = Camera.main!.WorldToViewportPoint(gameObject.transform.position);
            InputModule.TouchAt(position.x, position.y);
            return UniTask.FromResult(StepReport.Pass);
        }

        private async UniTask<StepReport> InvokePreDesignedFuncAsync(string funcKey) {
            preDesignedFuncs ??= GetPreDesignedFuncs();

            if (preDesignedFuncs.TryGetValue(funcKey, out var methodInfo) == false) {
                return StepReport.FailToFindPreDesignedFunc;
            }
            
            var stepResult = methodInfo.Invoke(null, null);

            return stepResult switch {
                UniTask<StepReport> stepResultTask => await stepResultTask,
                StepReport report => report,
                _ => StepReport.FailInvalidFunc
            };
        }

        private Dictionary<string, MethodInfo> GetPreDesignedFuncs() {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass)
                .SelectMany(type => type.GetRuntimeMethods())
                .Where(method => method.IsStatic)
                .Select(method => (Attribute: method.GetCustomAttribute<PreDesignedFuncAttribute>(), MethodInfo: method))
                .Where(pair => pair.Attribute != null)
                .ToDictionary(pair => pair.Attribute.Key, pair => pair.MethodInfo);
        }
    }
}