using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    public void CallReloadWeaponEvent(Weapon weapon, int topUpAmmoPercent) {
        ReloadWeaponEventArgs eventArgs = new ReloadWeaponEventArgs()
        {
            weapon = weapon,
            topUpAmmoPercent = topUpAmmoPercent,
        };

        OnReloadWeapon?.Invoke(this, eventArgs);
    }
}

public class ReloadWeaponEventArgs : EventArgs {
    public Weapon weapon;
    public int topUpAmmoPercent;
}
