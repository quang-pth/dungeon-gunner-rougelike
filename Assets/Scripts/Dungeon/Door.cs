using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameobject")]
    #endregion
    [SerializeField] private BoxCollider2D doorCollider;

    // Control the boss room door - open if only player defeated all the enemies in all the other rooms 
    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    // Unclock previously visited door - if player defeated all the enemies in one particular room
    private bool isPreviouslyOpened = false;
    private Animator animator;

    private void Awake() {
        doorCollider.enabled = false;

        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon) {
            OpenDoor();
        }
    }

    private void OnEnable() {
        // Restore the door animation if player comebacks to the door
        animator.SetBool(Settings.open, isOpen);
    }

    public void OpenDoor() {
        if (!isOpen) {
            isOpen = true;
            isPreviouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            // Play opening the door animation
            animator.SetBool(Settings.open, true);
        }
    }

    public void LockDoor() {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        // Play closing the door animation
        animator.SetBool(Settings.open, false);
    }

    public void UnclockDoor() {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (isPreviouslyOpened) {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}
