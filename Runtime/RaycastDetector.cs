using UnityEngine;
using UnityEngine.EventSystems;

namespace P1SModule.TestRecorder {
    public class RaycastDetector : MonoBehaviour {
        private IRaycastDetectorListener Listener { get; set; }
        
        public void Init(IRaycastDetectorListener listener) {
            Listener = listener;
        }
        
        private void Update() {
            if (Input.GetMouseButtonUp(0)) {
                CheckAndSendListener();
            }
            
            void CheckAndSendListener() {
                var hitGo = GetRaycastByGraphic() ?? GetRaycastByCollider();
                if (hitGo != null) {
                    Listener.GetHit(hitGo);
                }
            }

            GameObject GetRaycastByCollider() {
                var pos = Camera.main?.ScreenToWorldPoint(Input.mousePosition) ?? default;
                var hit = Physics2D.Raycast(pos, Vector3.forward, Mathf.Infinity);
                return hit.transform?.gameObject;
            }

            GameObject GetRaycastByGraphic() {
                return EventSystem.current.currentSelectedGameObject;
            }
        }
    }
}