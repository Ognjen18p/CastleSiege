using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerWeapons;

public class PlayerCombat : MonoBehaviour {
    [Header("Attack Settings")]
    public bool isAttack = false;
    public bool isGuard = false;
    [SerializeField] private float attackCooldown = 1.2f;
    private PlayerAnimator playerAnimator;
    private Health health;

    [Header("Weapon")]
    public GameObject weapon;
    private WeaponCollision weaponCollision;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip getHitSound;
    [SerializeField] private AudioClip attackSound;

    void Start() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        health.currentlyGuarding = false;
        weaponCollision = weapon.GetComponent<WeaponCollision>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    void Update() {
        Attack();
        Guard();
        GetHit();
    }

    private void Attack() {
        if (Input.GetMouseButtonDown(0)) {
            if (!isAttack) {
                playerAnimator.PlayAttack();
                isAttack = true;
                StartCoroutine(AttackCooldown());
            }
        }
    }
    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldown);
        isAttack = false;
    }
    private void Guard() {
        isGuard = Input.GetKey(KeyCode.Q);
        playerAnimator.PlayGuard(isGuard);
        health.currentlyGuarding = isGuard;
    }

    protected void GetHit() {
        if (health.tookDamage) {
            PlaySound(getHitSound);
            health.tookDamage = false;
        }
    }

    public void ForceStopAttack() {
        isAttack = false;
        StopAllCoroutines();
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    public void BeginingOfAttack() {
        if (audioSource != null && attackSound != null) {
            audioSource.PlayOneShot(attackSound);
        }

        if (weaponCollision != null)
            weaponCollision.BeginAttack();
    }

    public void EndOfAttack() {
        if (weaponCollision != null)
            weaponCollision.EndAttack();
    }
}