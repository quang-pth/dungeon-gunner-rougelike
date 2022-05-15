using System.Collections;
using UnityEngine;

public class SoundEffectManager : SingletonMonobehavior<SoundEffectManager>
{
    public int soundsVolume = 0;

    private void Start() {
        SetSoundsVolume(soundsVolume);
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

    private void SetSoundsVolume(int soundsVolume) {
        float muteDecibel = -80f;

        if (soundsVolume == 0) {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibel);   
        }
        else {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}
