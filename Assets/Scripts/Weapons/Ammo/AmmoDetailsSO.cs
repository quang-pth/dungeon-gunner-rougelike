using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetailsSO_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]
    #endregion Header BASIC AMMO DETAILS
    #region Tooltip
    [Tooltip("Name for the ammo")]
    #endregion
    public string ammoName;
    // Determine whether the ammo is came from the player
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFABS AND MATERIALS
    [Space(10)]
    [Header("AMMO SPRITE, PREFABS AND MATERIALS")]
    #endregion Header AMMO SPRITE, PREFABS AND MATERIALS
    #region Tooltip
    [Tooltip("Sprite to be used for the ammo")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("Populate with the prefab to be used with the ammo. If multiple prefabs are specified then a randoom" +
        " prefab from the array with be selected. The prefab can be an ammo pattern - as long as it conforms the IFireable interface.")]
    #endregion
    public GameObject[] ammoPrefabArray;

    #region Tooltip
    [Tooltip("Material to be used for the ammo")]
    #endregion
    public Material ammoMaterial;

    
    #region Tooltip
    [Tooltip("If the ammo should charge briefly before moving then set the time in seconds that the ammo is held charging after firing before release")]
    #endregion
    public float ammoChargeTime = 0.1f;

    #region Tooltip
    [Tooltip("If the ammo has a charge time then specify the material should be used to render the ammo while charging")]
    #endregion
    public Material ammoChargeMaterial;

    #region Header AMMO HIT EFFECT
    [Space(10)]
    [Header("AMMO HIT EFFECT")]
    #endregion
    #region Tooltip
    [Tooltip("The scriptable objects that defines the parameters for the hit effect prefab")]
    #endregion
    public AmmoHitEffectSO ammoHitEffectSO;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("Header AMMO BASE PARAMETERS")]
    #endregion Header AMMO BASE PARAMETERS
    #region Tooltip
    [Tooltip("The damage each ammo deals")]
    #endregion
    public int ammoDamage = 1;

    #region Tooltip
    [Tooltip("The minimum speed of the value - the speed will be a random value between the min and the max")]
    #endregion
    public float ammoMinSpeed = 20f;

    #region Tooltip
    [Tooltip("The maximum speed of the value - the speed will be a random value between the min and the max")]
    #endregion
    public float ammoMaxSpeed = 20f;

    #region Tooltip
    [Tooltip("The range of the ammo (or ammo pattern) in unity units")]
    #endregion
    public float ammoRange = 20f;

    #region Tooltip
    [Tooltip("The rotation speed in degrees per second of the ammo pattern")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("Header AMMO SPREAD DETAILS")]
    #endregion Header AMMO SPREAD DETAILS

    #region Tooltip
    [Tooltip("The minimum spread angle of the ammo. A higher spread means less accuracy. The spread angle will be random value between the min and max")]
    #endregion
    public float ammoSpreadMin = 0f;

    #region Tooltip
    [Tooltip("The maximum spread angle of the ammo. A higher spread means less accuracy. The spread angle will be random value between the min and max")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    #endregion Header AMMO SPAWN DETAILS

    #region Tooltip
    [Tooltip("The minimum number of ammo that are spawned per shot. The number of ammo will be a random value between the min and max")]
    #endregion
    public int ammoSpawnAmountMin = 1;

    #region Tooltip
    [Tooltip("The maximum number of ammo that are spawned per shot. The number of ammo will be a random value between the min and max")]
    #endregion
    public int ammoSpawnAmountMax = 1;

    #region Tooltip
    [Tooltip("Minimum spawn interval time. The time will be a random value between the min and the max")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;

    #region Tooltip
    [Tooltip("Maximum spawn interval time. The time will be a random value between the min and the max")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;

    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    #endregion Header AMMO TRAIL DETAILS

    #region Tooltip
    [Tooltip("Selected if an ammo trail is required, otherwise deselected. If selected then the rest of the ammo trail details should be populated")]
    #endregion
    public bool isAmmoTrail = false;

    #region Tooltip
    [Tooltip("Trail life time in seconds")]
    #endregion
    public float ammoTrailTime = 3f;

    #region Tooltip
    [Tooltip("Ammo trail material")]
    #endregion
    public Material ammoTrailMaterial;

    #region Tooltip
    [Tooltip("The starting width for the ammo trail")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;

    #region Tooltip
    [Tooltip("The ending width for the ammo trail")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);

        if (ammoChargeTime > 0) {
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoMinSpeed), ammoMinSpeed, nameof(ammoMaxSpeed), ammoMaxSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        }

        if (isAmmoTrail) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }
#endif
    #endregion
} 
