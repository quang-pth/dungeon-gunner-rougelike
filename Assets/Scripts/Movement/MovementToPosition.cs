using UnityEngine;

[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private MovementToPositionEvent movementToPositionEvent;
    private Rigidbody2D rigidBody2D;

    private void Awake() {
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable() {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs) {
        MoveRigidBody(movementToPositionArgs.mousePosition, movementToPositionArgs.currentPosition, movementToPositionArgs.moveSpeed);
    }

    private void MoveRigidBody(Vector3 mousePosition, Vector3 currentPosition, float moveSpeed) {
        // Roll direction
        Vector2 unitVector = Vector3.Normalize(mousePosition - currentPosition);

        rigidBody2D.MovePosition(rigidBody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }

}
