using System.Collections.Generic;
using UnityEngine;


public interface ICommandable
{
    
    public GameObject Target { get; set; } 
    //Target은 지금은 GameObject자료형이지만, 각각의 인게임 오브젝트를 모두 추상화해서 Entity(이름미정) 등의 자료형으로 둘 예정임...
    //아니다. GameObject로 Target을 설정하는 것은 자연스러움. 다만 단순 게임오브젝트일 때 이상의 로직이 필요한 경우에는 해당 로직을 처리하는 부분에서
    //(해당 게임오브젝트).GetComponent<Entity>()한 뒤에(여기서 Entity는 abstract이므로 구체 타입으로는 Trigger라던가 Platform이라던가 갈리게 됨)
    //실제 구체 타입이 무엇인지에 따라 로직을 분기하면 될 것같다. 뭣보다 Entity 자체는 맵에디터에서는 크게 쓰임이 없을 듯.
    //예컨대 플랫폼이나 트리거의 초기 상태 설정 기능 및 트리거의 연결 대상 설정 기능 등 외에는 딱히 쓸 데가 없음.
    

    
    
    void Execute();
    void Undo();
    void Redo();
}

public abstract class Command : ICommandable
{
    public GameObject Target { get; set; }


    public Vector3Int Position { get; protected set; }
    public Quaternion Rotation { get; protected set; }
    public Vector3 Scale { get; protected set; }
    

    public Vector3Int movePosition;
    public Quaternion rotateRotation;


    public OptionalProperty originalProperty;
    public OptionalProperty changedProperty = new();

    public Command(GameObject target)
    {
        //커맨드를 생성하는 시점에, PlaceCommand를 제외하고는 모두 target이 있을 것이고,
        //target이 있기 때문에 target의 원래 Position, Rotaion, Scale 정보를 받을 수 있음.
        Target = target;
        if (Target != null)
        {
            Position = Vector3Int.RoundToInt(Target.transform.position);
            Rotation = Target.transform.rotation;
            Scale = Target.transform.localScale;
        //추가로, 만약 하위 자료형의 객체 중 IOptionalProperty를 구현한 경우 property도 받을 수 있음.
        //이게 모든 커맨드에 필요한 이유는, 객체 삭제-생성-이동-속성변경 커맨드의 누적에 불구 참조 연결을 보장하기 위함임.
            if (Target.GetComponent<BlockIdentity>() is IOptionalProperty op)
            {
                originalProperty = op.property;
            }
        }
    
    }

    public abstract void Execute();
    public abstract void Undo();
    public abstract void Redo();

   
}





