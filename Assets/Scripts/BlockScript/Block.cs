using UnityEngine;

public abstract class Block : MonoBehaviour
{
    //enum 타입에 따라 기능 구현
    //추상화 클래스 활?용
    //인터페이스 활?용

    protected BlockIdentity blockid;

    protected virtual void Awake()
    {
        blockid = GetComponent<BlockIdentity>();
    }
}
