using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace P1SModule.TestRecorder.Editor {
    public class TestRecordWriterEditor : EditorWindow {
        private enum Tab { Writer, Player }
        
        private RaycastDetector raycastDetector;
        private TestRecordWriter writer;
        private TestRecordPlayer player;
        private List<(PreDesignedFuncAttribute funcAttr, MethodInfo method)> preDesignedFuncs;
        
        private string savePath = "Assets/Resources/TestRecord";
        private string testName;
        private Tab currentTab = Tab.Writer;

        private void OnGUI() {
            if (Application.isPlaying == false) {
                EditorGUILayout.LabelField("게임을 실행해야 UI가 나타납니다.");
                return;
            }
            
            savePath = EditorGUILayout.TextField("파일 저장 위치", savePath);
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

        private void DrawPlayerTab() {
            // savePath에서 "*.json" 파일 검색
            var jsonFiles = Directory.GetFiles(savePath, "*.json");
            if (jsonFiles.Length == 0) {
                EditorGUILayout.LabelField("No json files found.");
                return;
            }

            if (player is { IsPlaying: true }) {
                EditorGUILayout.LabelField("플레이어 실행중");
            }

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

        #region PlayerTab

        private async UniTask WaitAndSaveReportAsync() {
            var result = await player.WaitUntilTestEnd();
            var sb = new StringBuilder();
            sb.AppendLine($"테스트 이름 : {result.testName}");
            sb.AppendLine($"테스트 결과 : {result.result}");
            sb.AppendLine($"테스트 진행과정 :");
            foreach (var stepReport in result.stepReports) {
                sb.AppendLine($"\t{stepReport.result} >> {stepReport.comment}");
            }
            
            File.WriteAllText($"{savePath}/{result.testName}_result.txt", sb.ToString());
            player = null;

            EditorUtility.DisplayDialog("완료", $"플레이가 완료되었습니다. 결과는 {savePath}/{result.testName}_result.txt 에 저장되었습니다.", "OK");
        }

        #endregion

        private void DrawWriterTab() {
            testName = EditorGUILayout.TextField("테스트 이름", testName);
            
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

        #region WriterTab

        private void DrawNotRecording() {
            if (GUILayout.Button("기록 시작")) {
                if (!raycastDetector) raycastDetector = RaycastDetector.CreateOne();
                writer = new TestRecordWriter(raycastDetector, new SimpleLogWriterEvent());
                writer.Start();
            }
        }

        private void StopAndSaveRecord() {
            var result = writer.Stop();
            var record = new TestRecord { testName = testName, steps = result };
            if (!Directory.Exists(savePath)) {
                Directory.CreateDirectory(savePath);
            }
            File.WriteAllText($"{savePath}/{testName}.json", JsonConvert.SerializeObject(record, Formatting.Indented));
            EditorUtility.DisplayDialog("성공", $"파일이 {savePath}/{testName}.json 위치에 저장되었습니다.", "OK");
                
            writer = null;
            testName = null;
        }

        private void DrawOnRecording() {
            // 사용 가능한 사전지정 함수들을 버튼으로 나열한다.
            preDesignedFuncs ??= GetPreDesignedFuncs();

            foreach (var preDesignedFunc in preDesignedFuncs) {
                if (GUILayout.Button(preDesignedFunc.funcAttr.Comment ?? preDesignedFunc.funcAttr.Key)) {
                    try {
                        preDesignedFunc.method?.Invoke(null, null);
                        writer.RecordPreDesignedFunc(preDesignedFunc.funcAttr.Key);
                    } catch(Exception ex) {
                        ColoredDebug.Log($"Error invoking method: {ex.Message}", DebugColor.Red);
                    }
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

        #region EditowWindow

        [MenuItem("P1SModule/TestRecorder/Writer")]
        public static void ShowWindow() {
            GetWindow<TestRecordWriterEditor>().Show();
        }

        #endregion
    }
}