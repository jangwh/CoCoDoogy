using TMPro;
using UnityEngine;

public class AutoVersion : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionTxt;

    void Awake()
    {
        versionTxt.text = $"V.{Application.version}";
    }
}
