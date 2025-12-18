using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;


public class MapUIManager : MonoBehaviour
{
    [Header("References")]
    public MapSaver mapSaver;
    public EditorManager blockPlacer;
    public List<BlockListData> allBlocks;

    [Header("UI Elements")]
    public TMP_InputField saveInputField;
    public Button saveButton;
    public Button showLoadListButton;
    public Transform loadListParent;
    public GameObject loadButtonPrefab;

    public TextMeshProUGUI messageText;
    
    void Start()
    {
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        showLoadListButton.onClick.AddListener(OnShowLoadListFromFirebase);
        StartCoroutine(AutoFetchCoroutine());

    }
    IEnumerator AutoFetchCoroutine()
    {
        yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized);
        OnShowLoadListFromFirebase();
    }
    

    void OnSaveButtonClicked()
    {
        string name = saveInputField.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("맵 이름을 입력하세요!");
            saveInputField.placeholder.GetComponent<TextMeshProUGUI>().text = @"<color=red>Input MapName</color=red>";
            return;
        }

        mapSaver.SaveMap(blockPlacer.GetPlacedBlocks(), name, true, ShowMessage);
    }
    
    async void OnShowLoadListFromFirebase()
    {
        foreach (Transform child in loadListParent)
            Destroy(child.gameObject);

        List<string> maps = await FirebaseManager.Instance.FetchMapNamesFromFirebase();

        foreach (string mapName in maps)
        {
            GameObject btnObj = Instantiate(loadButtonPrefab, loadListParent);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = mapName;

            btnObj.GetComponent<Button>().onClick.AddListener(async () =>
            {
                var loaded = await mapSaver.LoadMapFromFirebase(mapName, allBlocks, ShowMessage);
                if (loaded == null) return;
                blockPlacer.SetPlacedBlocks(loaded);
            });
        }
    }

    //삭제하지 않고 남겨둡니다: 로컬의 데이터패스에서 저장된 맵 리스트를 가져오는 함수.
    void OnShowLoadList()
    {
        foreach (Transform child in loadListParent)
            Destroy(child.gameObject);

        string[] maps = mapSaver.GetSavedMaps();

        foreach (string mapName in maps)
        {
            GameObject btnObj = Instantiate(loadButtonPrefab, loadListParent);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = mapName;

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                var loaded = mapSaver.LoadMap(mapName, allBlocks);
                blockPlacer.SetPlacedBlocks(loaded);
            });
        }
    }




    public IEnumerator MessageTextCoroutine(string message)
    {
        WaitForSeconds wait = new(1.5f);
        messageText.text = message;
        messageText.alpha = 1;
        yield return wait;
        while (true)
        {
            messageText.alpha -= .05f;
            if (Mathf.Approximately(messageText.alpha, 0))
            {
                messageText.gameObject.SetActive(false); yield break;
            }
            yield return null;
        }

    }
    public void ShowMessage(string message)
    {
        if (!messageText.gameObject.activeSelf) messageText.gameObject.SetActive(true);
        messageText.StopAllCoroutines();
        messageText.StartCoroutine(MessageTextCoroutine(message));
        if (message.Contains("successfully")) saveInputField.text = "";
        if (message.Contains("Found!")) saveInputField.text = "";

    }
}