using System.Collections.Generic;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public static BlockFactory Instance;

    [Header("등록된 블록 데이터")]
    public List<BlockData> allBlocks = new List<BlockData>();

    void Awake() => Instance = this;

    public GameObject CreateBlock(string blockName, Vector3Int position, Quaternion rotation, BlockSaveData saveData = null)
    {
        BlockData data = allBlocks.Find(x => x.blockName == blockName);
        if (data == null)
        {
            Debug.LogWarning($"BlockFactory: '{blockName}' 데이터를 찾을 수 없습니다.");
            return null;
        }
        return CreateBlock(data, position, rotation, saveData);
    }

    public GameObject CreateBlock(BlockData data, Vector3Int position, Quaternion rotation, BlockSaveData saveData = null)
    {
        GameObject obj = Instantiate(data.prefab, position, rotation);
        obj.layer = LayerMask.NameToLayer("Ground");

        var id = obj.GetComponent<BlockIdentity>();
        if (!id)
        {
            
            if (BlockType.Box < data.blockType && data.blockType <= BlockType.Tower)
                obj.AddComponent<TriggerIdentity>();
            else if (data.blockType == BlockType.Door)
                obj.AddComponent<GimikIdentity>();
            else
                obj.AddComponent<BlockIdentity>();



            id = obj.GetComponent<BlockIdentity>();
            id.blockType = data.blockType;

        }
        //프리팹에 BlockIdentity 혹은 자식 클래스를 이미 붙여둘 것이기 때문에 여기서 id가 할당됨.
        id.blockName = data.blockName;
        id.blockType = data.blockType;

        if (id is IOptionalProperty specialId)
        {
            specialId.property = saveData != null ? saveData.property : new();
            id = specialId as BlockIdentity;
            //스페셜Id의 property는 세이브데이터로 생성한 경우 세이브데이터로 가져오고, 아니면 새로 만듦.
        }
        
        return obj;
    }

    public GameObject FindBlockPrefab(string blockName)
    {
        BlockData data = allBlocks.Find(x => x.blockName == blockName);
        if (data == null)
        {
            Debug.LogWarning($"BlockFactory: '{blockName}' 데이터를 찾을 수 없습니다.");
            return null;
        }
        return data.prefab;
    }
}