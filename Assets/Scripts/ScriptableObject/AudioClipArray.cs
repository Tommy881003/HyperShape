using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioClipArray : ScriptableObject
{
    public Sound[] sounds;
    private bool once = false;

    public void Initialize()
    {
        if (once)
            return;
        foreach(Sound s in sounds)
        {
            s.hashCode = s.name.GetHashCode();
        }
        once = true;
    }

    
}
