using System;
using UnityEngine;

public class Chest : MonoBehaviour, IBreakable
{
    [SerializeField]
    private Animator animator;
    public event Action Opened;

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

        Opened?.Invoke();
    }
}