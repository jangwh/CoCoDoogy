using UnityEngine;

[CreateAssetMenu(fileName = "BlockListInfo", menuName = "Data/BlockListInfo")]
public class BlockListData : ScriptableObject
{
    public string categoryName;
    public Sprite icon;
    public BlockData[] blocks;
}
