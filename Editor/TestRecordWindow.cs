using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace P1SModule.TestRecorder.Editor {
    public class TestRecordWindow : EditorWindow {
        private enum Tab { Writer, Player }
        
        private RaycastDetector raycastDetector;
        private TestRecordWriter writer;
        private TestRecordPlayer player;
        private List<(PreDesignedFuncAttribute funcAttr, MethodInfo method)> preDesignedFuncs;
        
        private string savePath = "Assets/Resources/TestRecord";
        private string testName;
        private string testDescription;
        private Tab currentTab = Tab.Writer;

        private void OnGUI() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (Application.isPlaying == false) {
                EditorGUILayout.LabelField("게임을 실행해야 UI가 나타납니다.");
                return;
            }
            
            savePath = EditorGUILayout.TextField("파일 저장 위치", savePath);
            DrawCommonExplanation();
            currentTab = (Tab)GUILayout.Toolbar((int)currentTab, Enum.GetNames(typeof(Tab)));

            switch (currentTab) {
                case Tab.Writer:
                    DrawWriterTab();
                    break;
                case Tab.Player:
                    DrawPlayerTab();
                    break;
            }
        }

        private void DrawCommonExplanation() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(@"자동화된 테스트를 기록하고 플레이할 수 있는 창입니다.
아래 Writer 탭을 클릭하면 테스트 기록을,
Player 탭을 클릭하면 기록한 테스트를 플레이할 수 있습니다.", MessageType.Info);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void DrawWriterTab() {
            EditorGUILayout.Space();
            DrawWriterTabExplanation();
            
            testName = EditorGUILayout.TextField("테스트 이름", testName);
            testDescription = EditorGUILayout.TextField("테스트 설명(옵션)", testDescription);
            
            EditorGUILayout.Space();

            if (writer is not { IsRecording: true }) {
                if (string.IsNullOrEmpty(testName)) {
                    EditorGUILayout.LabelField("테스트 이름을 기록해주세요.");
                    return;
                }

                DrawNotRecording();
                return;
            }

            if (GUILayout.Button("기록 종료")) {
                StopAndSaveRecord();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(@"미리 지정한 함수들을 실행할 수 있습니다.
여기서 실행된 함수들은 레코드 플레이어가 그대로 실행합니다.
함수를 추가하는 방법은 public static 함수에 [PreDesignedFunc] 어트리뷰트를 붙이면 됩니다.", MessageType.Info);
            EditorGUILayout.Space();
            
            DrawOnRecording();
        }

        private void DrawPlayerTab() {
            EditorGUILayout.Space();
            DrawPlayerTabExplanation();
            
            // savePath에서 "*.json" 파일 검색
            var jsonFiles = Directory.GetFiles(savePath, "*.json");
            if (jsonFiles.Length == 0) {
                EditorGUILayout.LabelField("No json files found.");
                return;
            }

            if (player is { IsPlaying: true }) {
                EditorGUILayout.LabelField("플레이어 실행중");
                if (GUILayout.Button("테스트 강제 중지")) {
                    player.Abort();
                }
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("실행 가능한 테스트들");
            EditorGUILayout.Space();

            // 파일 이름들을 읽어와서 foreach 돌면서 GUILayout.Button으로 분기
            foreach (var filePath in jsonFiles) {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (GUILayout.Button(fileName)) {
                    player ??= new TestRecordPlayer(new SimpleLogPlayerEvent());
                    player.Start(JsonConvert.DeserializeObject<TestRecord>(File.ReadAllText($"{savePath}/{fileName}.json")));
                    WaitAndSaveReportAsync().Forget();
                }
            }
        }

        #region WriterTab

        private void DrawWriterTabExplanation() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(@"테스트를 기록할 수 있는 탭입니다.
테스트 이름을 적은 후 기록 시작 버튼을 누르면 기록이 시작됩니다.
기록이 시작되면 사전지정한 함수들 버튼들이 아래에 나타나는데, 그 버튼을 눌러 해당 함수들을 실행할 수 있습니다.
(사전지정 함수는 [PreDesignedFunc] 어트리뷰트를 붙이면 만들 수 있습니다.)
기록 종료를 누르면 파일 저장 위치에 {테스트 이름}.json의 이름으로 파일이 저장됩니다.", MessageType.Info);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void DrawNotRecording() {
            if (GUILayout.Button("기록 시작")) {
                if (!raycastDetector) raycastDetector = RaycastDetector.CreateOne();
                writer = new TestRecordWriter(raycastDetector, new SimpleLogWriterEvent());
                writer.Start();
            }
        }

        private void StopAndSaveRecord() {
            var result = writer.Stop();
            var record = new TestRecord { testName = testName, testDescription = testDescription, steps = result };
            if (!Directory.Exists(savePath)) {
                Directory.CreateDirectory(savePath);
            }
            File.WriteAllText($"{savePath}/{testName}.json", JsonConvert.SerializeObject(record, Formatting.Indented));
            EditorUtility.DisplayDialog("성공", $"파일이 {savePath}/{testName}.json 위치에 저장되었습니다.", "OK");
                
            writer = null;
            testName = null;
        }

        private void DrawOnRecording() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("사전지정 함수들");
            EditorGUILayout.Space();
            
            // 사용 가능한 사전지정 함수들을 버튼으로 나열한다.
            preDesignedFuncs ??= GetPreDesignedFuncs();

            foreach (var preDesignedFunc in preDesignedFuncs) {
                if (GUILayout.Button(preDesignedFunc.funcAttr.Comment ?? preDesignedFunc.funcAttr.Key)) {
                    try {
                        writer.RecordPreDesignedFunc(preDesignedFunc.funcAttr.Key);
                        InvokePreDesignedFunc(preDesignedFunc.method).Forget();
                    } catch(Exception ex) {
                        ColoredDebug.Log($"Error invoking method: {ex.Message}", DebugColor.Red);
                    }
                }
            }
            
            // 함수를 실행했는데, 비동기함수면 함수 실행이 끝난 뒤 인풋타임을 초기화한다.
            async UniTask InvokePreDesignedFunc(MethodInfo method) {
                var result = method?.Invoke(null, null);
                switch (result) {
                    case UniTask<StepReport> stepResultTask:
                        await stepResultTask;
                        writer.RenewLastInputTime();
                        break;
                }
            }
        }

        private List<(PreDesignedFuncAttribute, MethodInfo)> GetPreDesignedFuncs() {
            // 리플렉션으로 PreDesignedFunc 어트리뷰트를 가진 함수들을 긁어와 저장한다.
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass)
                .SelectMany(type => type.GetRuntimeMethods())
                .Where(method => method.IsStatic)
                .Select(method => (Attribute: method.GetCustomAttribute<PreDesignedFuncAttribute>(), MethodInfo: method))
                .Where(pair => pair.Attribute != null)
                .ToList();
        }

        #endregion

        #region PlayerTab

        private void DrawPlayerTabExplanation() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(@"테스트를 실행할 수 있는 탭입니다.
아래의 테스트 버튼들 중 하나를 누르면 테스트가 시작됩니다.
테스트가 종료되면, 파일 저장 위치에 {테스트 이름}_result.txt의 이름으로 파일이 저장됩니다.
테스트 기록 후 바로 실행하면 내용을 읽지 못하는 경우가 있는데, 그 경우 유니티를 리프레시해주세요.
", MessageType.Info);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private async UniTask WaitAndSaveReportAsync() {
            var report = await player.WaitUntilTestEnd();
            File.WriteAllText($"{savePath}/{report.testName}_result.txt", ReportWriter.GetReportString(report));
            player = null;

            EditorUtility.DisplayDialog("완료", $"플레이가 완료되었습니다. 결과는 {savePath}/{report.testName}_result.txt 에 저장되었습니다.", "OK");
        }

        #endregion

        #region EditowWindow

        [MenuItem("P1SModule/TestRecorder/에디터 윈도우 열기")]
        public static void ShowWindow() {
            GetWindow<TestRecordWindow>().Show();
        }

        #endregion
    }
}