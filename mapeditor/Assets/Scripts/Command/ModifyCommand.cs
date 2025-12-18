using UnityEngine;

public abstract class ModifyCommand : Command
{
    public ModifyCommand(GameObject target) : base(target)
    {

    }
    //수정 커맨드 앱스트랙 클래스

    //필요한 것들
    //1. 수정할 대상의 트랜스폼 정보 *미싱뜨면 찾아오기도 편함


    //생각을 해보자.
    //이 클래스를 상속한 이동커맨드의 경우엔 바뀐 후의 위치가 필요함.
    //회전커맨드의 경우엔 바뀐 후의 회전값이 필요함.
    //크기변경 커맨드의 경우엔... 크기변경은 안 만들 것같지만 바뀐 후의 스케일값이 필요함.
    //속성변경 커맨드의 경우엔 바뀐 속성의 변동 정보가 필요함. 이건 속성이란 걸 만든 후에 구현.

}

public class MoveCommand : ModifyCommand
{

    

    public MoveCommand(GameObject target) : base(target)
    {
        //moveTo를 받아서 이 커맨드를 통해 target이 이동될 위치를 정의함.
        //실험: target만 잘 받음으로써 포지션과 회전, 스케일을 모두 주입해줄 수 있는가?
        //반드시 블록이 존재하는 상황에서만 이 커맨드를 만들 수 있기 때문에, 잘 되면 RemoveCommand에서도 불필요한 처리를 제거해줄 수 있음.
        
    }

    public override void Execute()
    {
        //1. Target이 없으면 Position에서 찾아서 주입하기.
        if (!Target) //모종의 이유로 미싱이 나버렸다면?
            Target = EditorManager.Instance.placedBlocks[Position];

        //2. movePosition으로 transform 이동
        Target.transform.position = movePosition;

        //3. 모든 배치된 블록 딕셔너리에서 Position키 제거
        EditorManager.Instance.placedBlocks.Remove(Position);

        //4. 모든 배치된 블록 딕셔너리에다 movePosition키 추가, 오브젝트는 이 커맨드의 Target
        EditorManager.Instance.placedBlocks.Add(movePosition, Target);

        if (Target.GetComponent<BlockIdentity>() is IOptionalProperty optional)
        {
            //만약 이 명령의 대상 객체가 옵셔널프로퍼티를 구현했다면

            if (EditorManager.Instance.placedBlocks.TryGetValue(originalProperty.linkedPos, out var value))
            {
                value.GetComponent<IOptionalProperty>().property.linkedPos = Vector3Int.RoundToInt(Target.transform.position);
                //명령으로 다루지 않고 원래 연결되어있던 대상의 연결 위치를 나의 위치로 바꿔줌.
            }
            else
            {
                //에디터매니저가 원래 연결돼있던 위치의 블록을 모르는 상황....이 있을 수가 있나?
            }
        }
    }
    public override void Undo()
    {
        //되돌리기.
        //1. Target이 없으면 movePosition에서 찾아서 주입하기.
        if (!Target)
            Target = EditorManager.Instance.placedBlocks[movePosition];

        //2. 원래 위치로 타겟을 이동.
        Target.transform.position = Position;

        //3. 블록 딕셔너리에서 movePosition 키 제거
        EditorManager.Instance.placedBlocks.Remove(movePosition);

        //4. 블록 딕셔너리에다 Position 키 추가, 값은 타겟.
        EditorManager.Instance.placedBlocks.Add(Position, Target);

        if (Target.GetComponent<BlockIdentity>() is IOptionalProperty optional)
        {
            //만약 이 명령의 대상 객체가 옵셔널프로퍼티를 구현했다면

            if (EditorManager.Instance.placedBlocks.TryGetValue(originalProperty.linkedPos, out var value))
            {
                value.GetComponent<IOptionalProperty>().property.linkedPos = Vector3Int.RoundToInt(Target.transform.position);
                //명령으로 다루지 않고 원래 연결되어있던 대상의 연결 위치를 나의 위치로 바꿔줌.
            }
            else
            {
                //에디터매니저가 원래 연결돼있던 위치의 블록을 모르는 상황....이 있을 수가 있나?
            }
        }
    }

    public override void Redo()
    {
        Execute();
    }

}


public class RotateCommand : ModifyCommand
{
    //커맨드 적용 후(변경 후) 회전값.

    public RotateCommand(GameObject target) : base(target)
    {
        Position = Vector3Int.RoundToInt(Target.transform.position);
        Rotation = Target.transform.rotation;
        Scale = Target.transform.localScale;
    }

    public override void Execute()
    {
        if (!Target) //모종의 이유로 미싱이 나버렸다면?
            Target = EditorManager.Instance.placedBlocks[Position];

        Target.transform.rotation = rotateRotation;

        
    }
    public override void Undo()
    {
        if (!Target) //모종의 이유로 미싱이 나버렸다면?
            Target = EditorManager.Instance.placedBlocks[Position];

        Target.transform.rotation = Rotation;
    }

    public override void Redo()
    {
        Execute();
    }

}

public class PropertyChangeCommand : ModifyCommand
{
    //커스텀프로퍼티 변경 커맨드. (위치, 회전이 아님.)
    //Target은 이 커맨드가 적용될 오브젝트임..
    
    //내가 연결할 오브젝트.

    //원래의 property

    //변경시킬 property


    public PropertyChangeCommand(GameObject target) : base(target)
    {
        //게임오브젝트 자체를 받아서,
        //해당 게임오브젝트가 가진 IOptionalProperty를 구현한 객체를 가져온 뒤 거기서 Property 주입
        

        //생각을 해보자. 내가 연결한 대상의 property도 '나'를 향하도록 바꿔줘야 하는가?
        //해줘서 나쁠 건 없지 싶음. 그렇게 상호 연결하고 나면, moveCommand의 생성자에서 추가로 이동할 때마다 '내가 가진 프로퍼티가 향하는 좌표에 있는 오브젝트(널이면 패스)' 의 프로퍼티를 가져오고,
        //수행, 언두, 리두에서 그 프로퍼티를 '나의 좌표'로 조작할 수 있음.
    }

    public override void Execute()
    {
        Debug.Log("Execute메서드 실행");
        //타겟이 없으면 가져오기
        Debug.Log("1. Target이 없으면 에디터메니저에서 찾아서 주입.");
        if (!Target) Target = EditorManager.Instance.placedBlocks[Position];
        Target.GetComponent<IOptionalProperty>().property = changedProperty;

        Debug.Log("2. 전체 블록 중에서 이 명령의 원래 링크 위치 탐색 시도");
        //원래 속성의 링크 위치에 대상이 있다면 그 위치에 있는 오브젝트의 속성을 가져와 연결 해제시킴.
        if (EditorManager.Instance.placedBlocks.TryGetValue(originalProperty.linkedPos, out var value)) //원래 속성의 링크 위치에 있는 대상을 찾아봄
        {
            Debug.Log("2-1. 원래 링크 위치에 블록이 있음.");
            if (value.TryGetComponent<IOptionalProperty>(out var linkedProp))
            {
                Debug.Log("2-2. 원래 링크 위치의 블록이 IOptionalProperty 구현.");
                linkedProp.property.linkedPos = Vector3Int.one * int.MaxValue;
                Debug.Log("2-3. 원래 링크 위치의 블록 링크 해제 완료.");
            }
        }
        Debug.Log("3. 전체 블록 중에서 이 명령의 바뀐 위치 탐색 시도.");
        //바꾼 속성의 링크 위치에 있는 오브젝트의 속성을 가져와 나랑 연결.
        if (EditorManager.Instance.placedBlocks.TryGetValue(changedProperty.linkedPos, out var dValue)) //바뀐 속성의 링크 위치에 있는 대상을 찾아봄
        {
            Debug.Log("3-1. 바뀐 링크 위치에 블록이 있음.");
            if (dValue.TryGetComponent<IOptionalProperty>(out var linkedProp))
            {
                Debug.Log("3-2. 바뀐 링크 위치의 블록이 IOptionalProperty 구현.");
                linkedProp.property.linkedPos = Position;
                Debug.Log("3-3. 바뀐 링크 위치의 블록의 링크 연결 완료.");
            }
        }
        Debug.Log("PropertyChageCommand.Execute() 종료.");

    }
    public override void Undo()
    {
        //타겟이 없으면 가져오기
        if (!Target) Target = EditorManager.Instance.placedBlocks[Position];
        Target.GetComponent<IOptionalProperty>().property = originalProperty;

        //바꾼 속성의 링크 위치에 있는 오브젝트의 속성을 가져와 연결 해제.
        if (EditorManager.Instance.placedBlocks.TryGetValue(changedProperty.linkedPos, out var dValue)) //바뀐 속성의 링크 위치에 있는 대상을 찾아봄
        {
            if (dValue.TryGetComponent<IOptionalProperty>(out var linkedProp))
            {
                linkedProp.property.linkedPos = Vector3Int.one * int.MaxValue;
            }
        }
        //원래 속성의 링크 위치에 있는 오브젝트의 속성을 가져와 나와 연결.
        if (EditorManager.Instance.placedBlocks.TryGetValue(originalProperty.linkedPos, out var value)) //원래 속성의 링크 위치에 있는 대상을 찾아봄
        {
            if (value.TryGetComponent<IOptionalProperty>(out var linkedProp))
            {
                linkedProp.property.linkedPos = Position;
            }
        }
    }

    public override void Redo()
    {
        Execute();
        ////타겟이 없으면 가져오기
        //if (!Target) Target = EditorManager.Instance.placedBlocks[Position];
        //Target.GetComponent<IOptionalProperty>().property = changedProperty;
    }

}