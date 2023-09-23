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
        private List<(PreDesignedFuncAttribute funcAttr, MethodInfo method)> preDesignedFuncs;

        private void OnGUI() {
            if (Application.isPlaying == false) {
                EditorGUILayout.LabelField("게임을 실행해야 UI가 나타납니다.");
                return;
            }

            savePath = EditorGUILayout.TextField("파일 저장 위치", savePath);
            testName = EditorGUILayout.TextField("테스트 이름", testName);

            if (writer is not { IsRecording: true } && !string.IsNullOrEmpty(testName)) {
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
                if (GUILayout.Button(preDesignedFunc.funcAttr.Comment ?? preDesignedFunc.funcAttr.Key)) {
                    preDesignedFunc.method?.Invoke(null, null);
                    writer.AddPreDesignedFunc(preDesignedFunc.funcAttr.Key);
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

        #region EditowWindow

        [MenuItem("P1SModule/TestRecorder/Writer")]
        public static void ShowWindow() {
            GetWindow<TestRecordWriterEditor>().Show();
        }

        #endregion
    }
}