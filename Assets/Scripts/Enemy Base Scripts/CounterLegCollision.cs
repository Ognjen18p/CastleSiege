using System.Collections.Generic;
using UnityEngine;

public class CounterLegCollision : MonoBehaviour {
    private HashSet<GameObject> collidedTargets = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player"))
            return;
        GameObject playerObject = other.gameObject;

        if (collidedTargets.Contains(playerObject))
            return;

        collidedTargets.Add(playerObject);

        PlayerCombat playerCombat =
            playerObject.GetComponentInParent<PlayerCombat>();

        if (playerCombat != null) {
            playerCombat.countered = true;
        }
    }

    public void ResetTargets() {
        collidedTargets.Clear();
    }
}