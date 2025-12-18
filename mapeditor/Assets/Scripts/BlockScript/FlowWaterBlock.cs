using UnityEngine;

public class FlowWaterBlock : MonoBehaviour
{
    WaterFlow waterFlow;


    void Awake()
    {
        waterFlow = GetComponentInChildren<WaterFlow>();
    }


    void Start()
    {
        waterFlow.SetFlowDirection();
    }

    void OnDestroy()
    {
        waterFlow.SetFlowDirection();
    }
    


    
}
