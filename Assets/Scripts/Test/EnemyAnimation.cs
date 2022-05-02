using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAnimation(RuntimeAnimatorController runtimeAnimatorController, Color spriteColor) {
        animator.runtimeAnimatorController = runtimeAnimatorController;
        spriteRenderer.color = spriteColor;
    }
}
