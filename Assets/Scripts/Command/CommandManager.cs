using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{

    public static CommandManager Instance { get; private set; }

    public LinkedList<ICommandable> commands = new();
    public Stack<ICommandable> redoStack = new();

    public const int MAX_COMMANDS = 10000;

    private bool ctrlPressed;

    
    //커맨드 관련 이벤트 호출이 필요한 상황
    //1. 커맨드의 신규 생성 및 commands에 추가 (엔트리 생성)
    //2. 커맨드의 undo 및 redoStack으로의 이동 (엔트리 수정)
    //3. 커맨드의 redo 및 commands로의 재이동 (엔트리 수정)
    //4. redoStack의 일괄 삭제(redoStack 삭제 전 엔트리 일괄삭제) => onAddCommand
    public Action<ICommandable> onAddCommand;
    public Action<ICommandable> onUndoCommand;
    public Action<ICommandable> onRedoCommand;
    public Action<ICommandable> onRemoveCommand;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //commands = new();
        //redoStack = new();
    }

    public void AddCommand(Command command)
    {
        if (commands.Count > MAX_COMMANDS) //커맨드 리스트의 상한에 도달하면 맨 앞의 것을 지우고
            commands.RemoveFirst();

        //이후 커맨드 리스트의 맨 마지막에 방금 커맨드를 이어붙여줌.
        commands.AddLast(command);
        //새 커맨드가 들어온 것이므로 Redo스택은 비워야 함.
        foreach (var c in redoStack) onRemoveCommand?.Invoke(c);
        redoStack.Clear();
        onAddCommand?.Invoke(command);
    }

    public void UndoCommand()
    {
        EditorManager.Instance.Deselect();
        if (commands.Count <= 0) return;

        //Undo할 커맨드가 뭐냐면... 커맨드 목록의 마지막 것.
        ICommandable cmd = commands.Last.Value;

        //찾은 걸 언두
        onUndoCommand?.Invoke(cmd);
        cmd.Undo();

        //언두했으니까 리두스택에 밀어넣기
        redoStack.Push(cmd);

        //처리가 끝났으니 커맨드 삭제
        commands.RemoveLast();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ctrlPressed = true;
        }

        if (ctrlPressed)
        {
            if (Input.GetKeyDown(KeyCode.Z))
                UndoCommand();
            if (Input.GetKeyDown(KeyCode.Y))
                RedoCommand();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ctrlPressed = false;
        }
    }
    public void RedoCommand()
    {
        EditorManager.Instance.Deselect();
        if (redoStack.Count == 0) return;

        ICommandable redo = redoStack.Pop();
        onRedoCommand?.Invoke(redo);
        redo.Redo();
        commands.AddLast(redo);
    }

    public void UndoCommands(ICommandable command)
    {
        while (commands.Count > 0 && commands.Last.Value != command)
        UndoCommand();
    }

    public void RedoCommands(ICommandable command)
    {
        while (redoStack.Count > 0 && redoStack.Peek() != command)
        {
            RedoCommand();
        }

        // 마지막으로 목표 커맨드까지 실행
        if (redoStack.Count > 0 && redoStack.Peek() == command)
            RedoCommand();
    }

    public void ClearAll()
    {
        foreach (ICommandable command in commands) onRemoveCommand?.Invoke(command);
        foreach (ICommandable command in redoStack) onRemoveCommand?.Invoke(command);
        commands.Clear();
        redoStack.Clear();
    }


}
