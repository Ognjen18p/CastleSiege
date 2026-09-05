using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpear : MonoBehaviour {
    [Header("Spear Settings")]
    public float damage;
    public float throwForce;
    private List<GameObject> hitTargets = new List<GameObject>();
    private Rigidbody rb;
    private bool hasHit;
    private GameObject player;
    private PlayerWeapons playerWeapons;
    private bool canBePickedUp = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            playerWeapons = player.GetComponent<PlayerWeapons>();
        }
    }

    private void OnTriggerEnter(Collider other) {
        bool isStuck = transform.parent != null;

        if (isStuck) {
            if (other.CompareTag("Player") && canBePickedUp) {
                CheckTakeBack(other);
            }
            return;
        }

        if (hitTargets.Contains(other.gameObject)) return;

        GameObject enemyTarget = FindEnemyTarget(other.gameObject);
        if (enemyTarget != null) {
            Health targetHealth = enemyTarget.GetComponent<Health>();
            if (targetHealth != null) {
                hitTargets.Add(other.gameObject);
                targetHealth.TakeDamage(damage);
                transform.SetParent(enemyTarget.transform);
                StopSpear();
                hasHit = true;
                canBePickedUp = true;
                return;
            }
        }

        if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
            StopSpear();
            hasHit = true;
            canBePickedUp = true;
        }

        if (other.CompareTag("Player") && canBePickedUp) {
            CheckTakeBack(other);
        }
    }

    private GameObject FindEnemyTarget(GameObject obj) {
        if (obj.CompareTag("Enemy")) {
            return obj;
        }
        Transform current = obj.transform;
        while (current.parent != null) {
            current = current.parent;
            if (current.gameObject.CompareTag("Enemy")) {
                return current.gameObject;
            }
        }
        if (obj.transform.childCount > 0) {
            foreach (Transform child in obj.transform) {
                if (child.gameObject.CompareTag("Enemy")) {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    private void CheckTakeBack(Collider other) {
        if (other.CompareTag("Player") && canBePickedUp) {
            if (playerWeapons != null) {
                playerWeapons.hasSpear = true;
                playerWeapons.set_current_weapon(PlayerWeapons.Weapon.spear);
            }
            Destroy(this.gameObject);
        }
    }

    private void StopSpear() {
        if (rb != null) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    void Update() {
    }
}