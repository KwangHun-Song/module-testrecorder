using UnityEngine;
using UnityEngine.EventSystems;

namespace P1SModule.TestRecorder {
    /// <summary>
    /// 시뮬레이션 터치를 지원하는 커스텀 인풋모듈.
    /// </summary>
    public class TestInputModule : StandaloneInputModule { 
        public void TouchAt(float normalizedX, float normalizedY) {
            Input.simulateMouseWithTouches = true;
            var pointerData = GetTouchPointerEventData(
                new Touch { position = new Vector2(Screen.width * normalizedX, Screen.height * normalizedY) }, out _, out _
            );
            
            ProcessTouchPress(pointerData, true, true);
        }
    }
}