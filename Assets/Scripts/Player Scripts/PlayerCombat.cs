using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerWeapons;

public class PlayerCombat : MonoBehaviour {
    [Header("Attack Settings")]
    private bool inAttack = false;
    private bool inGuard = false;
    private bool inBeginAttack = false;
    public bool InAttack => inAttack;
    public bool InGuard => inGuard;
    public bool InBeginAttack => inBeginAttack;
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
            if (!inAttack) {
                playerAnimator.PlayAttack();
                inAttack = true;
                inBeginAttack = true;
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            playerAnimator.Special();
        }
    }
    public void EndAttack() {
        weaponCollision.EndAttack();
        inAttack = false;
    }
    private void Guard() {
        inGuard = Input.GetKey(KeyCode.Q);
        playerAnimator.PlayGuard(inGuard);
        health.currentlyGuarding = inGuard;
    }

    protected void GetHit() {
        if (health.tookDamage) {
            PlaySound(getHitSound);
            health.tookDamage = false;
        }
    }

    public void ForceStopAttack() {
        inAttack = false;
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

        inBeginAttack = false;
    }

    public void EndOfAttack() {
        if (weaponCollision != null)
            EndAttack();
    }
}