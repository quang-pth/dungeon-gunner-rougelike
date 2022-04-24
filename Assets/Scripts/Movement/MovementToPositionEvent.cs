using System;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(Vector3 mousePosition, Vector3 currentPosition, float moveSpeed, Vector2 moveDirection, bool isRolling = false) {
        MovementToPositionArgs eventArgs = new MovementToPositionArgs()
        {
            mousePosition = mousePosition,
            currentPosition = currentPosition,
            moveSpeed = moveSpeed,
            moveDirection = moveDirection,
            isRolling = isRolling,
        };

        OnMovementToPosition?.Invoke(this, eventArgs);
    }
}

public class MovementToPositionArgs : EventArgs {
    public Vector3 mousePosition;
    public Vector3 currentPosition;
    public float moveSpeed;
    public Vector2 moveDirection;
    public bool isRolling;
}
