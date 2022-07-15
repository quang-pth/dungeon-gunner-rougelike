using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveContactDamage : MonoBehaviour
{
    #region Header
    [Header("The contact damage amount to receive")]
    #endregion
    [SerializeField] private int contactDamageAmount;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeContactDamage(int damageAmount)
    {
        if (damageAmount > 0)
        {
            contactDamageAmount = damageAmount;
        }

        health.TakeDamage(contactDamageAmount);
        
        // Reduce multiplier score if the player recieve contact damage
        if (gameObject.CompareTag("Player"))
        {
            StaticEventHandler.CallOnMultiplierEvent(false);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
    #endregion
}
