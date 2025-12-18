using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Data/BlockData")]
public class BlockData : ScriptableObject
{
    public BlockType blockType;
    public string blockName;
    public Sprite icon;      // UI에 표시할 이미지
    public GameObject prefab; // 실제 배치될 프리팹
}