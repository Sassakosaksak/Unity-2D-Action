using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;
    [SerializeField]
    private float changeSpeed = 5f;

    private float targetFillAmount;

    private void Awake()
    {
        targetFillAmount = fillImage.fillAmount;
    }

    private void Update()
    {
        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFillAmount,
            Time.deltaTime * changeSpeed
        );
    }
    public void SetHP(float currentHP, float maxHP)
    {
        targetFillAmount = (float)currentHP / maxHP;
    }
}