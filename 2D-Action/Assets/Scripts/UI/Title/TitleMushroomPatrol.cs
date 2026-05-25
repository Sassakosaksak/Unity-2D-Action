using UnityEngine;

/// <summary>
/// タイトル画面専用の簡易回遊制御。
/// 演出目的のため、本編EnemyAIとは分離して実装。
/// </summary>
public class TitleMushroomPatrol : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField]
    private Transform leftPoint;
    [SerializeField]
    private Transform rightPoint;
    [SerializeField]
    private float moveSpeed = 1.2f;

    [Header("Visual")]
    [SerializeField]
    private Transform visual;

    private Transform currentTarget;

    private void Start()
    {
        currentTarget = rightPoint;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget.position,
            moveSpeed * Time.deltaTime
        );

        if (
            Vector3.Distance(
                transform.position,
                currentTarget.position
            ) <= 0.05f)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        bool isMovingRight =
            currentTarget == rightPoint;

        currentTarget =
            isMovingRight
                ? leftPoint
                : rightPoint;

        if (visual == null) return;

        Vector3 scale =
            visual.localScale;

        scale.x =
            isMovingRight
                ? Mathf.Abs(scale.x)
                : -Mathf.Abs(scale.x);

        visual.localScale =
            scale;
    }
}