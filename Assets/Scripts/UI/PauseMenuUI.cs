using TMPro;
using UnityEngine;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the music volume level")]
    #endregion
    [SerializeField] private TextMeshProUGUI musicLevelText;
    #region Tooltip
    [Tooltip("Populate with the sounds volum level")]
    #endregion
    [SerializeField] private TextMeshProUGUI soundsLevelText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator IntializeUI()
    {
        yield return null;

        soundsLevelText.SetText(SoundEffectManager.Instance.soundVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        StartCoroutine(IntializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    public void IncreaseSoundVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundVolume.ToString());
    }

    public void DecreaseSoundVolume()
    {
        SoundEffectManager.Instance.DecreaseMusicVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundVolume.ToString());
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsLevelText), soundsLevelText);
    }
#endif
    #endregion
}
