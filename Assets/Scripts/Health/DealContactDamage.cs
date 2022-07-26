using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    #region Header DEAL DAMAGE
    [Space(10)]
    [Header("DEAL DAMAGE")]
    #endregion
    #region Tooltip
    [Tooltip("The contact damage to deal (can be overridden by the receiver)")]
    #endregion
    [SerializeField] private int contactDamageAmount;
    #region Tooltip
    [Tooltip("Specify the objects layer that can receive contact damage")]
    #endregion 
    [SerializeField] private LayerMask layerMask;
    private bool isCollding = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCollding) return;
        DealDamage(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isCollding) return;
        DealDamage(collision);
    }
       
    private void DealDamage(Collision2D collision)
    {
        // Get the layer value in binary form
        int collisionLayerValue = (1 << collision.gameObject.layer);
        // Return if the collision layer not be able to receive damage layer
        if ((layerMask & collisionLayerValue) == 0)
        {
            return;
        }
        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();
        if (receiveContactDamage != null)
        {
            isCollding = true;
            // Reset the contact collision after set time
            Invoke(nameof(ResetContactCollision), Settings.contactDamageCollisionResetDelay);

            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    private void ResetContactCollision()
    {
        isCollding = false;
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
