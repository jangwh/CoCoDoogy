using UnityEngine;

public class RemoveCommand : Command
{
    // 삭제 커맨드
    // 삭제 커맨드이기 때문에 추가로 필요한 것:
    // 1. 내가 삭제하는 대상의 프리팹 원본
    BlockData data;

    // 2. 대상 트랜스폼 정보




    public RemoveCommand(GameObject target, Vector3Int position, Quaternion rotation, Vector3 scale) : base(target)
    {
        //타겟만 받으면 되는 이유 -> 이미 블록이 있는 상태에서만 RemoveCommand를 사용 가능하기 때문에.(없는 걸 지울 순 없음)
        //이 커맨드의 블록데이터는 타겟의 이름으로 블록데이터를 팩토리에서 블록데이터를 받아오도록 시키기
        this.data = BlockFactory.Instance.allBlocks.Find(x => x.blockName == target.GetComponent<BlockIdentity>().blockName); //이거 안되겠지. 되나?

        //언두 때 명령을 복원하기 위함
        this.Position = position;
        this.Rotation = rotation;
        this.Scale = scale;
    }
    public override void Execute()
    {
        //블록을 지우고, 전체 리스트에서 삭제하는 로직
        //이 시점에는 Target이 반드시 있음. 글쎄 반드시 있나??
        EditorManager.Instance.placedBlocks.Remove(Position);
        Object.Destroy(Target);

        if (Target.TryGetComponent<IOptionalProperty>(out var optional))
        {
            if (optional.property.linkedPos != Vector3Int.one * int.MaxValue) //연결된 대상이 있었다면
            {

                if (EditorManager.Instance.placedBlocks.TryGetValue(originalProperty.linkedPos, out var linked))
                {
                    linked.GetComponent<IOptionalProperty>().property.linkedPos = Vector3Int.one * int.MaxValue; //커맨드로 수행 없이 대상의 연결 해제
                }

            }
        }
    }
    public override void Undo()
    {
        //지웠던 블록을 기억해뒀다가 다시 생성하는 로직: 이 커맨드가 기억하고 있는 BlockData를 활용하여 재생성
        var result = BlockFactory.Instance.CreateBlock(data, Position, Rotation);
        result.transform.rotation = Rotation;
        result.transform.localScale = Scale;

        //블록플레이서의 인스턴스가 가진 '이미 자리잡은 블록' 딕셔너리에 등록
        EditorManager.Instance.placedBlocks.Add(Position, result);

        Target = result; // 생성이 잘 되었으니 Target 등록

        //만약 옵셔널프로퍼티를 가진 객체면 이 명령이 기억하고 있는 프로퍼티 적용
        if (Target.TryGetComponent<IOptionalProperty>(out var optional))
        {
            optional.property = originalProperty;
            if (EditorManager.Instance.placedBlocks.TryGetValue(optional.property.linkedPos, out var linked)) //원래 속성에서 연결된 대상이 있었다면
            {
                linked.GetComponent<IOptionalProperty>().property.linkedPos = Position;
            }
        }
    }

    public override void Redo()
    {
        //다시 실행하는 로직(Execute랑 비슷)
        //타겟이 없어졌을 수가 있으므로 다시 찾기
        if (!Target)
            Target = EditorManager.Instance.placedBlocks[Position];
        Execute();
    }

}
