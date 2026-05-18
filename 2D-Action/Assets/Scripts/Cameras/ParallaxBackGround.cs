using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [Range(0f, 1f)]
    [SerializeField]
    private float parallaxFactor = 0.3f;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;
        cameraStartPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - cameraStartPosition;

        transform.position = startPosition + new Vector3(
            cameraDelta.x * parallaxFactor,
            cameraDelta.y * parallaxFactor,
            0f
        );
    }
}