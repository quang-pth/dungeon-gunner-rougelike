using System.Collections;
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
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake() {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start() {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetPlayerAnimationSpeed();
    }

    private void SetPlayerAnimationSpeed() {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update() {
        if (isPlayerRolling) return;

        MovementInput();
        WeaponInput();

        PlayerRollCooldownTimer();
    }

    private void MovementInput() {
        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        Vector2 moveDirection = new Vector2(xMovement, yMovement);

        // Player either moves or rolls
        if (moveDirection != Vector2.zero) {
            if (!rightMouseButtonDown) {
                player.movementByVelocityEvent.CallMovementByVelocityEvent(moveDirection.normalized, moveSpeed);
            }
            else if (playerRollCooldownTimer <= 0f) {
                PlayerRoll((Vector3)moveDirection.normalized);
            }
        }
        else {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void PlayerRoll(Vector3 direction) {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction) {
        float minDistance = 0.2f;
        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + direction * movementDetails.rollDistance;

        // Rolling player a little bit per fixed update call
        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance) {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);
            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;

        playerRollCooldownTimer = movementDetails.rollCoolDownTime;

        player.transform.position = targetPosition;
    }

    private void PlayerRollCooldownTimer() {
        if (playerRollCooldownTimer >= 0f) {
            playerRollCooldownTimer -= Time.deltaTime;
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

    // Run only at the first frame of collision until collision exit
    private void OnCollisionEnter(Collision other) {
        StopPlayerRollRoutine();
    }

    // Run during the collision
    private void OnCollisionStay2D(Collision2D other) {
        StopPlayerRollRoutine();
    }
    
    private void StopPlayerRollRoutine() {
        if (playerRollCoroutine != null) {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif
    #endregion 

}
