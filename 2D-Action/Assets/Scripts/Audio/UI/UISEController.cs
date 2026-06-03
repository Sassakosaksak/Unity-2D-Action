using UnityEngine;

public class UISEController : MonoBehaviour
{
	[SerializeField]
	private SEEntry hoverSE;
	[SerializeField]
	private SEEntry positiveSE;
	[SerializeField]
	private SEEntry negativeSE;
	[SerializeField]
	private SEEntry lockedSE;

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

	public void PlayLocked()
	{
		SEManager.Instance.Play(lockedSE);
	}
}