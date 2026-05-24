using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private Image darkPanel;
    [SerializeField]
    private GameObject gameClearUI;
    [SerializeField]
    private Image clearPanel;

    private bool isGameOver = false;
    private bool isGameClear = false;

    [ContextMenu("Reset")]
    private void DebugDie()
    {
        Retry();    
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        CanvasGroup canvasGroup = gameOverUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        gameOverUI.SetActive(true);

        darkPanel.DOFade(0.7f, 1f);

        canvasGroup.DOFade(1f, 1f)
                   .SetDelay(0.5f)
                   .SetUpdate(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameClear()
    {
        if (isGameClear) return;
        isGameClear = true;

        CanvasGroup canvasGroup = gameClearUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        gameClearUI.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(clearPanel.DOFade(0.2f, 2f))
           .AppendCallback(() =>
           {
               canvasGroup.alpha = 1f;
           })
           .SetUpdate(true);

        Time.timeScale = 0f;
    }
}