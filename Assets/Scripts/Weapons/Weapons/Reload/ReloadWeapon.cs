using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private Coroutine reloadWeaponCoroutine;

    private void Awake() {
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable() {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable() {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;        
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs) {
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs) {
        if (reloadWeaponCoroutine != null) {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.topUpAmmoPercent));
    }

    private IEnumerator ReloadWeaponCoroutine(Weapon weapon, int topUpAmmoPercent) {
        weapon.isWeaponReloading = true;

        while (weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime) {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        // If the total ammo is to be increased then update
        if (topUpAmmoPercent != 0) {
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.weaponClipAmmoCapacity * topUpAmmoPercent) / 100f);

            int totalAmmo = weapon.weaponRemainingAmmo + ammoIncrease;

            if (totalAmmo > weapon.weaponDetails.weaponClipAmmoCapacity) {
                weapon.weaponRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
            } else {
                weapon.weaponRemainingAmmo = totalAmmo;
            }
        }

        int amountOfAmmoToLoad = weapon.weaponDetails.weaponClipAmmoCapacity - weapon.weaponClipRemainingAmmo;
        amountOfAmmoToLoad = Mathf.Min(weapon.weaponRemainingAmmo - weapon.weaponDetails.weaponClipAmmoCapacity, amountOfAmmoToLoad);

        if (weapon.weaponDetails.hasInfiniteAmmo) {
            weapon.weaponClipRemainingAmmo += amountOfAmmoToLoad;
        }
        // else if (weapon.weaponRemainingAmmo >= weapon.weaponDetails.weaponClipAmmoCapacity) {
        //     weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        // }
        else {
            // weapon.weaponClipRemainingAmmo = weapon.weaponRemainingAmmo;
            weapon.weaponClipRemainingAmmo += amountOfAmmoToLoad;
            weapon.weaponRemainingAmmo -= amountOfAmmoToLoad;
        }

        weapon.weaponReloadTimer = 0f;

        weapon.isWeaponReloading = false;

        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs) {
        if (setActiveWeaponEventArgs.weapon.isWeaponReloading) {
            if (reloadWeaponCoroutine != null) {
                StopCoroutine(reloadWeaponCoroutine);
            }

            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(setActiveWeaponEventArgs.weapon, 0));
        }
    }
}
