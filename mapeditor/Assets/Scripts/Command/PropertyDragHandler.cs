using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class PropertyDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //IOptionalProperty를 상속한 identity 구체 클래스 객체
    private IOptionalProperty identity;
    //드래그 시작점
    private Vector3Int originalLinkedPos;


    PropertyChangeCommand cmd;
    bool isPropertyChanged;

    //라인렌더러는 PropertyDragHandler가 갖는 게 맞음.
    //드래그핸들러 객체가 있을 때만 같은 게임오브젝트가 가진 LinkableIdentity 정보에 맞추어 라인을 그려주는 처리가 필요.
    protected LineRenderer lineRenderer;
    

    void Awake()
    {
        //IOptionalProperty 나에게서 주입.
        identity = GetComponent<IOptionalProperty>();
        originalLinkedPos = identity.property.linkedPos;



        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.widthMultiplier = 0.1f;
            
        }

        UpdateLinkVisual(transform.position, originalLinkedPos);
    }

    public virtual void UpdateLinkVisual(Vector3 start, Vector3 end)
    {

        if (end == Vector3Int.one * int.MaxValue) end = transform.position;
        
        Vector3 middle = Mathf.Approximately(start.magnitude, end.magnitude) ? (transform.position + Vector3.up) : Vector3.Lerp(start, end, .5f) + Vector3.up;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, middle);
        lineRenderer.SetPosition(2, end);
        lineRenderer.enabled = true;

    }

    private Vector3 GridToWorld(Vector3Int gridPos)
    {
        // 그리드 좌표를 월드 좌표로 변환하는 로직
        return new Vector3(gridPos.x, gridPos.y, gridPos.z); // 예시
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cmd = new(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.layer = 0;
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, EditorManager.Instance.gridLayer))
        {


            

    


            cmd.changedProperty.linkedPos =
                hit.collider.gameObject.TryGetComponent<LinkableIdentity>(out LinkableIdentity linkable) ?
                Vector3Int.RoundToInt(linkable.transform.position) : Vector3Int.one * int.MaxValue;

            UpdateLinkVisual(transform.position, cmd.changedProperty.linkedPos);
            isPropertyChanged = cmd.changedProperty.linkedPos != originalLinkedPos;
            
            
            //TODO: isOn의 변동도 관리해줘야 함.(isOn이 바뀌면 isPropertyChanged도 바꾸는 것.)

                
            
            
        }
        else
        {
            //TODO: 이게뭐ㅏ야
            cmd.changedProperty.linkedPos = originalLinkedPos;
            isPropertyChanged = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그를 끝마칠 때 해야 할 일
        //이동 커맨드 수행과 그에 맞는 처리.
        gameObject.layer = LayerMask.NameToLayer("Ground");
        if (isPropertyChanged)
        {
            ExecuteCommand();
        }
        else cmd = null;
        UpdateLinkVisual(transform.position, identity.property.linkedPos);
    }

    public void ExecuteCommand()
    {
        if (cmd == null) return;
        CommandManager.Instance.AddCommand(cmd);
        cmd.Execute();
    }

public void Init(IOptionalProperty target)
    {
        identity = target;
    }

    //void OnMouseDown()
    //{
    //    dragStart = Input.mousePosition;
    //}

    //void OnMouseDrag()
    //{
    //    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
    //        out RaycastHit hit, 100f, EditorManager.Instance.gridLayer))
    //    {
    //        Vector3Int newLinkedPos = Vector3Int.RoundToInt(hit.point);
    //        identity.property.linkedPos = newLinkedPos;
    //        (identity as LinkableIdentity).isLineVisible = true;
    //        identity.UpdateLinkVisual();
    //    } else
    //    {
    //        Vector3Int newLinkedPos = Vector3Int.one * int.MaxValue;
    //        identity.UpdateLinkVisual();
    //    }


    //}

    //void OnMouseUp()
    //{
    //    if (!EditorManager.Instance.placedBlocks[identity.property.linkedPos].TryGetComponent<IOptionalProperty>(out IOptionalProperty optional))
    //    {
    //        identity.property.linkedPos = Vector3Int.one * int.MaxValue;
    //    } else
    //    {
    //        optional.property.linkedPos = Vector3Int.RoundToInt(transform.position);
    //    }
    //}

    void OnDestroy()
    {
        if (cmd != null) cmd = null;
        Destroy(lineRenderer);
    }
    private Vector3Int CalculateLinkedPos(Vector3 dragDelta)
    {
        
        // 예시: 드래그 방향을 그리드 좌표로 변환
        Vector3 worldDelta = Camera.main.ScreenToWorldPoint(dragDelta);
        return Vector3Int.RoundToInt(worldDelta);
    }
}

