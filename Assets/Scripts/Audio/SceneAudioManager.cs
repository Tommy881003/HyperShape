using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClipArray sceneClipArray, generalClipArray;
    [HideInInspector]
    public List<Clip> sceneClips = new List<Clip>(), generalClips = new List<Clip>();
    public static SceneAudioManager instance;
    [Range(0f, 1f)]
    public float musicAmp = 1, fxAmp = 1;
    private bool sceneChangeComplete = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if(generalClipArray != null)
        {
            generalClipArray.Initialize();
            foreach (Sound s in generalClipArray.sounds)
            {
                Clip newClip = new Clip();
                newClip.name = s.name;
                newClip.hash = s.hashCode;
                newClip.isFX = s.isFX;
                newClip.volume = s.volume;
                newClip.source = gameObject.AddComponent<AudioSource>();
                newClip.source.playOnAwake = false;
                newClip.source.clip = s.clip;
                newClip.source.volume = s.volume;
                newClip.source.pitch = s.pitch;
                newClip.source.loop = s.loop;
                newClip.source.spatialBlend = s.blend;
                newClip.source.bypassReverbZones = true;
                generalClips.Add(newClip);
            }
        }
        if(sceneClipArray != null)
        {
            foreach (Sound s in sceneClipArray.sounds)
            {
                Clip newClip = new Clip();
                newClip.name = s.name;
                newClip.hash = s.hashCode;
                newClip.isFX = s.isFX;
                newClip.volume = s.volume;
                newClip.source = gameObject.AddComponent<AudioSource>();
                newClip.source.clip = s.clip;
                newClip.source.playOnAwake = false;
                newClip.source.volume = s.volume;
                newClip.source.pitch = s.pitch;
                newClip.source.loop = s.loop;
                newClip.source.spatialBlend = s.blend;
                newClip.source.bypassEffects = true;
                newClip.source.bypassReverbZones = true;
                sceneClips.Add(newClip);
            }
        }
    }

    private void Start()
    {
        PlayByName("bgm", sceneClips);
        StopByName("boss", sceneClips);
        StopByName("low", sceneClips);
    }

    private void Update()
    {
        if(sceneChangeComplete)
        {
            foreach (Clip s in sceneClips)
            {
                if (s.isFX)
                    s.source.volume = s.volume * fxAmp;
                else
                    s.source.volume = s.volume * musicAmp;
            }
        }
    }

    public void PlayByName(string name, List<Clip> clips)
    {
        int hash = name.GetHashCode();
        foreach (Clip s in clips)
        {
            if (hash == s.hash && name.Equals(s.name))
            {
                if (s.isFX)
                    s.source.volume = s.volume * fxAmp;
                else
                    s.source.volume = s.volume * musicAmp;
                if(s.source.isPlaying == false)
                    s.source.Play();
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void StopByName(string name, List<Clip> clips)
    {
        int hash = name.GetHashCode();
        foreach (Clip s in clips)
        {
            if (hash == s.hash && name.Equals(s.name))
            {
                if (s.source.isPlaying)
                    s.source.Stop();
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void StopAll()
    {
        foreach (Clip s in sceneClips)
            if (s.source.isPlaying)
                s.source.Stop();
    }

    public void VolumeChange(string name, List<Clip> clips, float endValue, float time)
    {
        int hash = name.GetHashCode();
        foreach (Clip s in clips)
        {
            if (hash == s.hash && name.Equals(s.name))
            {
                DOTween.To(() => s.volume, x => s.volume = x, endValue, time);
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void VolumeChange(List<Clip> clips, float endValue, float time)
    {
        foreach (Clip s in clips)
        {
            DOTween.To(() => s.volume, x => s.volume = x, endValue, time);
        }
    }
}
