using UnityEngine;

public class WaterBlock : Block
{
    WaterFlow waterFlow;

    protected override void Awake()
    {
        waterFlow = GetComponentInChildren<WaterFlow>();
    }
    void OnEnable()
    {
        waterFlow.enabled = false;
    }
}
