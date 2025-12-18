using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    
    bool isMoved;

    MoveCommand cmd;
    void Awake()
    {
        //이게 필요한 이유: 모종의 이유로 드래그 취소 판정 시에 원래 위치로 돌아가기 위함임.
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //드래그를 시작할 때 해야 할 일
        //이동 커맨드 생성? 아닌거같음. 엔드드래그에서 생성하고 바로 수행시켜도 아무 문제 없을 것임.
        cmd = new(gameObject);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //드래그하는 동안 해야 할 일
        //기본적으로는 마우스를 따라다님. => 카메라에서 마우스로 레이를 쏴가지고 레이캐스트 맞는 대상을 검출.
        gameObject.layer = 0;
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, EditorManager.Instance.gridLayer))
        {
            

            // 클릭한 표면의 법선 방향으로 한 칸 위에 기즈모 표시
            Vector3 targetPos = hit.point + hit.normal * (EditorManager.Instance.gridSize * 0.5f);
            Vector3 placementPos = EditorManager.Instance.SnapToGrid(targetPos);

            // 설치 가능 여부 판단
            Vector3Int gridPos = Vector3Int.RoundToInt(placementPos);

            bool canPlace = !EditorManager.Instance.placedBlocks.ContainsKey(gridPos);


            
            transform.position = gridPos;
            cmd.movePosition = gridPos;
            SetPreviewColor(canPlace);
            isMoved = canPlace && (cmd.Position != cmd.movePosition);
        }
        else
        {
            //TODO: 이게뭐ㅏ야
            transform.position = cmd.Position;
            isMoved = false;
        }
        
    }

    //Debug.Log(hits.Length);
    //if (hits.Length == 0) return;

    //1. 기본적으로 마우스를 따라다니도록 함.



    //// 거리가 가까운 순으로 정렬
    //System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

    //RaycastHit? hitResult = null;

    //foreach (var hit in hits)
    //{
    //    if (gameObject != null && hit.collider.gameObject == gameObject)
    //        continue; // 자기 자신은 무시
    //    hitResult = hit;
    //    break;
    //}

    //if (!hitResult.HasValue)
    //    return;

    //배치가 가능한 곳일 경우 스냅이 걸림.

//}

    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그를 끝마칠 때 해야 할 일
        //이동 커맨드 수행과 그에 맞는 처리.
        gameObject.layer = LayerMask.NameToLayer("Ground");
        if (isMoved)
        {
            ExecuteCommand();
            SetPreviewColor(true);
        }
        else
        {
            transform.position = cmd.Position;
            cmd = null;

            SetPreviewColor(true);
        }
            EditorManager.Instance.SetIndicator(true);

    }
    void SetPreviewColor(bool canPlace)
    {
        if (gameObject == null) return;

        Color color = canPlace ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
        foreach (var rend in gameObject.GetComponentsInChildren<Renderer>())
            rend.material.color = color;
    }

    private void ExecuteCommand()
    {
        CommandManager.Instance.AddCommand(cmd);
        cmd.Execute();
    }

    void OnDestroy()
    {
        //transform.position = isMoved ? transform.position : originalPos;
        
    }

}
