using System;
using UnityEngine.Scripting;

namespace P1SModule.TestRecorder {
    [AttributeUsage(AttributeTargets.Method)]
    public class PreDesignedFuncAttribute : PreserveAttribute {
        public string Key { get; }
        public string Comment { get; }
        
        /// <summary>
        /// 어트리뷰트는 테스트 기록시 임의의 함수를 실행하기 위한 어트리뷰트입니다.
        /// public static 함수만 적용됩니다.
        /// </summary>
        /// <param name="key">함수를 식별하기 위한 식별자입니다.</param>
        /// <param name="comment">에디터에서 버튼에 보여지는 설명입니다. 지정하지 않을 경우 Key가 노출됩니다.</param>
        public PreDesignedFuncAttribute(string key, string comment = null) {
            Key = key;
            Comment = comment;
        }
    }
}