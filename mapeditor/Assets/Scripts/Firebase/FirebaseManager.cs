using Firebase;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class FirebaseManager : MonoBehaviour
{

    public static FirebaseManager Instance { get; private set; }
    private FirebaseApp App { get; set; }
    private FirebaseDatabase DB { get; set; }
    private DatabaseReference MapDataRef => DB.RootReference.Child($"mapData");
    private DatabaseReference MapMetaRef => DB.RootReference.Child($"mapMeta");

    public bool IsInitialized { get; private set; }
    async void Start()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        var options = new AppOptions()
        {
            ApiKey = "",
            DatabaseUrl = new("https://doogymapeditor-default-rtdb.asia-southeast1.firebasedatabase.app/"),
            ProjectId = "doogymapeditor",
            StorageBucket = "doogymapeditor.firebasestorage.app",
            MessageSenderId = "236130748269",
            AppId = "1:236130748269:web:34a94137f83bef839dfc64"
        };

        App = FirebaseApp.Create(options);

        if (status == DependencyStatus.Available)
        {

            //초기화 성공
            Debug.Log($"파이어베이스 초기화 성공");
            DB = FirebaseDatabase.GetInstance(App);
            DB.SetPersistenceEnabled(false);
            IsInitialized = true;
        }
        else
        {
            Debug.LogWarning($"파이어베이스 초기화 실패, 파이어베이스 앱 상태: {status}");
        }
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 파이어베이스에 맵을 저장하는 함수
    /// </summary>
    /// <param name="mapName">저장할 맵 이름</param>
    /// <param name="">저장할 맵 데이터 객체</param>
    public async void SaveMapToFirebase(string mapName, MapData data, Action<string> callback = null)
    {
        string json = JsonUtility.ToJson(data);
        try
        {
            callback?.Invoke($"Saving {mapName} to Firebase DB...");
            await MapDataRef.Child(mapName).SetRawJsonValueAsync(json);
            await MapMetaRef.Child("maps").Child(mapName).SetRawJsonValueAsync(data.blocks.Count.ToString());
            callback?.Invoke($"{mapName} has been successfully saved to Firebase DB!");
        }
        catch (FirebaseException fe)
        {
            callback?.Invoke(fe.Message);
            Debug.LogError(fe.Message);
        }
    }

    public async Task<List<string>> FetchMapNamesFromFirebase()
    {

        List<string> allMaps = new();
        try
        {
            var snapshot = await MapMetaRef.Child("maps").GetValueAsync();
            if (snapshot.Exists)
            {
                foreach (var map in snapshot.Children)
                {
                    allMaps.Add(map.Key);
                }

            }
            return allMaps;
        }
        catch (FirebaseException fe)
        {
            Debug.LogError(fe.Message);
            return null;
        }

    }

    public async Task<MapData> LoadMapFromFirebase(string mapName, Action<string> callback = null)
    {
        try
        {
            callback?.Invoke($"Looking for mapdata from DB by {mapName}...");
            var snapshot = await MapDataRef.Child(mapName).GetValueAsync();
            if (snapshot.Exists)
            {
                callback?.Invoke($"{mapName} data Found!");
                MapData data = JsonUtility.FromJson<MapData>(snapshot.GetRawJsonValue());
                return data;
            }
            else
            {
                throw new Exception("No such map data exists.");
            }
            
        }
        catch (FirebaseException fe)
        {

            callback?.Invoke(fe.Message);
            Debug.LogError(fe.Message);
            return null;
        } catch (Exception ee)
        {
            callback?.Invoke(ee.Message);
            Debug.LogError(ee.Message);
            return null;
        }
    }   
}
