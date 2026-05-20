using UnityEngine;

[System.Serializable]
public class SEEntry
{
    [SerializeField]
    [Tooltip("•¡”‚Ìê‡ƒ‰ƒ“ƒ_ƒ€Ä¶")]
    private AudioClip[] clips;
    public AudioClip[] Clips => clips;

    [SerializeField]
    private float volume = 1f;
    public float Volume => volume;

    [SerializeField]
    private bool isPitchRandom = false;
    public bool IsPitchRandom => isPitchRandom;

    [SerializeField]
    private Vector2 pitchRange = new(0.95f, 1.05f);
    public Vector2 PitchRange => pitchRange;
}