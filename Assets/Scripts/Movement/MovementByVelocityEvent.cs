using System;
using UnityEngine;

public class MovementByVelocityEvent : MonoBehaviour
{
    public event Action<MovementByVelocityEvent, MovementByVelocityArgs> OnMovementByVelocity;

    public void CallMovementByVelocityEvent(Vector2 moveDirection, float moveSpeed) {
        MovementByVelocityArgs eventArgs = new MovementByVelocityArgs()
        {
            moveDirection = moveDirection,
            moveSpeed = moveSpeed,
        };

        OnMovementByVelocity?.Invoke(this, eventArgs);
    }

}


public class MovementByVelocityArgs : EventArgs {
    public Vector2 moveDirection;
    public float moveSpeed;
}
