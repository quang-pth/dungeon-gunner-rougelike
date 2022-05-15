using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    #region Header SOUND EFFECT DETAILS
    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The name for the sound effect")]
    #endregion
    public string soundEffectName;

    #region Tooltip
    [Tooltip("The prefab for the sound effect")]
    #endregion
    public GameObject soundPrefab;

    #region Tooltip
    [Tooltip("The audio clip for the sound effect")]
    #endregion
    public AudioClip soundEffectClip;

    #region Tooltip
    [Tooltip("The minimum pitch variation for the sound effect. A random pitch will be generated between the minimum and maximum values. "
    + " A random pitch variation makes the sound effect sound more natural.")]
    #endregion
    [Range(0.1f, 1.5f)] public float soundEffectPitchVariationMin = 0.8f;

    #region Tooltip
    [Tooltip("The maximum pitch variation for the sound effect. A random pitch will be generated between the minimum and maximum values. "
    + " A random pitch variation makes the sound effect sound more natural.")]
    #endregion
    [Range(0.1f, 1.5f)] public float soundEffectPitchVariationMax = 1.2f;

    #region Tooltip
    [Tooltip("The sound effect volume.")]
    #endregion
    [Range(0f, 1f)] public float soundEffectVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchVariationMin), soundEffectPitchVariationMin,
                nameof(soundEffectPitchVariationMax), soundEffectPitchVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
    #endregion
}
