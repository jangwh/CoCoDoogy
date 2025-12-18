using UnityEngine;
using UnityEngine.EventSystems;

public class InputActive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EditorManager.Instance.cameraController.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EditorManager.Instance.cameraController.enabled = true;
    }
}
