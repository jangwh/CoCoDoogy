using UnityEngine;

public class PlaceCommand : Command
{
    //배치 커맨드.
    //배치 커맨드이기 때문에 추가로 필요한 것:
    // 1. 대상이 되는 오브젝트의 프리팹 원본
    BlockData data;

    // 2. 대상 트랜스폼 정보


    /*// 3. 생성 결과물의 참조
    GameObject result;*/

    public PlaceCommand(BlockData data, Vector3Int position, Quaternion rotation, Vector3 scale) : base(null)
    {
        //생성될 prefab 기록.
        this.data = data;

        //부모의 생성자를 확장하여 x, y, z도 같이 기록.
        this.Position = position;
        this.Rotation = rotation;
        this.Scale = scale;
    }

    public override void Execute()
    {
        //배치 커맨드의 수행 => 커맨드매니저에선 Redo스택을 비움.
        var result = BlockFactory.Instance.CreateBlock(data, Position, Rotation);
        result.transform.rotation = Rotation;
        result.transform.localScale = Scale;

        //블록플레이서의 인스턴스가 가진 '이미 자리잡은 블록' 딕셔너리에 등록
        EditorManager.Instance.placedBlocks.Add(Position, result);

        Target = result; // 생성이 잘 되었으니 Target 등록
        if (Target.TryGetComponent<IOptionalProperty>(out var optional))
        {
            originalProperty = optional.property;
        }
    }


    public override void Undo()
    {
        //배치 커맨드의 되돌리기 => Execute에서 했던 동작의 정 반대 로직(지우기)
        //타겟이 없어졌을 수가 있음. 다시 찾기
        
        Target = EditorManager.Instance.placedBlocks[Position];
        
        Object.Destroy(Target);
        EditorManager.Instance.placedBlocks.Remove(Position);
        

        
    }

    public override void Redo()
    {
        /*//Undo에서의 배치 커맨드 재수행 => Redo스택을 건드리지 않도록 함.
        result = BlockFactory.Instance.CreateBlock(data, position);
        result.transform.rotation = rotation;
        result.transform.localScale = scale;

        //블록플레이서의 인스턴스가 가진 '이미 자리잡은 블록' 딕셔너리에 등록
        BlockPlacer.Instance.placedBlocks.Add(position, result);

        Target = result; // 다시 생겼으니까 Target 재등록*/
        Execute();

        if (Target.TryGetComponent<IOptionalProperty>(out var optional))
        {
            optional.property = originalProperty;
        }
    }
}
