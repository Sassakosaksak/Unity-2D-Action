using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class PlayerGroundSensor : MonoBehaviour
{
    private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask surfaceLayer;

    public bool IsGrounded { get; private set; }

    public SurfaceType CurrentSurface { get; private set; }

    private RaycastHit2D groundHit;

    private void FixedUpdate()
    {
        CheckGround();
        CheckSurface();
    }

    public void CheckGround()
    {
        IsGrounded = groundHit.collider != null; 
        
        groundHit = Physics2D.BoxCast(
            transform.position,
            groundCheckSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    public void CheckSurface()
    {
        RaycastHit2D[] surfaceHits = Physics2D.BoxCastAll(
            transform.position,
            groundCheckSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            surfaceLayer
        );

        CurrentSurface = SurfaceType.None;
        SurfaceType fallbackSurface = SurfaceType.None;

        foreach (RaycastHit2D hit in surfaceHits)
        {
            if (!hit.collider.TryGetComponent(out SurfaceMaterial surfaceMaterial))
            {
                continue;
            }

            if (surfaceMaterial.SurfaceType == SurfaceType.Grass)
            {
                CurrentSurface = SurfaceType.Grass;
                return;
            }

            if (fallbackSurface == SurfaceType.None)
            {
                fallbackSurface = surfaceMaterial.SurfaceType;
            }
        }

        CurrentSurface = fallbackSurface;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 boxCenter = transform.position + Vector3.down * groundCheckDistance;

        Gizmos.DrawWireCube(boxCenter, groundCheckSize);
    }
}