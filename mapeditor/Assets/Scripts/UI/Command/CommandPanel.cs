using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private List<CommandEntry> entries;
    [SerializeField] private CommandEntry commandEntry;
    [SerializeField] private Transform entryContent;


    void Reset()
    {
        
    }

    void Awake()
    {

    }

    void Start()
    {
        entries = new();
        CommandManager.Instance.onAddCommand += OnAddCommand;
        CommandManager.Instance.onUndoCommand += OnUndoCommand;
        CommandManager.Instance.onRedoCommand += OnRedoCommand;
        CommandManager.Instance.onRemoveCommand += OnRemoveCommand;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        EditorManager.Instance.cameraController.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EditorManager.Instance.cameraController.enabled = true;
    }

    private void OnAddCommand(ICommandable command)
    {
        var entry = Instantiate(commandEntry, entryContent);
        entries.Add(entry);
        entry.Set(command);
    }

    private void OnUndoCommand(ICommandable command)
    {
        var entry = entries.Find(x => x.Command == command);
        entry.Dim();
    }

    private void OnRedoCommand(ICommandable command)
    {
        var entry = entries.Find(x=> x.Command == command);
        entry.Show();
    }

    private void OnRemoveCommand(ICommandable command)
    {
        var entry = entries.Find(x => x.Command == command);
        entries.Remove(entry);
        Destroy(entry.gameObject);
    }
}
