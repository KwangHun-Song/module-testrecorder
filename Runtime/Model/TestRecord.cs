using System;
using System.Collections.Generic;

namespace P1SModule.TestRecorder {
    [Serializable]
    public class TestRecord {
        public string testName;
        public string testDescription;
        public List<TestRecordStep> steps;
    }
}