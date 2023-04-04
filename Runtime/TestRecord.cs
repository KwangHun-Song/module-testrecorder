namespace P1SModule.TestRecorder {
    public enum InputType { Down, Up }
    
    public class TestRecord {
        public float second;
        public InputType inputType;
        public string gameObjectPath;
        public string extraParam;
    }
}