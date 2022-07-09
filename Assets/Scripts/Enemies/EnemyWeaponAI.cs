using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Select the layer that enemy's bullet will hit")]
    #endregion
    [SerializeField] private LayerMask layerMask;
    #region Tooltip
    [Tooltip("Populate with the WeaponShootPosition child gameobject transform")]
    #endregion
    [SerializeField] private Transform weaponShootPosition;
    private Enemy enemy;
    private EnemyDetailsSO enemyDetailsSO;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetailsSO = enemy.enemyDetailsSO;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        firingIntervalTimer -= Time.deltaTime;

        if (firingIntervalTimer < 0f)
        {
            if (firingDurationTimer >= 0f)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                // Reset timers
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    private void FireWeapon()
    {
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;
        Vector3 weaponDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.transform.position;

        float enemyAnglesDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirectionVector);

        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(weaponAngleDegrees);

        // Call aim weapon event 
        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAnglesDegrees, weaponAngleDegrees, weaponDirectionVector);
        if (enemyDetailsSO.enemyWeaponDetailsSO != null)
        {
            float enemyAmmoRange = enemyDetailsSO.enemyWeaponDetailsSO.weaponCurrentAmmo.ammoRange;
            // If the player is in firable range
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                bool notAbleToFire = enemyDetailsSO.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirectionVector, enemyAmmoRange);
                if (notAbleToFire)
                {
                    return;
                }

                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAnglesDegrees, weaponAngleDegrees, weaponDirectionVector);
            }

            ReloadWeapon();
        }

    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirectionVector, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirectionVector, enemyAmmoRange, layerMask);
        // return true if raycast hit the player
        return raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag);
    }

    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetailsSO.firingIntervalMin, enemyDetailsSO.firingIntervalMax);
    }

    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetailsSO.firingDurationMin, enemyDetailsSO.firingDurationMax);
    }

    private void ReloadWeapon()
    {
        Weapon currentWeapon = enemy.getWeapon();

        if (currentWeapon.isWeaponReloading) return;
        if (currentWeapon.weaponClipRemainingAmmo > 0) return;
        if (currentWeapon.weaponDetails.hasInfiniteClipCapacity) return;

        enemy.reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon, 0);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }
#endif
    #endregion
}
