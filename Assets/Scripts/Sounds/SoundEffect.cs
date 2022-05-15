using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        if (audioSource.clip != null) {
            audioSource.Play();
        }
    }

    private void OnDisable() {
        audioSource.Stop();
    }

    public void SetSound(SoundEffectSO soundEffectSO)
    {
        audioSource.pitch = Random.Range(soundEffectSO.soundEffectPitchVariationMin, soundEffectSO.soundEffectPitchVariationMax);
        audioSource.volume = soundEffectSO.soundEffectVolume;
        audioSource.clip = soundEffectSO.soundEffectClip;
    }
}
