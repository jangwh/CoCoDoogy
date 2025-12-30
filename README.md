# CoCoDoogy
![Image](https://github.com/user-attachments/assets/03417ae4-b922-4f4c-9975-6c50247d507d)

#  CoCoDoogy
CoCoDoogy [기업협약 프로젝트] 

[코코두기 전체 플레이 영상](https://youtu.be/WJiAdnbbeG4)


[코코두기 맵 에디터 유튜브 영상](https://youtu.be/dpCA9EOCIJI)

## 1. 코코두기

기업협약 프로젝트, 소코반장르의 모바일 안드로이드 퍼즐게임입니다.

Unity를 활용하여 3D로 제작하였습니다

개발기간 : 2025.10.16 ~ 2025.12.10

## 2. 주요 기능

장우형 개발파트

### 2.1 맵 에디터 UI / 블록 선택 시스템

### [BlockCategoryUI.cs](mapeditor/Assets/Scripts/UI/BlockCategoryUI.cs)

* 맵 에디터에서 카테고리 기반 블록 선택 UI 및 단축키 입력을 관리하는 클래스


#### 💡 역할

* 블록 / 동물 / 기믹 등 카테고리별 블록 목록 UI 관리

* 키보드 단축키(F1~ / 1~9 / Tab)를 통한 빠른 블록 선택 지원
#### 📌 주요 메서드

* ShowBlocks(BlockListData category)
→ 선택한 카테고리의 블록 목록을 현재 페이지 기준으로 표시

* CreateCategoryButtons()
→ BlockListData를 기반으로 카테고리 버튼 생성

* HandleCategoryHotkeys()
→ F1 ~ Fn : 카테고리 전환
→ ESC : 블록 선택 해제 및 에디터 Idle 전환

* HandleBlockHotkeys()
→ 숫자키 1 ~ 9를 이용한 블록 즉시 선택
→ EditorMode가 Place일 때만 동작

* HandlePageChange()
→ Tab 키 입력 시 페이지 순환 처리

#### 기술적 포인트

* UI 생성/삭제를 코드로 통제하여 에디터 확장성 확보

* 키보드 단축키 중심 UX로 맵 제작 속도 향상

### 2.2 블록 생성 및 식별 시스템

 
### [BlockFactory.cs](mapeditor/Assets/Scripts/BlockFactory.cs)

* 블록 데이터 기반 프리팹 생성 및 식별 컴포넌트 구성 담당 클래스

#### 💡 역할

* ScriptableObject(BlockData) 기반 블록 생성 책임 집중

* 블록 타입에 따른 Identity 컴포넌트 자동 부착

* 세이브 데이터 기반 블록 속성 초기화 처리

* 에디터 / 인게임 공통 생성 로직 제공

#### 📌 주요 메서드

* CreateBlock(string blockName, Vector3Int position, Quaternion rotation, BlockSaveData saveData = null)
→ 블록 이름 기반으로 BlockData 탐색 후 생성

* CreateBlock(BlockData data, Vector3Int position, Quaternion rotation, BlockSaveData saveData = null)
→ 실제 블록 생성 핵심 메서드
→ 프리팹 Instantiate
→ BlockType에 따른 Identity 컴포넌트 자동 부착
→ SaveData 존재 시 속성 복원 처리

* FindBlockPrefab(string blockName)
→ 블록 이름 기반 프리팹 조회 (생성 없이 참조 목적)

#### 기술적 포인트

* 팩토리 패턴 적용으로 생성 책임 단일화

* 프리팹에 Identity가 없을 경우 자동 보정 처리

* 맵 저장/로드 시스템과 자연스럽게 연동 가능

### 2.3 코코두기 데이터 관리 시스템

* 구글 스프레드시트 기반 데이터 파이프라인 구축 및 DataManager 허브화

#### 📌 개요

* CoCoDoogy는 구글 스프레드시트를 단일 데이터 원본으로 사용하며,
에디터 단계에서 CSV를 자동 다운로드한 뒤 ScriptableObject(S/O)로 변환하고,
런타임에서는 DataManager를 중심으로 모든 데이터 접근을 통합하는 구조를 사용합니다.

### 2.3.1 CSV → ScriptableObject 자동 변환 파이프라인

### [MetaJsonGenerator.cs](Assets/_Proj/Scripts/Editor/Tools/MetaJsonGenerator.cs)

* 데이터 테이블 메타 정보를 기반으로 table_meta.json을 자동 생성하는 에디터 툴

#### 💡 역할

* 마스터 구글 스프레드시트로부터 테이블 목록 CSV 다운로드

* 테이블 이름 / 타입 / CSV URL 정보를 추출

* CSV Import 단계에서 사용되는 메타 JSON(table_meta.json) 자동 생성

#### 📌 주요 메서드

* GenerateMetaJson()
→ 마스터 시트 CSV 다운로드
→ 테이블 메타 정보 파싱
→ TableMetaList 구조로 JSON 파일 생성



### [CsvImportManager.cs](Assets/_Proj/Scripts/Editor/Tools/CsvImportManager.cs)

* 메타 JSON을 기반으로 CSV를 다운로드하고 ScriptableObject로 변환하는 에디터 툴
  
#### 💡 역할

* table_meta.json 기반 전체 데이터 테이블 일괄 처리

* 구글 스프레드시트 CSV 자동 다운로드

* 테이블 타입에 따라 CSV 저장 또는 ScriptableObject 생성 수행

#### 📌 주요 메서드

* DownloadAndImport() : 메타 JSON 로드, 각 테이블 CSV 다운로드, 타입에 따라 Import 분기

* ImportAllTables(string name, string type, string csv) : 테이블 이름 기준 Parser 클래스 호출, CSV → S/O 변환

* DownloadCSV(string url) : CSV 원격 다운로드 처리


### 2.3.2 DataManager 허브화 구조

### [DataManager.cs](Assets/_Proj/Scripts/Data/DataTable/DataCore/DataManager.cs)

* 프로젝트 전체 데이터 접근을 단일 진입점으로 통합한 데이터 허브


#### 💡 역할

* 모든 데이터 Provider를 중앙에서 초기화 및 관리

* 씬 전환과 무관하게 유지되는 글로벌 데이터 접근 포인트 제공

* 인게임 시스템에서 직접 S/O 접근을 차단하고 Provider를 통해서만 접근하도록 설계

#### 구조적 특징

* Singleton + DontDestroyOnLoad

* 데이터 테이블별 Provider 명확히 분리

* DataRegistry를 통해 S/O 참조 집합 관리


### [ResourcesLoader.cs](Assets/_Proj/Scripts/Data/DataTable/DataCore/ResourcesLoader.cs)

* 리소스 로딩 책임을 분리하기 위한 Loader 클래스

#### 💡 역할

* Resources.Load 호출을 한 곳으로 캡슐화

* Provider가 구체적인 로딩 방식에 의존하지 않도록 분리

### 2.4 씬 연결 (OutGame ↔ InGame ↔ OutGame)

* 스테이지 진입부터 클리어 후 메인 복귀까지의 씬 전환 흐름과
보물(별) 획득 여부에 따른 추가 처리 로직을 정리한다.

### 2.4.1 아웃게임 → 인게임 전환 처리

### [StageManager.cs](Assets/_Proj/Scripts/Stage/StageManager.cs)

* StageManager는 인게임 진입의 총괄 컨트롤러로서 다음 책임을 가진다.

#### 📌 주요 메서드 및 기능

🔄 인게임 초기화 시퀀스

1️⃣ 스테이지 데이터 확보

* 스테이지 로딩 시
→ FirebaseManager.Instance.currentMapData

2️⃣ 블록 팩토리를 통한 스테이지 구성
* LoadStage(currentMapData) : BlockFactory를 통해 맵 블록을 생성
* InspectBlocks() : 그리드 기준으로 블록 등록 (blockDictionary)
* LinkSignals() : 시그널 블록 간 연결 처리

3️⃣ 연출 처리 (페이드 → 컷씬 → BGM → 카메라)

* 페이드 인/아웃

* 시작 컷씬 재생 (존재 시)

* 스테이지 BGM 재생

* 카메라 워킹 연출

4️⃣ 플레이어 생성 및 다이얼로그 시작

* SpawnPlayer() : 시작 지점에 플레이어 생성, 다이얼로그 존재 시 입력 잠금 및 진행 제어


### 2.4.2 인게임 중 상호작용 제어

### [DialogueManager.cs](Assets/_Proj/Scripts/Stage/DialogueManager.cs)

#### 💡 역할

* 다이얼로그 출력 및 진행 관리

* 터치 입력 기반 다음 대사 처리

* 게임플레이 입력 잠금 / 해제

### 2.4.3 보물(Treasure) 획득 플로우 및 인게임 처리

* 인게임 스테이지 내에서 보물 오브젝트와 상호작용할 때의
플레이 흐름 제어, UI 전환, 진행도 반영 과정을 담당하는 시스템

### [Treasure.cs](Assets/_Proj/Scripts/Stage/Block/Treasure.cs)

* Treasure는 스테이지에 배치되는 상호작용 오브젝트로,
플레이어 충돌을 기점으로 다음을 처리한다.

* 보물 획득 여부 판별

* 보물 타입에 따른 UI 출력

* 플레이어 입력 및 이동 제어

* 스테이지 내 보물 획득 상태(StageManager) 반영


### 2.4.4 인게임 → 아웃게임 전환 (스테이지 클리어)

#### 스테이지 클리어 진입

* EndBlock → StageManager.ClearStage()

* 도착 블록(EndBlock)이 StageManager에 클리어 이벤트 전달

🔄 클리어 처리 시퀀스

1️⃣ 엔드 다이얼로그 처리

2️⃣ 결과 UI 출력

* OnTreasureCollected(int index) : 보물 획득 상태 배열 업데이트, 별 개수 계산

* ShowResultUI() : 결과 패널 활성화, 별(보물) 수 계산 및 UI 반영

### 2.4.5 보물 획득 여부에 따른 추가 처리

#### 보물(별) 시스템 개요

* 스테이지당 최대 3개의 보물

* StageManager.collectedTreasures[3] 로 관리

* 이전 기록보다 더 많이 수집했을 때만 갱신

### [PlayerProgressManager.cs](Assets/_Proj/Scripts/Data/PlayerProgressManager.cs)

#### 💡 역할

* 스테이지별 보물 획득 상태 저장

* 최고 기록 유지

* 중복 보상 방지

#### 📌 주요 메서드

* GetStageProgress(string stageId) : 스테이지별 진행 데이터 조회 / 생성

* UpdateStageTreasure(string stageId, bool[] newlyCollected) : 기존 기록보다 더 많은 별을 획득했을 때만 갱신

* SaveProgress() : UserData(Local) 기반 저장

## 3. 플로우 차트 및 클래스 다이어그램

3.1 플로우차트
<img width="1233" height="596" alt="Image" src="https://github.com/user-attachments/assets/a37e5675-90dd-47fa-8e8c-02c54524466e" />

--------------------------------------------------------------------------------------------------------------------
3.2 클래스 다이어그램

<img width="677" height="470" alt="Image" src="https://github.com/user-attachments/assets/b037eb25-7b73-4782-8d3d-305c4a46a9a0" />

--------------------------------------------------------------------------------------------------------------------
<img width="682" height="465" alt="Image" src="https://github.com/user-attachments/assets/0880ddff-8f27-482f-886a-ddd50b38df39" />

--------------------------------------------------------------------------------------------------------------------
<img width="692" height="443" alt="Image" src="https://github.com/user-attachments/assets/3094c32d-6d40-4eb0-9a9b-ac29f25ff2c7" />

--------------------------------------------------------------------------------------------------------------------
<img width="970" height="546" alt="Image" src="https://github.com/user-attachments/assets/a67efe66-9b33-4281-9e5f-e1ffeb2a54af" />

--------------------------------------------------------------------------------------------------------------------
<img width="834" height="485" alt="Image" src="https://github.com/user-attachments/assets/80a88277-7011-4bd2-bd08-99ba3bd0fe3a" />

--------------------------------------------------------------------------------------------------------------------
   
## 4. 기술 스택
   
* C#
* Unity
* Fork + Github(형상 관리)
  
기술파트
* BlockFactory를 이용해 블록생성 관리
* S/O 데이터 관리
* DataManager를 이용해 데이터 허브화
* Firebase와 연동하여 보물 획득 여부 저장
