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

    private bool isGameOver = false;

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

        gameOverUI.SetActive(true);

        darkPanel.DOFade(0.7f, 1f);

        //Time.timeScale = 0f;

        CanvasGroup canvasGroup = gameOverUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 1f)
                   .SetDelay(0.5f)
                   .SetUpdate(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}