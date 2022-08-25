using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    #region Header SOUND EFFECT
    [Header("SOUND EFFECT")]
    #endregion
    #region Tooltip
    [Tooltip("The sound effect when this item is moved")]
    #endregion
    [SerializeField] private SoundEffectSO soundEffectSO;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();
        instantiatedRoom.moveableItemsList.Add(this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    private void UpdateObstacles()
    {
        ConfineItemToBounds();

        instantiatedRoom.UpdateMoveableObstacles();

        previousPosition = transform.position;
        if (Mathf.Abs(rigidBody2D.velocity.x) > 0.01f || Mathf.Abs(rigidBody2D.velocity.y) > 0.01)
        {
            if (soundEffectSO != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(soundEffectSO);
            }
        }
    }

    private void ConfineItemToBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiatedRoom.roomColliderBounds;
        bool xIsOutOfBounds = itemBounds.min.x <= roomBounds.min.x || itemBounds.max.x >= roomBounds.max.x;
        bool yIsOutOfBounds = itemBounds.min.y <= roomBounds.min.y || itemBounds.max.y >= roomBounds.max.y;
        if (xIsOutOfBounds || yIsOutOfBounds)
        {
            transform.position = previousPosition;
        }
    }
}