using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace P1SModule.TestRecorder.Editor {
    public class TestRecordWriterEditor : EditorWindow {
        private RaycastDetector raycastDetector;
        private TestRecordWriter writer;
        
        private string savePath = "Assets/Resources/TestRecord";
        private string testName;
        private List<(string funcName, MethodInfo method)> preDesignedFuncs;

        private void OnGUI() {
            if (Application.isPlaying == false) {
                EditorGUILayout.LabelField("게임을 실행해야 UI가 나타납니다.");
                return;
            }

            savePath = EditorGUILayout.TextField("파일 저장 위치", savePath);
            testName = EditorGUILayout.TextField("테스트 이름", testName);

            if (writer is not { IsRecording: true }) {
                if (GUILayout.Button("기록 시작")) {
                    if (!raycastDetector) raycastDetector = RaycastDetector.CreateOne();
                    writer = new TestRecordWriter(raycastDetector);
                    writer.Begin();
                }

                return;
            }

            if (GUILayout.Button("기록 종료")) {
                var result = writer.Stop();
                var record = new TestRecord { testName = testName, steps = result };
                if (!Directory.Exists(savePath)) {
                    Directory.CreateDirectory(savePath);
                }
                File.WriteAllText($"{savePath}/{testName}.json", JsonConvert.SerializeObject(record, Formatting.Indented));
                EditorUtility.DisplayDialog("성공", $"파일이 {savePath}/{testName}.json 위치에 저장되었습니다.", "OK");
                
                writer = null;
                testName = null;
                return;
            }
                
            ShowPreDesignedFuncs();
        }

        private void ShowPreDesignedFuncs() {
            preDesignedFuncs ??= GetPreDesignedFuncs();

            foreach (var preDesignedFunc in preDesignedFuncs) {
                if (GUILayout.Button(preDesignedFunc.funcName)) {
                    preDesignedFunc.method?.Invoke(null, null);
                }
            }
        }

        private List<(string, MethodInfo)> GetPreDesignedFuncs() {
            var funcs = new List<(string, MethodInfo)>();
            
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            foreach (var type in allTypes) {
                if (type.IsClass == false) continue;
            
                foreach (var methodInfo in type.GetRuntimeMethods()) {
                    if (methodInfo.IsStatic == false) continue;
                
                    var attribute = methodInfo.GetCustomAttribute<TestFuncAttribute>();
                    if (attribute == null) continue;
                    
                    funcs.Add((attribute.FuncName, methodInfo));
                }
            }

            return funcs;
        }


        #region EditowWindow

        [MenuItem("P1SModule/TestRecorder/Writer")]
        public static void ShowWindow() {
            GetWindow<TestRecordWriterEditor>().Show();
        }

        #endregion
    }
}