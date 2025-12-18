using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class SwitchBlock : Block, ISignalSender
{
    //스위치 기능 구현
    //다른 물체(코코두기, 나무상자 등등)에 닿으면 스위치가 움직임
    //스위치가 움직이면 연결된 오브젝트에 변화가 생겨야 함
    //bool 받아서 true면 문 열리고 아니면 닫히게

    ISignalReceiver receiver;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        LinkRecevier();
    }
    void Update()
    {
        //Todo: 에디터에서만 이렇게 설정
        //이후 실제 게임에서는 blockfactory에서 createblock 함수에서 설정
        LinkRecevier();
    }
    public void LinkRecevier()
    {
        // 트리거 아이덴티티가 존재하고 link 정보가 유효할 때만
        if (TryGetComponent(out TriggerIdentity triggerId))
        {
            var prop = triggerId.property;
            Debug.Log($"{name}: linkPos = {prop.linkedPos}");

            if (prop != null)
            {
                if (prop.linkedPos != Vector3Int.one * int.MaxValue)
                {
                    // 연결된 오브젝트 탐색 (좌표 기반)
                    Collider[] hits = Physics.OverlapSphere(prop.linkedPos, 1.0f);
                    Debug.Log($"{name}: linkPos={prop.linkedPos} / 감지된 {hits.Length}개 오브젝트");

                    foreach (var hit in hits)
                    {
                        Debug.Log($"  hit: {hit.name}");
                        if (hit.TryGetComponent(out ISignalReceiver recv))
                        {
                            receiver = recv;
                            Debug.Log($"{name}: {recv} 연결 성공!");
                            break;
                        }
                    }
                }
            }
        }

        if (receiver == null)
        {
            Debug.LogWarning($"{name}:연결된 수신기가 없!");
        }
    }

    public void OnSignalSend()
    {
        if (receiver != null)
            receiver.OnSignalReceive();
        else
            Debug.LogWarning($"{name}: 연결된 수신기가 없습니다!");
    }
    
    // 콜라이더가 눌리면 신호 보냄
    private void OnTriggerEnter(Collider other)
    {
        // 예: 코코두기, 박스 등 무언가 닿으면 작동
        if (other.CompareTag("Animal"))
        {
            OnSignalSend();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 예: 코코두기, 박스 등 무언가 닿으면 작동
        if (other.CompareTag("Animal"))
        {
            OnSignalSend();
        }
    }
}
