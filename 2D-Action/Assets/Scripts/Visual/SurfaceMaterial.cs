using UnityEngine;
using UnityEngine.Tilemaps;

public class SurfaceMaterial : MonoBehaviour
{
    [SerializeField]
    private SurfaceType surfaceType;

    public SurfaceType SurfaceType => surfaceType;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public bool HasTile(Vector3 worldPosition)
    {
        Vector3Int cellPosition =
            tilemap.WorldToCell(worldPosition);

        return tilemap.HasTile(cellPosition);
    }
}