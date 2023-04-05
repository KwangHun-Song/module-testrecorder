using UnityEngine;

namespace P1SModule.TestRecorder {
    public interface IRaycastDetectorListener {
        void GetHit(GameObject gameObject);
    }
}