using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DoorBlock : Block, ISignalReceiver
{
    //문 기능 구현
    //스위치가 움직이면 문이 열려야 한다.

    public GameObject Door;
    bool isOpen;


    protected override void Awake()
    {
        base.Awake();
    }

    public void OnSignalReceive()
    {
        // 문 상태 전환
        isOpen = !isOpen;

        // 위치로 표현 
        Door.transform.rotation = isOpen ? Quaternion.Euler(0, 90, 0): Quaternion.Euler(0, 0, 0);
    }
}
