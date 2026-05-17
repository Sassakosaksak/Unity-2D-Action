using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMessageUI : MonoBehaviour
{
    [SerializeField]
    private GameObject messagePanel;
    [SerializeField] 
    private TMP_Text messageText;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float messageUIMargin = 1.8f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        messagePanel.SetActive(false);
        transform.position = playerTransform.position + Vector3.up * messageUIMargin;
    }

    private void LateUpdate()
    {
        transform.position = playerTransform.position + Vector3.up * messageUIMargin;
    }

    public void ShowMessage(string text, float duration = 2f)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(MessageRoutine(text, duration));
    }

    private IEnumerator MessageRoutine(string text, float duration)
    {
        messagePanel.SetActive(true);
        messageText.text = text;

        yield return new WaitForSeconds(duration);

        messagePanel.SetActive(false);
    }
}