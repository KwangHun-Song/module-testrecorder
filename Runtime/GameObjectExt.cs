using UnityEngine;

namespace P1SModule.TestRecorder {
    public static class GameObjectExt {
        public static string GetFullPath(this GameObject go) {
            var transform = go.transform;
            var path = transform.name;
            while (transform.parent != null) {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
    }
}