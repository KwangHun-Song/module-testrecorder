# TestRecorder

## 개요
테스트 자동화를 도와주는 클래스로, 유니티 에디터에서 하는 행동을 기억해서 데이터로 저장하고 저장된 데이터를 읽어서 그대로 실행한다.

## 설치
모듈의 최신 버전을 manifest.json에 추가한다.

## 사용법(인터페이스)

### 기록하기
1. F5키를 누르면 기록이 시작된다. 원하는 곳들을 클릭해서 테스트를 수행한다.
2. F6키를 누르면 기록이 종료되어 파일로 저장된다. 저장된 파일은 Assets/Resources/TestRecords.json 파일에 저장되니 확인해볼 수 있다.

### 기록된 테스트 실행하기
1. F7키를 누르면 Assets/Resources/TestRecords.json 파일을 읽어서 테스트를 시작한다.
2. 테스트가 끝나면 리포트가 저장된다.(기본적으로 리포트가 콘솔로그에 출력된다. 다른 방법으로 리포트 출력을 원하면 IPlayerEvent 인터페이스를 구현하면 된다.)
3. F8키를 누르면 테스트가 중단된다.
