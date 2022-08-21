using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);
    [SerializeField] private HealthBar healthBar;

    [HideInInspector] public bool isDamagable = true;
    [HideInInspector] public Enemy enemy;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();    
    }

    private void Start()
    {
        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    
        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetailsSO.isImmunityAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetailsSO.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }

        if (enemy != null && enemy.enemyDetailsSO.isHealthBarDisplayed && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }
    }

    // This method get called when the gameObject is taking damage
    public void TakeDamage(int damageAmount)
    {
        bool playerIsRolling = player != null && player.playerControl.isPlayerRolling;

        if (isDamagable && !playerIsRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
            PostHitImmunity();

            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }
        }

        // Decrease multiplier point if the player has taken damages
        if (gameObject.CompareTag("Player"))
        {
            StaticEventHandler.CallOnMultiplierEvent(false);
        }
    }

    private void PostHitImmunity()
    {
        if (gameObject.activeSelf == false) return;

        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
            {
                StopCoroutine(immunityCoroutine);
            }

            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }
    }

    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        // Prevent object from being damaged while in the immunityTime
        isDamagable = false;
        // Iterations to flash red color and white color per spriteFlashInterval
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);
        while (iterations > 0)
        {
            // Flash red color
            spriteRenderer.color = Color.red;
            yield return WaitForSecondsSpriteFlashInterval;
            // Flash white color
            spriteRenderer.color = Color.white;
            yield return WaitForSecondsSpriteFlashInterval;
            iterations--;

            yield return null;
        }

        // End immunity time
        isDamagable = true;
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

    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((startingHealth * healthPercent) / 100f);
        int totalHealth = currentHealth + healthIncrease;
        if (totalHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }

        CallHealthEvent(0);
    }
}
