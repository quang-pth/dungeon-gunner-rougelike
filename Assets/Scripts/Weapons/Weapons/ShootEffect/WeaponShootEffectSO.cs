using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header WEAPON SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The color gradient for the shoot effect. The color gradient show the color for the particle during" +
    " during their time life - from left to right")]
    #endregion
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("The length of time the particle system is emitting particles")]
    #endregion
    public float duration = 0.5f;

    #region Tooltip
    [Tooltip("The start particle size for the particle effect")]
    #endregion
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("The start particle speed for the particle effect")]
    #endregion
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("The particle life time for the particle effect")]
    #endregion
    public float startLifeTime = 0.5f;

    #region Tooltip
    [Tooltip("The maximum number of particle to be emitted")]
    #endregion
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("The number of particles emitted per second. If zero it will be the burst number")]
    #endregion
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("How many particles should be emitted in the particle burst effect")]
    #endregion
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("The gravity on the particles - a small negative number will make them float up")]
    #endregion
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("The sprite for the particle effect. If none is specified then the default particle sprite will be used")]
    #endregion
    public Sprite sprite;

    #region Tooltip
    [Tooltip("The min velocity for the particles over its life time. A random value between min and max will be generated")]
    #endregion
    public Vector3 velocityOverLifeTimeMin;

    #region Tooltip
    [Tooltip("The max velocity for the particles over its life time. A random value between min and max will be generated")]
    #endregion
    public Vector3 velocityOverLifeTimeMax;

    #region Tooltip
    [Tooltip("Contains the particle system for the shoot effect - and is configured by the weaponShootEffectSO")]
    #endregion
    public GameObject weaponShootEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifeTime), startLifeTime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);
    }
#endif
    #endregion
}
