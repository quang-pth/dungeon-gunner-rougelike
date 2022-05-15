using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePrechargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;

    private void Awake() {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }

    private void OnEnable() {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;

    }

    private void OnDisable() {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update() {
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs) {
        WeaponFire(fireWeaponEventArgs);
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs) {
        // Handle weapon precharge
        WeaponPrecharge(fireWeaponEventArgs);

        // Handle weapon fire
        if (fireWeaponEventArgs.fire) {
            if (IsWeaponReadyToFire()) {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    private void WeaponPrecharge(FireWeaponEventArgs fireWeaponEventArgs ) {
        if (fireWeaponEventArgs.firePreviousFrame) {
            firePrechargeTimer -= Time.deltaTime;
        }
        else {
            ResetPrechargeTimer();
        }
    }

    private bool IsWeaponReadyToFire()
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        // If there is no ammo and weapon doesnt have infinite ammo
        if (currentWeapon.weaponRemainingAmmo <= 0 && !currentWeapon.weaponDetails.hasInfiniteAmmo) {
            return false;
        }

        // If run out of ammo on both clip ammo and ramaining ammo
        if (currentWeapon.weaponClipRemainingAmmo == 0 && (currentWeapon.weaponRemainingAmmo - currentWeapon.weaponDetails.weaponClipAmmoCapacity) <= 0) {
            return false;
        }

        if (currentWeapon.isWeaponReloading) {
            return false;
        }

        // If the weapon is precharging or cooling down then return false
        if (firePrechargeTimer > 0f || fireRateCoolDownTimer > 0f) {
            return false;
        }

        // If there is no ammo in the clip and weapon doesnt have infinite clip capacity
        bool canReloadAmmo = !currentWeapon.weaponDetails.hasInfiniteClipCapacity && (currentWeapon.weaponClipRemainingAmmo <= 0) && (currentWeapon.weaponRemainingAmmo - currentWeapon.weaponDetails.weaponClipAmmoCapacity > 0);
        if (canReloadAmmo) {
            // Reload the weapon
            reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon, 0);
            return false;
        }

        return true;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector) {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();
        
        if (currentAmmo != null) {
            StartCoroutine(FireAmmoCoroutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    private IEnumerator FireAmmoCoroutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector) {
        int ammoCounter = 0;
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);
        float ammoSpawnInterval = ammoPerShot > 1 ? Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax) : 0;
        
        while (ammoCounter < ammoPerShot) {
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            float ammoSpeed = Random.Range(currentAmmo.ammoMinSpeed, currentAmmo.ammoMaxSpeed);

            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            ammoCounter++;

            // wait for ammo per shot
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        // Reduce the ammo clip count if not infinite clip capacity
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity) {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
        }

        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        // PLay weapon firing sound effect if any
        WeaponSoundEffect();
    }

    private void ResetCoolDownTimer() {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void ResetPrechargeTimer() {
        firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    private void WeaponSoundEffect() {
        SoundEffectSO weaponFiringSoundEffect = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect;
        if (weaponFiringSoundEffect != null) {
            SoundEffectManager.Instance.PlaySoundEffect(weaponFiringSoundEffect);
        }
    }
}
