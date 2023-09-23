using System;
using UnityEngine;

namespace P1SModule.TestRecorder {
    public enum DebugColor { Cyan, Lime, Yellow, White, Orange, Purple, Red, }
    public class ColoredDebug {
        public static void Log(object message, DebugColor color = DebugColor.Cyan) {
            LogInternal(message, GetHtmlColorCode(color));
        }

        public static void Log(object message, string htmlColorCode) {
            LogInternal(message, htmlColorCode);
        }

        private static void LogInternal(object message, string htmlColorCode) {
#if UNITY_EDITOR
            Debug.Log($"<color=#{htmlColorCode}>{message}</color>");
#else
            // 에디터가 아닌 경우 콘솔에 색깔이 출력되지 않으므로 적용하지 않는다.
            Debug.Log(message);
#endif
        }

        public static string GetHtmlColorCode(DebugColor color) {
            return color switch {
                DebugColor.Cyan => "00FFFF",
                DebugColor.Lime => "00FF22",
                DebugColor.Yellow => "FFFF00",
                DebugColor.White => "FFFFFF",
                DebugColor.Orange => "FAD656",
                DebugColor.Purple => "C99BFF",
                DebugColor.Red => "D77265",
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }
    }
}