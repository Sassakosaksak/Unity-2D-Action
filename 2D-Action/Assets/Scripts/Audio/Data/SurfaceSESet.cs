using UnityEngine;

[System.Serializable]
public class SurfaceSESet
{
    [SerializeField]
    private SurfaceType surfaceType;
    public SurfaceType SurfaceType => surfaceType;

    [SerializeField]
    private SEEntry se;
    public SEEntry Se => se;
}