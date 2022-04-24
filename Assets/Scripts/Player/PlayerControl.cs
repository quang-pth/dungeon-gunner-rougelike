using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;

    #region Tooltip
    [Tooltip("The player WeaponShootPosition gameobject in the hieracrchy")]
    #endregion
    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;

    private void Awake() {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Update() {
        MovementInput();
        WeaponInput();
    }

    private void MovementInput() {
        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(xMovement, yMovement);

        if (moveDirection != Vector2.zero) {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(moveDirection.normalized, moveSpeed);
        }
        else {
            player.idleEvent.CallIdleEvent();
        }

    }

    private void WeaponInput() {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection) {
        Vector3 worldMousePosition = HelperUtilities.GetMouseWorldPosition();

        // Calculate the weapon direction vector
        weaponDirection = (worldMousePosition - weaponShootPosition.position).normalized;
        // Calculate the player direction vector
        Vector3 playerDirection = (worldMousePosition - transform.position).normalized;

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif
    #endregion 

}
