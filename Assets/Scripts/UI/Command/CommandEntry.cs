using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandEntry : MonoBehaviour, IPointerClickHandler
{
    public ICommandable Command { get; private set; }
    [SerializeField] TextMeshProUGUI commandText;

    //public bool isOnRedoStack;

    Color showColor = Color.black;
    Color dimColor = Color.white * .5f;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Command == null) { print("커맨드가 없음;;"); return; }
        if (CommandManager.Instance.commands.Contains(Command)) { CommandManager.Instance.UndoCommands(Command); }
        else if (CommandManager.Instance.redoStack.Contains(Command)) { CommandManager.Instance.RedoCommands(Command); }
        else { print("커맨드엔트리 클릭: 근데 메서드가 이상한듯"); return; }
    }

    public void Set(ICommandable command)
    {
        this.Command = command;
        switch (command)
        {
            case PlaceCommand pc:
                commandText.text = $"Place - [{pc.Position.x}], [{pc.Position.y}], [{pc.Position.z}]";
                break;
            case RemoveCommand rc:
                commandText.text = $"Remove - [{rc.Position.x}], [{rc.Position.y}], [{rc.Position.z}]";
                break;
            case MoveCommand mc:
                commandText.text = $"Move - [{mc.Position.x}], [{mc.Position.y}], [{mc.Position.z}] -> [{mc.movePosition.x}], [{mc.movePosition.y}], [{mc.movePosition.z}]";
                break;
            case RotateCommand rc:
                Vector3 originalRot = rc.Rotation.eulerAngles;
                Vector3 rotateRot = rc.rotateRotation.eulerAngles;
                commandText.text = $"Rotate - [{originalRot.x}], [{originalRot.y}], [{originalRot.z}] -> [{rotateRot.x}], [{rotateRot.y}], [{rotateRot.z}]";
                break;
            case PropertyChangeCommand pc:
                Vector3 pos = pc.Position;
                Vector3 originalLink = pc.originalProperty.linkedPos;
                Vector3 changedLink = pc.changedProperty.linkedPos;

                string originalLinkText = originalLink == Vector3Int.one * int.MaxValue ? "NONE" : $"[{originalLink.x}], [{originalLink.y}], [{originalLink.z}]";
                string changedLinkText = changedLink == Vector3Int.one * int.MaxValue ? "NONE" : $"[{changedLink.x}], [{changedLink.y}], [{changedLink.z}]";
                commandText.text = $"Property Change : Link - [{pos.x}], [{pos.y}], [{pos.z}] : [{originalLinkText}] -> [{changedLinkText}]";
                break;
            default:
                commandText.text = $"Undefined Command";
                print("커맨드엔트리 set을 호출하긴 했는데 메서드가 이상함;");
                break;

        }
    }

    public void Dim()
    {
        //isOnRedoStack = true;
        commandText.color = dimColor;
    }

    public void Show()
    {
        //isOnRedoStack = false;
        commandText.color = showColor;
    }
            
        
        
 
}
