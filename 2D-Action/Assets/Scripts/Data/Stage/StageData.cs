using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage/Stage Data")]
public class StageData : ScriptableObject
{
    [SerializeField]
    private string displayName;

    [SerializeField]
    private SceneAsset scene;

    public string DisplayName => displayName;

    public string SceneName => scene != null ? scene.name : string.Empty;

    public bool IsPlayable => scene != null;
}