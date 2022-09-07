using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetailsSO_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject {
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name of the enemy")]
    #endregion
    public string enemyName;


    #region Tooltip
    [Tooltip("The prefab for the enemy")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip
    [Tooltip("Distance to the player before enemy starts chasing")]
    #endregion
    public float chaseDistance = 50.0f;

    #region Header ENEMY MATERIAL
    [Space(10)]
    [Header("ENEMY MATERIAL")]
    #endregion
    #region Tooltip
    [Tooltip("This is the standard lit shader material for the enemy (used after the enemy materializes)")]
    #endregion
    public Material enemyStandardMaterial;

    #region Header ENEMY MATERIALIZE SETTINGS
    [Space(10)]
    [Header("ENEMY MATERIALIZE SETTINGS")]
    #endregion
    #region Tooltip
    [Tooltip("The time in seconds that it takes the enemy to materialize")]
    #endregion
    public float enemyMaterializeTime;
    #region Tooltip
    [Tooltip("The shader to be used when the enemy materializes")]
    #endregion
    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    #region Tooltip
    [Tooltip("The color to use when the enemy materializes. This is an HDR color so intensity can be set to cause glowing/bloom")]
    #endregion
    public Color enemyMaterializeColor;

    #region Header ENEMY WEAPON SETTINGS
    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    #endregion
    #region Tooltip
    [Tooltip("The weapon for the enemy - none if doesnt")]
    #endregion
    public WeaponDetailsSO enemyWeaponDetailsSO;
    #region Tooltip
    [Tooltip("The minimum time delay interval in seconds between bursts of enemy shooting. This value should be greater than 0. " +
        "A random value will be selected between the minimum and maximum")]
    #endregion
    public float firingIntervalMin = 0.1f;
    #region Tooltip
    [Tooltip("The maximum time delay interval in seconds between bursts of enemy shooting. This value should be greater than 0. " +
        "A random value will be selected between the minimum and maximum")]
    #endregion
    public float firingIntervalMax = 1.0f;
    #region Tooltip
    [Tooltip("The minimum firing duration that the enemy shoots for during a firing burst. This value should be greater than 0. " +
        "A random value will be selected between the minimum and maximum")]
    #endregion
    public float firingDurationMin = 1.0f;
    #region Tooltip
    [Tooltip("The maximum firing duration that the enemy shoots for during a firing burst. This value should be greater than 0. " +
        "A random value will be selected between the minimum and maximum")]
    #endregion
    public float firingDurationMax = 2.0f;
    #region Tooltip
    [Tooltip("Select this line of sight is required of the player before the enemy fires. If line of sight" +
        " isn't selected the enemy will fire regardless of obstacles whenever the player is in 'range'")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ENEMY HEALTH DETAILS
    [Space(10)]
    [Header("ENEMY HEALTH DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The health of the enemy for each level")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetails;
    #region Tooltip
    [Tooltip("Select if has imunity period immediately affter get hit. If so specify the immunity time in seconds in the other field")]
    #endregion
    public bool isImmunityAfterHit = false;
    #region Tooltip
    [Tooltip("The immunity time in seconds")]
    #endregion
    public float hitImmunityTime;
    #region Tooltip
    [Tooltip("Select to display the health bar for the enemy")]
    #endregion
    public bool isHealthBarDisplayed = false;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        if (isImmunityAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif
    #endregion
}

