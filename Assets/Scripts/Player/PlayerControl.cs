using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
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

        SetStartingWeapon();

        SetPlayerAnimationSpeed();
    }

    private void SetStartingWeapon() {
        int index = 1;

        foreach (Weapon weapon in player.weaponList) {
            if (weapon.weaponDetails == player.playerDetails.startingWeapon) {
                SetWeaponByIndex(index);
                break;
            }

            index++;
        }
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

        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        SwitchWeaponInput();

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection) {
        Vector3 worldMousePosition = HelperUtilities.GetMouseWorldPosition();
        
        // Calculate the weapon direction vector
        weaponDirection = (worldMousePosition - player.activeWeapon.GetShootPosition()).normalized;
        // Calculate the player direction vector
        Vector3 playerDirection = (worldMousePosition - transform.position).normalized;

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection) {
        // Fire when left mouse button is clicked
        if (Input.GetMouseButton(0)) {
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput() {
        // Preventing player from switching weapon while reloading
        if (player.activeWeapon.GetCurrentWeapon().isWeaponReloading) {
            return;
        }

        // Scrol mouse down to select previous weapon
        if (Input.mouseScrollDelta.y < 0f) {
            SelectPreviousWeapon();
        }
        
        // Scrol mouse up to select next weapon
        if (Input.mouseScrollDelta.y > 0f) {
            SelectNextWeapon();
        }

        // Use number key from 1 to 0 to select weapon
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetWeaponByIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetWeaponByIndex(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SetWeaponByIndex(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SetWeaponByIndex(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SetWeaponByIndex(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            SetWeaponByIndex(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            SetWeaponByIndex(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            SetWeaponByIndex(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            SetWeaponByIndex(9);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            SetWeaponByIndex(10);
        }

        // Press minus key to re-order the weapon list 
        if (Input.GetKeyDown(KeyCode.Minus)) {
            SetCurrentWeaponToFirstInTheList();
        }
    }

    private void SetWeaponByIndex(int weaponIndex) {
        if (weaponIndex - 1 < player.weaponList.Count) {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void SelectNextWeapon() {
        currentWeaponIndex++;

        if (currentWeaponIndex > player.weaponList.Count) {
            currentWeaponIndex = 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void SelectPreviousWeapon() {
        currentWeaponIndex--;

        if (currentWeaponIndex < 1) {
            currentWeaponIndex = player.weaponList.Count;
        }

        SetWeaponByIndex(currentWeaponIndex);

    }

    private void SetCurrentWeaponToFirstInTheList() {
        List<Weapon> tempWeaponList = new List<Weapon>();

        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        int idx = 2;
        foreach(Weapon weapon in player.weaponList) {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = idx;
            idx++;
        }

        player.weaponList = tempWeaponList;
        currentWeaponIndex = 1;
        SetWeaponByIndex(currentWeaponIndex);
    }

    private void ReloadWeaponInput() {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        if (currentWeapon.isWeaponReloading) {
            return;
        }

        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponAmmoCapacity) {
            return;
        }

        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            player.reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon, 0);
        }
    }

    // Run only at the first frame of collision until collision exit
    private void OnCollisionEnter2D(Collision2D other) {
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
