using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] 
    private StageFlowBase stageFlow;
    [SerializeField]
    private GameObject fireworks;

    private GameClearSEController gameClearSEController;


    private void Awake()
    {
        gameClearSEController = GetComponent<GameClearSEController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        stageFlow.ClearStage();
        gameClearSEController.PlayTriumph();
        ParticleManager.Instance.ChangeParticle(fireworks);
    }
}