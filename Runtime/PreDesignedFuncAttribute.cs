using System;
using UnityEngine.Scripting;

namespace P1SModule.TestRecorder {
    [AttributeUsage(AttributeTargets.Method)]
    public class PreDesignedFuncAttribute : PreserveAttribute {
        public string Key { get; }
        public string Comment { get; }
        
        public PreDesignedFuncAttribute(string key, string comment = null) {
            Key = key;
            Comment = comment;
        }
    }
}