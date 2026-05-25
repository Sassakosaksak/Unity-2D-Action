using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    public void StartTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}