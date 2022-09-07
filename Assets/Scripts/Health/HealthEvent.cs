using System;
using UnityEngine;

public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthChangedEvent(float healthPercent, int healthAmount, int damageAmount)
    {
        HealthEventArgs healthEventArgs = new HealthEventArgs()
        {
            healthPercent = healthPercent,
            healthAmount = healthAmount,
            damageAmount = damageAmount
        };
        OnHealthChanged?.Invoke(this, healthEventArgs);
    }
}

public class HealthEventArgs
{
    public float healthPercent;
    public int healthAmount;
    public int damageAmount;
}
