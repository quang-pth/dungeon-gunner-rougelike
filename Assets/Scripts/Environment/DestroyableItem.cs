using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    #region Header HEALTH
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("What the starting health should be for the destroyable item")]
    #endregion
    [SerializeField] private int startingHeatlthAmount = 1;
    #region Header SOUND EFFECT
    [Header("SOUND EFFECT")]
    #endregion
    #region Tooltip
    [Tooltip("The sound effect is played when the item is destroyed")]
    #endregion
    [SerializeField] private SoundEffectSO destroySoundEffectSO;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveContactDamage receiveContactDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHeatlthAmount);
        receiveContactDamage = GetComponent<ReceiveContactDamage>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HeatlEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HeatlEvent_OnHealthChanged;
    }

    private void HeatlEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        Destroy(boxCollider2D);

        if (destroySoundEffectSO != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffectSO);
        }

        animator.SetBool(Settings.destroy, true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.destroyedState))
        {
            yield return null;
        }

        Destroy(animator);
        Destroy(receiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(this);
    }
}
