using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour {
    [Header("Combat Chances")]
    [SerializeField] private int defendChance = 80;
    [SerializeField] private int attackChance = 90;
    [SerializeField] private int comboAttackChance = 50;
    [SerializeField] private int counterAttackChance = 40;
    [SerializeField] private int strafeBasicAttackChance = 60;

    [Header("Timer")]
    [SerializeField] private float attackCooldown = 1.1f;
    private bool canAttack = true;
    private bool isDefending = false;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip chaseSound;
    private GameObject player;
    private PlayerCombat playerCombat;
    private AudioSource audioSource;
    private GameObject weapon;
    private WeaponCollision weaponCollision;

    private void Start() {
        if (weapon != null)
            weaponCollision = weapon.GetComponent<WeaponCollision>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCombat = player.GetComponent<PlayerCombat>();
    }

    public bool ShouldCounterAttack() {
        if (!canAttack) return false;
        if (playerCombat != null && playerCombat.InBeginAttack) {
            int randomValue = Random.Range(0, 100);
            if (randomValue < counterAttackChance) {
                canAttack = false;
                StartCoroutine(AttackCooldown());
                return true;
            }
        }
        return false;
    }

    public bool ShouldStrafeBasicAttack() {
        if (!canAttack) return false;

        if (playerCombat != null && playerCombat.InAttack) {
            int randomValue = Random.Range(0, 100);
            if (randomValue < strafeBasicAttackChance) {
                canAttack = false;
                StartCoroutine(AttackCooldown());
                return true;
            }
        }
        return false;
    }

    public bool ShouldAttack() {
        if (!canAttack) return false;

        if (playerCombat != null && !playerCombat.InGuard) {
            int randomValue = Random.Range(0, 100);
            if (randomValue < attackChance) {
                canAttack = false;
                StartCoroutine(AttackCooldown());
                return true;
            }
        }
        return false;
    }

    public bool ShouldComboAttack() {
        if (!canAttack) return false;

        if (playerCombat != null && !playerCombat.InGuard) {
            int randomValue = Random.Range(0, 100);
            if (randomValue < comboAttackChance) {
                canAttack = false;
                StartCoroutine(AttackCooldown());
                return true;
            }
        }
        return false;
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public bool ShouldDefend() {
        if (playerCombat != null && !playerCombat.InAttack) {
            isDefending = false;
            return false;
        }

        if (isDefending && playerCombat != null && playerCombat.InAttack) {
            return true;
        }
        if (playerCombat != null && playerCombat.InAttack) {
            int randomValue = Random.Range(0, 100);
            if (randomValue < defendChance) {
                isDefending = true;
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// /Animator calls this function to begin the attack. It plays the attack sound and enables the weapon collision.
    /// </summary>
    public void BeginAttack() {
        PlaySound(attackSound);
        if (weaponCollision != null)
            weaponCollision.BeginAttack();
    }

    public void EndAttack() {
        if (weaponCollision != null)
            weaponCollision.EndAttack();
    }


    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}