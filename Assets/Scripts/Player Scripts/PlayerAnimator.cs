using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerWeapons;

public class PlayerAnimator : MonoBehaviour {
    [Header("Player Animator")]
    [SerializeField] private float acceleration;
    [SerializeField] private float velocityY;
    [SerializeField] private float velocityX;
    [SerializeField] private float walkLimit;
    [SerializeField] private float runLimit;

    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();

    }

    void Update() {

    }

    public void TrackMovementVelocity(float horizontalInput, float verticalInput) {
        float currentLimit = Input.GetKey(KeyCode.LeftShift) ? runLimit : walkLimit;
        float limitVelocityX = horizontalInput * currentLimit;
        float limitVelocityY = verticalInput * currentLimit;

        if (horizontalInput != 0 || verticalInput != 0) {
            velocityY = Mathf.Lerp(velocityY, limitVelocityY, acceleration * Time.deltaTime);
            velocityX = Mathf.Lerp(velocityX, limitVelocityX, acceleration * Time.deltaTime);
        }
        else {
            velocityX = 0;
            velocityY = 0;
        }

        animator.SetFloat("MovementVelocityX", velocityX);
        animator.SetFloat("MovementVelocityY", velocityY);
    }

    public void PlayAttack() {

        int random_attack = Random.Range(0, 2);

        if (random_attack == 0) {
            animator.SetTrigger("Attack1");
        }
        else {
            animator.SetTrigger("Attack2");
        }
    }

    public void PlayGuard(bool isGuarding) {
        animator.SetBool("Guard", isGuarding);
    }

    public void PlayGetHit() {
        animator.SetTrigger("GetHit");
    }

    public void DeathAnimation() {
        animator.SetTrigger("Death");
    }

}