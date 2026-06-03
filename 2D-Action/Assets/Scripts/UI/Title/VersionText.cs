using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI versionText;

    private void Start()
    {
        versionText.text = $"v{Application.version}";
    }
}