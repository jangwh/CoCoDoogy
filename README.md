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

2.1 맵 에디터 UI / 블록 선택 시스템

📌 BlockCategoryUI

* 맵 에디터에서 카테고리 기반 블록 선택 UI 및 단축키 입력을 관리하는 클래스

🔗 Class

[BlockCategoryUI.cs](mapeditor/Assets/Scripts/UI/BlockCategoryUI.cs)

역할

* 블록 / 동물 / 기믹 등 카테고리별 블록 목록 UI 관리

* 페이지네이션 기반 블록 표시 (9개 단위)

* 키보드 단축키(F1~ / 1~9 / Tab)를 통한 빠른 블록 선택 지원

* EditorManager와 연동하여 블록 선택 및 에디터 모드 전환 처리

주요 기능 정리

* 카테고리 버튼 자동 생성

* 블록 버튼 동적 생성 및 제거

* 숫자 키 표시 UI(TextMeshPro) 연동


주요 메서드

* ShowBlocks(BlockListData category)
→ 선택한 카테고리의 블록 목록을 현재 페이지 기준으로 표시

* CreateCategoryButtons()
→ BlockListData를 기반으로 카테고리 버튼 동적 생성

* HandleCategoryHotkeys()
→ F1 ~ Fn : 카테고리 전환
→ ESC : 블록 선택 해제 및 에디터 Idle 전환

* HandleBlockHotkeys()
→ 숫자키 1 ~ 9를 이용한 블록 즉시 선택
→ EditorMode가 Place일 때만 동작

* HandlePageChange()
→ Tab 키 입력 시 페이지 순환 처리

기술적 포인트

* UI 생성/삭제를 코드로 통제하여 에디터 확장성 확보

* 키보드 단축키 중심 UX로 맵 제작 속도 향상

2.2 블록 생성 및 식별 시스템


📌 BlockFactory

* 블록 데이터 기반 프리팹 생성 및 식별 컴포넌트 구성 담당 클래스

🔗 Class

[BlockFactory.cs](mapeditor/Assets/Scripts/BlockFactory.cs)

역할

* ScriptableObject(BlockData) 기반 블록 생성 책임 집중

* 블록 타입에 따른 Identity 컴포넌트 자동 부착

* 세이브 데이터 기반 블록 속성 초기화 처리

* 에디터 / 인게임 공통 생성 로직 제공

주요 기능 정리

* 블록 이름 또는 BlockData 기반 생성 지원

* 블록 타입에 따른 Identity 클래스 분기 처리

* 블록 프리팹 탐색 기능 제공

주요 메서드

* CreateBlock(string blockName, Vector3Int position, Quaternion rotation, BlockSaveData saveData = null)
→ 블록 이름 기반으로 BlockData 탐색 후 생성

* CreateBlock(BlockData data, Vector3Int position, Quaternion rotation, BlockSaveData saveData = null)
→ 실제 블록 생성 핵심 메서드
→ 프리팹 Instantiate
→ BlockType에 따른 Identity 컴포넌트 자동 부착
→ SaveData 존재 시 속성 복원 처리

* FindBlockPrefab(string blockName)
→ 블록 이름 기반 프리팹 조회 (생성 없이 참조 목적)

기술적 포인트

* 팩토리 패턴 적용으로 생성 책임 단일화

* 프리팹에 Identity가 없을 경우 자동 보정 처리

* 맵 저장/로드 시스템과 자연스럽게 연동 가능

2.2 코코두기 데이터 관리
* 구글 스프레드 시트의 주소를 받아 데이터를 S/O화
* 데이터매니저의 허브화

2.3 씬연결
* 아웃게임-인게임-아웃게임 전환시의 처리
* 보물 획득 여부에 따른 추가처리


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
