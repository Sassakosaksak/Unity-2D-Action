using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] 
    private StageFlowBase stageFlow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        stageFlow.ClearStage();
    }
}