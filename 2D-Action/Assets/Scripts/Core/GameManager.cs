using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Input")]
    [SerializeField]
    private PlayerInput playerInput;

    [Header("UI")]
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private Image darkPanel;
    [SerializeField]
    private SEEntry gameOverSE;
    [SerializeField]
    private GameObject gameClearUI;
    [SerializeField]
    private Image clearPanel;
    [SerializeField]
    private AudioClip gameClearBGM;
    [SerializeField]
    private GameObject retryButton;
    [SerializeField]
    private GameObject titleButton;

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
        playerInput.SwitchCurrentActionMap("UI");

        isGameOver = true;
        
        SEManager.Instance.Play(gameOverSE);
        AudioManager.Instance.ChangeBGM(null);
        AudioManager.Instance.ChangeAmbient(null);

        CanvasGroup canvasGroup = gameOverUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        gameOverUI.SetActive(true);
        SelectButton(retryButton);

        darkPanel.DOFade(0.7f, 1f);

        canvasGroup.DOFade(1f, 1f)
                   .SetDelay(0.5f)
                   .SetUpdate(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        playerInput.SwitchCurrentActionMap("Player");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;

        playerInput.SwitchCurrentActionMap("Player");

        SceneManager.LoadScene("TitleScene");
    }

    public void GameClear()
    {
        if (isGameClear) return;
        isGameClear = true;

        AudioManager.Instance.ChangeBGM(gameClearBGM);
        AudioManager.Instance.ChangeAmbient(null);

        CanvasGroup canvasGroup = gameClearUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        gameClearUI.SetActive(true);
        SelectButton(titleButton);

        Sequence seq = DOTween.Sequence();
        seq.Append(clearPanel.DOFade(0.2f, 2f))
           .AppendCallback(() =>
           {
               canvasGroup.alpha = 1f;
           })
           .SetUpdate(true);

        Time.timeScale = 0f;
    }

    private void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}