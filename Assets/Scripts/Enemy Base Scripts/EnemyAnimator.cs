using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {
    [Header("Movement Blend")]
    [SerializeField] private float acceleration = 2f;
    [SerializeField] protected float velocityLimitX = 0.5f;
    [SerializeField] protected float velocityLimitZ = 1f;
    private Animator animator;
    private float currentVelocityX;
    private float currentVelocityZ;

    private void Awake() {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void TrackMovementVelocity(float velocityX, float velocityZ) {
        Vector2 velocity = new Vector2(velocityX, velocityZ);

        if (velocity.magnitude > 0.01f)
            velocity = velocity.normalized;

        float targetX = velocity.x * velocityLimitX;
        float targetZ = velocity.y * velocityLimitZ;

        currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetX, acceleration * Time.deltaTime);
        currentVelocityZ = Mathf.MoveTowards(currentVelocityZ, targetZ, acceleration * Time.deltaTime);

        animator.SetFloat("VelocityX", currentVelocityX);
        animator.SetFloat("VelocityY", currentVelocityZ);
    }

    public void PlayAttack() {
        int randomAttack = Random.Range(0, 2);
        if (randomAttack == 0)
            animator.SetTrigger("Attack1");
        else
            animator.SetTrigger("Attack2");
    }
    public void PlayComboAttack() {
        animator.SetTrigger("Attack1");
        animator.SetTrigger("Attack2");
    }
    public void PlayCounterAttack() {
        animator.SetTrigger("CounterAttack");
    }

    public void SetDefend(bool isDefending) {
        animator.SetBool("Defend", isDefending);
    }
    public void PlayGetHit() {
        animator.SetTrigger("GetHit");
    }

    public void PlayDead() {
        animator.SetTrigger("Dead");
    }
}