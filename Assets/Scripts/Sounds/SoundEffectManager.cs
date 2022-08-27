using System.Collections;
using UnityEngine;

public class SoundEffectManager : SingletonMonobehavior<SoundEffectManager>
{
    public int soundVolume = 0;
    private const string soundVolumeStoreKey = "soundVolume";

    private void Start() {
        // Restore the player set sound volume
        if (PlayerPrefs.HasKey(soundVolumeStoreKey))
        {
            soundVolume = PlayerPrefs.GetInt(soundVolumeStoreKey);
        }
        SetSoundVolume(soundVolume);
    }

    private void OnDisable()
    {
        // Store player set sound volume
        PlayerPrefs.SetInt(soundVolumeStoreKey, soundVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffectSO) {
        SoundEffect soundEffect = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffectSO.soundPrefab, Vector3.zero, Quaternion.identity);
        soundEffect.SetSound(soundEffectSO);
        // Play the sound effect
        soundEffect.gameObject.SetActive(true);
        // Stop the sound effect
        StartCoroutine(DisableSound(soundEffect, soundEffectSO.soundEffectClip.length));
    }

    private IEnumerator DisableSound(SoundEffect sound, float soundDuration) {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    private void SetSoundVolume(int soundsVolume) {
        float muteDecibel = -80f;

        if (soundsVolume == 0) {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibel);   
        }
        else {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }

    public void IncreaseSoundVolume()
    {
        int maxSoundVolume = 20;

        if (soundVolume >= maxSoundVolume) return;

        soundVolume++;
        SetSoundVolume(soundVolume);
    }

    public void DecreaseMusicVolume()
    {
        if (soundVolume <= 0) return;

        soundVolume--;
        SetSoundVolume(soundVolume);
    }
}

