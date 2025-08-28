using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SoundEffect
{
    DiceRoll,
    SlotMachine,
    Victory,
    Lose

}

[System.Serializable]
public class Sound
{
    public SoundEffect effect;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgGameMusic;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private List<Sound> soundEffects = new List<Sound>();

    [Header("Settings")]
    [SerializeField] private float musicFadeDuration = 1f;

    private Dictionary<SoundEffect, Sound> soundDictionary = new Dictionary<SoundEffect, Sound>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAudioSources();
        BuildSoundDictionary();
    }

    void InitializeAudioSources()
    {
        bgGameMusic.loop = true;
        bgGameMusic.Play();
    }


    void BuildSoundDictionary()
    {
        foreach (Sound sound in soundEffects)
        {
            if (!soundDictionary.ContainsKey(sound.effect))
            {
                soundDictionary.Add(sound.effect, sound);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound effect found: {sound.effect}");
            }
        }
    }

    #region Public Methods - Sound Effects

    public void PlayEffect(SoundEffect effect)
    {
        PlaySoundEffect(effect, sfxSource);
    }


    private void PlaySoundEffect(SoundEffect effect, AudioSource source)
    {
        if (soundDictionary.TryGetValue(effect, out Sound sound))
        {
            if (sound.loop)
            {
                if (source.clip != sound.clip || !source.isPlaying)
                {
                    source.clip = sound.clip;
                    source.Play();
                }
            }
            else
            {
                source.PlayOneShot(sound.clip, 1);
            }
        }
        else
        {
            Debug.LogWarning($"Sound effect not found: {effect}");
        }
    }

    #endregion   

    #region Utility Methods

    public bool IsPlaying(SoundEffect effect)
    {
        AudioSource[] sources = { sfxSource };

        foreach (AudioSource source in sources)
        {
            if (source.isPlaying && soundDictionary.TryGetValue(effect, out Sound sound))
            {
                if (source.clip == sound.clip)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void StopEffectsAudios()
    {
        sfxSource.Stop();
    }

    public void PauseAll()
    {
        bgGameMusic.Pause();
        sfxSource.Pause();
    }

    public void ResumeAll()
    {
        bgGameMusic.UnPause();
        sfxSource.UnPause();
    }

    public void StopAll()
    {
        bgGameMusic.Stop();
        sfxSource.Stop();
    }

    #endregion
}