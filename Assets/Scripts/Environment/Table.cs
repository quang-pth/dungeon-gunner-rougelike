using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("The mass of the table to control the speed that it moves when pushed")]
    #endregion
    [SerializeField] private float itemMass;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private bool itemUsed = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void UseItem()
    {
        if (itemUsed) return;

        Bounds tableBound = boxCollider2D.bounds;
        Vector3 closestPointToPlayer = tableBound.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());
        
        if (closestPointToPlayer.x == tableBound.max.x)
        {
            animator.SetBool(Settings.flipLeft, true);
        }
        else if (closestPointToPlayer.x == tableBound.min.x)
        {
            animator.SetBool(Settings.flipRight, true);
        }
        else if (closestPointToPlayer.y == tableBound.max.y)
        {
            animator.SetBool(Settings.flipDown, true);
        }
        else
        {
            animator.SetBool(Settings.flipUp, true);
        }

        gameObject.layer = LayerMask.NameToLayer("Environment");
        rigidBody2D.mass = itemMass;
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlipSoundEffectSO);
        itemUsed = true;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif
    #endregion
}
