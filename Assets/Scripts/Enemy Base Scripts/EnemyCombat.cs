using UnityEngine;

public class EnemyCombat : MonoBehaviour {
    [Header("Combat Chances")]
    [SerializeField] private int defendChance = 45;
    [SerializeField] private int counterAttackChance = 20;
    [SerializeField] private int comboAttackChance = 25;
    [SerializeField] private int strafeBasicAttackChance = 40;
    [SerializeField] private int attackChance = 70;

    [Header("Timer")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float strafeAttackCooldown = 3f;
    [SerializeField] private float defendCooldown = 2f;
    private float attackTimer = 0;
    private float defendTimer = 0;
    private bool canAttack = true;
    public bool CanAttack => canAttack;
    private bool canDefend = true;
    public bool CanDefend => canDefend;

    private float strafeAttackTimer = 0f;
    private bool canStrafeAttack = true;

    public bool CanStrafeAttack => canStrafeAttack;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip chaseSound;

    private PlayerCombat playerCombat;
    private AudioSource audioSource;
    private Health health;
    private WeaponCollision weaponCollision;
    private CounterLegCollision counterLegCollision;
    private Collider counterLegCollider;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        weaponCollision = GetComponentInChildren<WeaponCollision>();
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        GameObject counterLeg = GameObject.FindGameObjectWithTag("CounterLeg");
        if (counterLeg != null){
            counterLegCollider = counterLeg.GetComponent<Collider>();
            counterLegCollision = counterLeg.GetComponent<CounterLegCollision>();
        }
        if (counterLegCollider != null)
            counterLegCollider.enabled = false;
    }

    public bool Attack(int valuePercentage) {
        if (!canAttack) return false;

        if (playerCombat != null && !playerCombat.InGuard) {
            if (valuePercentage < attackChance) {
                canAttack = false;
                return true;
            }
        }
        return false;
    }

    public bool CounterAttack(int valuePercentage) {
        if (!canAttack) return false;
        if (playerCombat != null && playerCombat.InBeginAttack) {
            if (valuePercentage < counterAttackChance) {
                canAttack = false;
                return true;
            }
        }
        return false;
    }

    public bool ComboAttack(int valuePercentage) {
        if (!canAttack) return false;

        if (playerCombat != null && !playerCombat.InGuard) {
            if (valuePercentage < comboAttackChance) {
                canAttack = false;
                return true;
            }
        }
        return false;
    }

    public void AttackCooldown() {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown) {
            canAttack = true;
            attackTimer = 0;
        }
    }
    
    public bool StrafeBasicAttack(int valuePercentage) {
        if (playerCombat != null && playerCombat.InAttack) {
            if (valuePercentage < strafeBasicAttackChance) {
                return true;
            }
        }
        return false;
    }

    public bool ShouldDefend(int valuePercentage) {
        if (!canDefend) return false;
        if (playerCombat != null && playerCombat.InBeginAttack && !health.currentlyDefending) {
            if (valuePercentage < defendChance) {
                canDefend = false;
                return true;
            }
        }
        return false;
    }
    public void DefendCooldown() {
        defendTimer += Time.deltaTime;
        if (defendTimer >= defendCooldown) {
            canDefend = true;
            defendTimer = 0;
        }
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

    public void FinishDefend() {
        health.EndDefend();
        EnemyAnimator enemyAnimator = GetComponent<EnemyAnimator>();
        if (enemyAnimator != null) {
            enemyAnimator.SetDefend(false);
        }
    }
    public void BeginCounterAttack() {
        counterLegCollision?.ResetTargets();
        ApplyKnockback(playerCombat.gameObject, 300f);

        if (counterLegCollider != null)
            counterLegCollider.enabled = true;
    }

    public void EndCounterAttack() {
        if (counterLegCollider != null)
            counterLegCollider.enabled = false;
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
    public void ApplyKnockback(GameObject obj, float force) {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 direction = -obj.transform.forward * force;

        rb.AddForce(direction * force, ForceMode.Impulse);
        Debug.Log(rb.velocity);
    }

}