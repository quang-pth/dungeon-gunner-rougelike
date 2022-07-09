using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    [HideInInspector] public bool isDamagable = true;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Update()
    {
        CallHealthEvent(0);
    }

    // This method get called when the gameObject is taking damage
    public void TakeDamage(int damageAmount)
    {
        if (isDamagable)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
        }
    }

    private void CallHealthEvent(int damageAmount)
    {
        float healthPercent = (float)currentHealth / (float)startingHealth;
        healthEvent.CallHealthChangedEvent(healthPercent, currentHealth, damageAmount);
    }

    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }
}
