using UnityEngine;

public class Chest : MonoBehaviour, IBreakable
{
    [SerializeField]
    private Animator animator;

    private bool isOpened = false;

    public void Break()
    {
        Open();
    }

    public void Open()
    {
        if (isOpened) return;

        isOpened = true;
        animator.SetTrigger("Open");
    }
}