using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    public Rigidbody2D rigidBody2D;
    public IdleEvent idleEvent;

    private void Awake() {
        rigidBody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable() {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable() {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent) {
        MoveRigidBody();
    }

    private void MoveRigidBody() {
        rigidBody2D.velocity = Vector2.zero;
    }
}
