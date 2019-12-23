using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 1f)]
    public float pitch;
    [Range(0f, 1f)]
    public float blend;
    public bool loop;
    public bool isFX;
    [HideInInspector]
    public AudioSource source = null;
    [HideInInspector]
    public int hashCode = 0;
}
