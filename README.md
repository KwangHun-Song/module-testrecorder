# TestRecorder

## 개요
자동화된 테스트를 기록하고 실행할 수 있는 모듈.<br>
테스트를 에디터로 게임을 실행하고 직접 클릭해서 플로우를 따라가는 식으로 기록하고 기록한 테스트를 그대로 실행할 수 있다.

그래서 파이썬 코드를 직접 짜는 방식으로 만들지 않아도 되어서 테스트 제작이 빠르다.<br>
그리고 저장된 json 테스트 기록을 로드해서 테스트하므로 에디터와 기기에서 바로 테스트 플레이를 해볼 수 있다.

## 설치
이 모듈의 최신 버전을 Packages/manifest.json에 추가

## 사용법

### 테스트 기록하기
1. 유니티 메뉴의 P1SModule/TestRecorder/에디터 윈도우 열기 메뉴를 선택해서 에디터 윈도우를 띄워둔다.
2. 유니티 에디터로 게임을 실행한다.
3. 에디터 윈도우에서 Writer 탭에 들어가 테스트 이름을 적고 기록 시작을 누른다.
4. 실제 테스트 플로우대로 이것저것 테스트를 진행한다.(사전지정한 함수를 실행할 수도 있다.)
5. 기록 중지 버튼을 눌러서 기록을 저장한다.

### 기록한 테스트 실행하기
1. 유니티 에디터로 게임을 실행한다.
2. 에디터 윈도우의 Player 탭이 들어가면 기록한 테스트 목록이 나타날 것이다.
3. 테스트 목록들 중 실행하고 싶은 테스트를 클릭하면 테스트가 실행된다.
4. 테스트가 종료되면 Report가 저장된다.

### 사전지정 함수를 추가하는 방법
사전지정 함수를 사용해 원하는 기능을 테스트 중 실행하거나 테스트 중 진행을 검증할 수 있습니다.<br>
사전지정 함수는 `PreDesignedFunc` 어트리뷰트를 붙여서 만들 수 있습니다.

```c#
// 사전지정 함수 예시. 함수는 항상 public static 함수여야 합니다.

// 이 사전지정 함수는 레벨이 모두 클리어된 상태를 지정합니다.
[PreDesignedFunc("Set Level All Clear")]
public static void SetLevelAllClear() {
    LevelManager.AllClear();
}

// 이 사전지정 함수는 현재 코인의 개수가 200개인지 검증합니다.
// 200개가 아니면 failReason과 함께 Fail 리포트를 남깁니다. 테스트는 Fail로 종료됩니다.
// 200개면 테스트를 계속 진행합니다.
[PreDesignedFunc("Assert Coin Count", "코인의 개수가 200개인지 검증합니다.")]
public static StepReport AssertCoinCount() {
    if (Wallet.GetItemCount("coin" != 200) {
        return new StepReport(StepResult.Fail, "코인의 개수가 200개여야 합니다");
    
    return StepReport.Pass;
}

// 사전지정 함수는 비동기 함수로도 제작할 수 있습니다.
[PreDesignedFunc("StartSolver", "현재 플레이페이지에서 솔버 시작. 클리어 실패시 Fail을 반환합니다.")]
public static async UniTask<StepReport> StartSolver() {
    await new SolverHandler().StartSolverAsync();
    await UniTask.WaitUntil(() => PopupManager.CurrentPopup != null);
    
    if (PopupManager.CurrentPopup is ClearPopup) {
        return new StepReport(StepResult.Pass, $"Level {CurrentGameData.LevelIndex} Cleared!");
    
    return new StepReport(StepResult.Fail, "레벨 클리어에 실패했습니다.");
}
```

### 기록 된 테스트 예시
테스트 이름을 `GamePlayTest_1`로, 설명을 `1, 2, 3레벨 진입 후 클리어`로 지정하고 기록한 테스트입니다.<br>
메인신 > 레디팝업 > 플레이페이지 1레벨 진입 > 튜토리얼 닫기 > 솔버 시작 > 클리어팝업 > 2레벨 진입 > ... <br>
이런 테스트 플로우를 대략적으로 확인할 수 있습니다.

```json
{
  "testName": "GamePlayTest_1",
  "testDescription": "1, 2, 3레벨 진입 후 클리어",
  "steps": [
    {
      "second": 1.72841644,
      "gameObjectPath": "PageParent/LevelPage/Canvas/TabSwiperMainScene/level/SafeAreaRoot/PlayButton",
      "preDesignedFunc": null,
      "StepType": 0
    },
    {
      "second": 0.779377,
      "gameObjectPath": "Popup Parent/ReadyPopup/Canvas/PopupBackground/PlayButtonsHolder/Normal/ButtonRoot/buttonPivot/Play",
      "preDesignedFunc": null,
      "StepType": 0
    },
    {
      "second": 4.43042755,
      "gameObjectPath": "Popup Parent/TutorialPopup/Canvas/PopupAnimationRoot/CharacterDescriptionHolder/ContinueButton",
      "preDesignedFunc": null,
      "StepType": 0
    },
    {
      "second": 2.02885818,
      "gameObjectPath": null,
      "preDesignedFunc": "StartSolver",
      "StepType": 1
    },
    {
      "second": 2.33870316,
      "gameObjectPath": "Popup Parent/ClearPopup/Canvas/PopupBackground/Button_Root/Button_NoSlice",
      "preDesignedFunc": null,
      "StepType": 0
    },
    {
      "second": 3.27897263,
      "gameObjectPath": null,
      "preDesignedFunc": "StartSolver",
      "StepType": 1
    },
    {
      "second": 2.15751648,
      "gameObjectPath": "Popup Parent/ClearPopup/Canvas/PopupBackground/Button_Root/Button_NoSlice",
      "preDesignedFunc": null,
      "StepType": 0
    },
    {
      "second": 3.99859619,
      "gameObjectPath": "Popup Parent/TutorialPopup/Canvas/PopupAnimationRoot/CharacterDescriptionHolder/ContinueButton",
      "preDesignedFunc": null,
      "StepType": 0
    },
    {
      "second": 1.24151611,
      "gameObjectPath": null,
      "preDesignedFunc": "StartSolver",
      "StepType": 1
    }
  ]
}
```

### 테스트 실행 결과 예시
테스트가 진행된 내용을 알아볼 수 있습니다.<br>
어떠한 오브젝트를 클릭했는지, 어떤 사전지정 함수를 실행했는지를 알 수 있고<br>
각 스텝의 결과를 확인할 수 있습니다.
```text
테스트 이름 : GamePlayTest_1
테스트 설명 : 1, 2, 3레벨 진입 후 클리어
테스트 결과 : Pass
테스트 진행과정 :
	Execute >> Click PageParent/LevelPage/Canvas/TabSwiperMainScene/level/SafeAreaRoot/PlayButton
		Pass
	Execute >> Click Popup Parent/ReadyPopup/Canvas/PopupBackground/PlayButtonsHolder/Normal/ButtonRoot/buttonPivot/Play
		Pass
	Execute >> Click Popup Parent/TutorialPopup/Canvas/PopupAnimationRoot/CharacterDescriptionHolder/ContinueButton
		Pass
	Execute >> Func StartSolver
		Pass :: Level 1 Cleared!
	Execute >> Click Popup Parent/ClearPopup/Canvas/PopupBackground/Button_Root/Button_NoSlice
		Pass
	Execute >> Func StartSolver
		Pass :: Level 2 Cleared!
	Execute >> Click Popup Parent/ClearPopup/Canvas/PopupBackground/Button_Root/Button_NoSlice
		Pass
	Execute >> Click Popup Parent/TutorialPopup/Canvas/PopupAnimationRoot/CharacterDescriptionHolder/ContinueButton
		Pass
	Execute >> Func StartSolver
		Pass :: Level 3 Cleared!

```