using System.Collections;
using UnityEngine;

public class MusicManager : SingletonMonobehavior<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    [SerializeField] private int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        musicAudioSource = GetComponent<AudioSource>();
        GameResources.Instance.musicOnOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        SetMusicVolume(musicVolume);
    }

    private void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", 
                HelperUtilities.LinearToDecibels(musicVolume));
        }
    }

    public void PlayMusic(MusicTrackSO musicTrackSO, float fadeOutTime = Settings.musicFadeOutTime, 
        float fadeInTime = Settings.musicFadeInTime)
    {
        StartCoroutine(PlayMusicCoroutine(musicTrackSO, fadeOutTime, fadeInTime));
    }

    private IEnumerator PlayMusicCoroutine(MusicTrackSO musicTrackSO, float fadeOutTime, float fadeInTime)
    {
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }
        
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        if (musicTrackSO.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrackSO.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));
            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrackSO, fadeInTime));
        }

        yield return null;
    }

    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicOnLowSnapshot.TransitionTo(fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
    }

    private IEnumerator FadeInMusic(MusicTrackSO musicTrackSO, float fadeInTime)
    {
        musicAudioSource.clip = musicTrackSO.musicClip;
        musicAudioSource.volume = musicTrackSO.musicVolume;
        musicAudioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
    }
}
