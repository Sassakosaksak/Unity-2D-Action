using UnityEngine;

public abstract class StageFlowBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject clearPanel;

    protected bool isStarted = false;
    protected bool isCleared = false;

    protected virtual void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
        if (isStarted) return;

        isStarted = true;
        OnStageStart();
    }

    public void ClearStage()
    {
        if (isCleared) return;

        isCleared = true;
        OnStageClear();
    }

    protected abstract void OnStageStart();

    protected abstract void OnStageClear();
} 