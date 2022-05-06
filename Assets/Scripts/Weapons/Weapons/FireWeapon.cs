using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake() {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
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
        if (fireWeaponEventArgs.fire) {
            if (IsWeaponReadyToFire()) {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();
            }
        }
    }

    private bool IsWeaponReadyToFire()
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        // If there is no ammo and weapon doesnt have infinite ammo
        if (currentWeapon.weaponRemainingAmmo <= 0 && !currentWeapon.weaponDetails.hasInfiniteAmmo) {
            return false;
        }

        if (currentWeapon.isWeaponReloading) {
            return false;
        }

        if (fireRateCoolDownTimer > 0f) {
            return false;
        }

        // If there is no ammo in the clip and weapon doesnt have infinite clip capacity
        if (!currentWeapon.weaponDetails.hasInfiniteClipCapacity && currentWeapon.weaponClipRemainingAmmo <= 0) {
            return false;
        }

        return true;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector) {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null) {
            // Get a random ammo prefab
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            float ammoSpeed = Random.Range(currentAmmo.ammoMinSpeed, currentAmmo.ammoMaxSpeed);

            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            // Reduce the ammo clip count if not infinite clip capacity
            if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity) {
                activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
                activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
            }

            weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        }
    }

    private void ResetCoolDownTimer() {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }
}
