using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerWeapons;

public class PlayerCombat : MonoBehaviour {
    [Header("Attack Settings")]
    private bool inAttack = false;
    private bool inGuard = false;
    private bool inBeginAttack = false;
    private bool manualUnlock = false;

    public bool InAttack => inAttack;
    public bool InGuard => inGuard;
    public bool InBeginAttack => inBeginAttack;
    public bool countered;
    private PlayerAnimator playerAnimator;
    private PlayerMovement playerMovement;
    private List<GameObject> enemies = new List<GameObject>();
    private Health health;
    public bool lockedTarget;
    public GameObject lockTarget;

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
        health.currentlyDefending = false;
        weaponCollision = weapon.GetComponent<WeaponCollision>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        UpdateLockTarget();
        Attack();
        Guard();
        GetHit();
        IsCountered();
    }
    private void UpdateLockTarget() {
        EnemyCommunicationLine communication = EnemyCommunicationLine.getInstance;

        if (communication == null || communication.MainTarget == null) {
            lockTarget = null;
            lockedTarget = false;
            return;
        }

        lockTarget = communication.MainTarget.gameObject;
        lockedTarget = true;
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
            manualUnlock = !manualUnlock;

            if (manualUnlock) {
                lockedTarget = false;
                lockTarget = null;
            }
        }
    }

    private void Guard() {
        inGuard = Input.GetKey(KeyCode.Q);
        playerAnimator.PlayGuard(inGuard);
        health.currentlyDefending = inGuard;
    }

    protected void GetHit() {
        if (health.tookDamage) {
            PlaySound(getHitSound);
            health.tookDamage = false;
        }
    }

    public void ForceStopAttack() {
        Debug.Log("zaustvao");
        inAttack = false;
        inBeginAttack = false;
        EndOfAttack();
        playerAnimator.ResetAnim();
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
            weaponCollision.EndAttack();
        inAttack = false;
    }

    public void IsCountered() {
        if (countered) {
            ForceStopAttack();
            playerAnimator.PlayGetHit();
            countered = false;
        }
    }
}
