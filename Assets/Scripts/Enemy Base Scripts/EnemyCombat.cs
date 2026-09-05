using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour {
    [Header("Combat Chances")]
    [SerializeField] private int defendChance = 80;
    [SerializeField] private int attackChance = 90;
    [SerializeField] private int attackAgainChance = 50;
    [SerializeField] private int strafeBasicAttackChance = 70;

    [Header("Weapon")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip chaseSound;
    private AudioSource audioSource;
    private WeaponCollision weaponCollision;
    
    private EnemyAnimator animator;

    private void Start() {
        animator = GetComponent<EnemyAnimator>();
        if (weapon != null)
            weaponCollision = weapon.GetComponent<WeaponCollision>();
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayAttack() {
        int randomAttack = Random.Range(0, 2);
        if (randomAttack == 0)
            animator.PlayAttack1();
        else
            animator.PlayAttack2();
    }

    public void PlayDefend(bool isDefending) {
        animator.SetDefend(isDefending);
    }
    public void PlayDead() {
        animator.PlayDead();
    }
    public void BeginChase() {
        PlaySound(chaseSound);
    }
    public void BeginAttack() {
        PlaySound(attackSound);
        if (weaponCollision != null)
            weaponCollision.BeginAttack();
    }

    public void EndAttack() {
        if (weaponCollision != null)
            weaponCollision.EndAttack();
    }

    public bool ShouldDefend() {
        return Random.Range(0, 100) < defendChance;
    }

    public bool ShouldAttack() {
        return Random.Range(0, 100) < attackChance;
    }

    public bool ShouldAttackAgain() {
        return Random.Range(0, 100) < attackAgainChance;
    }
    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
    //public bool StrafeBasicAttackCheck() {
    //    bool tokenHolderDefending = EnemyCommunicationLine.getInstance.IsTokenHolderDefending();
    //    return tokenHolderDefending && Random.Range(0, 100) < strafeBasicAttackChance;
    //}
}