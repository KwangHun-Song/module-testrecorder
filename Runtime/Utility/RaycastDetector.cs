using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace P1SModule.TestRecorder {
    public class RaycastDetector : MonoBehaviour {
        public List<IRaycastDetectorListener> Listeners { get; } = new List<IRaycastDetectorListener>();
        
        private void Update() {
            if (Input.GetMouseButtonUp(0)) {
                CheckAndSendListener();
            }
            
            void CheckAndSendListener() {
                var hitGo = GetRaycastByGraphic() ?? GetRaycastByCollider();
                if (hitGo == null) return;
                foreach (var listener in Listeners) {
                    listener.GetHit(hitGo);
                }
            }
        }

        protected virtual GameObject GetRaycastByGraphic() {
            return EventSystem.current.currentSelectedGameObject;
        }

        protected virtual GameObject GetRaycastByCollider() {
            var pos = Camera.main?.ScreenToWorldPoint(Input.mousePosition) ?? default;
            var hit = Physics2D.Raycast(pos, Vector3.forward, Mathf.Infinity);
            return hit.transform?.gameObject;
        }
    }
}