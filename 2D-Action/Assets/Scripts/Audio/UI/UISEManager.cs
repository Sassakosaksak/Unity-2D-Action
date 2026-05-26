using UnityEngine;

public class UISEManager : MonoBehaviour
{
	public static UISEManager Instance { get; private set; }

	[SerializeField]
	private SEEntry hoverSE;
	[SerializeField]
	private SEEntry positiveSE;
	[SerializeField]
	private SEEntry negativeSE;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayHover()
	{
		SEManager.Instance.Play(hoverSE);
	}

	public void PlayPositive()
	{
		SEManager.Instance.Play(positiveSE);
	}

	public void PlayNegative()
	{
		SEManager.Instance.Play(negativeSE);
	}
}