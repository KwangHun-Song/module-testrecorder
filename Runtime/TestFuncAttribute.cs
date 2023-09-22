using System;
using UnityEngine.Scripting;

namespace P1SModule.TestRecorder {
    [AttributeUsage(AttributeTargets.Method)]
    public class TestFuncAttribute : PreserveAttribute {
        public string FuncName { get; }
        
        public TestFuncAttribute(string funcName) {
            FuncName = funcName;
        }
    }
}