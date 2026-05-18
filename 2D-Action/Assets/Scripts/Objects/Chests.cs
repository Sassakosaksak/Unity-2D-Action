using System;
using UnityEngine;

public class Chest : MonoBehaviour, IBreakable
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject openEffectPrefab;
    [SerializeField] 
    private ParticleSystem openParticle;
    private Collider2D col;


    public event Action Opened;

    private bool isOpened = false;

    [ContextMenu("Reset Chest")]
    private void ResetChest()
    {
        isOpened = false;

        animator.Rebind();
        animator.Update(0f);
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

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

    public void PlayOpenEffect()
    {
        // ChestのColliderの位置からエフェクト出現位置を設定
        Vector3 spawnPos = col.bounds.center + new Vector3(col.bounds.extents.x * 0.5f , col.bounds.extents.y * 1.25f);

        Instantiate(openEffectPrefab, spawnPos, Quaternion.identity);

        openParticle.Play();
    }
}