using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class MapSaver : MonoBehaviour
{
    private string FolderPath => Application.persistentDataPath + "/Maps";

    void Awake()
    {
        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);
    }
    public void NewMap()
    {
        //1. 에디터매니저의 placedBlocks 딕셔너리를 참조하여 모든 Value(GameObject) 파괴
        foreach (var obj in EditorManager.Instance.placedBlocks)
        {
            Destroy(obj.Value);
        }
        //TODO: 새 맵 만들기: 2. 장식물 등 배치 딕셔너리를 따로 추가하게 될 경우 이곳에 추가.

        EditorManager.Instance.placedBlocks = new(); //딕셔너리 초기화
        CommandManager.Instance.ClearAll(); //커맨드 목록 초기화
    }
    public void SaveMap(Dictionary<Vector3Int, GameObject> placedBlocks, string mapName, bool toFirebase = false, Action<string> callback = null)
    {
        MapData mapData = new MapData();

        foreach (var pair in placedBlocks)
        {
            
            var id = pair.Value.GetComponent<BlockIdentity>();
            if (id == null) continue;
            if (id is IOptionalProperty specialId)
            {
                mapData.blocks.Add(new BlockSaveData
                {
                    blockName = id.blockName,
                    blockType = id.blockType,
                    position = pair.Key,
                    rotation = pair.Value.transform.rotation,

                    property = specialId.property
                });
            }
            else
            {
                mapData.blocks.Add(new BlockSaveData
                {
                    blockName = id.blockName,
                    blockType = id.blockType,
                    position = pair.Key,
                    rotation = pair.Value.transform.rotation,


                });
            }
        }

        string json = JsonUtility.ToJson(mapData, true);

        if (!toFirebase)
        {
            string path = Path.Combine(FolderPath, mapName + ".json");
            File.WriteAllText(path, json);
            Debug.Log($"로컬에 맵 저장 완료: {path}");
        } else
        {
            FirebaseManager.Instance.SaveMapToFirebase(mapName, mapData, callback);
        }
    }

    public Dictionary<Vector3Int, GameObject> LoadMap(string mapName, List<BlockListData> allBlockData)
    {

        string path = Path.Combine(FolderPath, mapName + ".json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("해당 맵 없음: " + mapName);
            return new Dictionary<Vector3Int, GameObject>();
        }

        string json = File.ReadAllText(path);
        MapData mapData = JsonUtility.FromJson<MapData>(json);

        Dictionary<Vector3Int, GameObject> loadedBlocks = new Dictionary<Vector3Int, GameObject>();

        foreach (var block in mapData.blocks)
        {
            GameObject obj = BlockFactory.Instance.CreateBlock(block.blockName, block.position, block.rotation, block);
            loadedBlocks.Add(block.position, obj);
        }

        Debug.Log($"맵 로드 완료: {mapName} ({loadedBlocks.Count}개 블록)");
        //HACK: 강욱 - 1026: 맵을 불러오게 될 경우 기존의 커맨드는 모두 삭제하는 걸 기본으로 합니다. (남겨두면 똑바로 작동 안 할 가능성이 매우 높음)
        CommandManager.Instance?.ClearAll();
        return loadedBlocks;
    }

    public async Task<Dictionary<Vector3Int, GameObject>> LoadMapFromFirebase(string mapName, List<BlockListData> allBlockData, Action<string> callback = null)
    {

        MapData mapData = await FirebaseManager.Instance.LoadMapFromFirebase(mapName, callback);
        if (mapData == null) return null;
        Dictionary<Vector3Int, GameObject> loadedBlocks = new Dictionary<Vector3Int, GameObject>();

        foreach (var block in mapData.blocks)
        {
            GameObject obj = BlockFactory.Instance.CreateBlock(block.blockName, block.position, block.rotation, block);
            loadedBlocks.Add(block.position, obj);
        }

        Debug.Log($"맵 로드 완료: {mapName} ({loadedBlocks.Count}개 블록)");
        //HACK: 강욱 - 1026: 맵을 불러오게 될 경우 기존의 커맨드는 모두 삭제하는 걸 기본으로 합니다. (남겨두면 똑바로 작동 안 할 가능성이 매우 높음)
        CommandManager.Instance?.ClearAll();
        return loadedBlocks;
    }

    public string[] GetSavedMaps()
    {
        if (!Directory.Exists(FolderPath))
            return new string[0];

        string[] files = Directory.GetFiles(FolderPath, "*.json");
        for (int i = 0; i < files.Length; i++)
            files[i] = Path.GetFileNameWithoutExtension(files[i]);

        return files;
    }
}