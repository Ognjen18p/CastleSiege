using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour {
    [Header("Weapon Settings")]
    public float damage = 50f;
    public bool canDealDamage = false;

    private List<GameObject> hitTargets = new List<GameObject>();

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    private void Start() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void BeginAttack() {
        hitTargets.Clear();
        canDealDamage = true;
    }

    public void EndAttack() {
        canDealDamage = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (!canDealDamage) return;
        /// IF ENEMY
        EnemyBehaviour enemy = other.GetComponentInParent<EnemyBehaviour>();
        if (enemy != null) {
            EnemyCommunicationLine communication = EnemyCommunicationLine.getInstance;

            if (communication == null ||
                communication.MainTarget != enemy) {
                return;
            }
        }

        if (other.transform.root.CompareTag(transform.root.tag)) return;
        if (hitTargets.Contains(other.gameObject)) return;

        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth == null) return;

        hitTargets.Add(other.gameObject);

        targetHealth.TakeDamage(damage);
    }
}