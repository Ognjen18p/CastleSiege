using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommunicationLine : MonoBehaviour {
    public static EnemyCommunicationLine getInstance;

    private List<EnemyBehaviour> allies = new List<EnemyBehaviour>();
    private EnemyBehaviour tokenHolder;

    private void Awake() { getInstance = this; }

    public bool TryTakeToken(EnemyBehaviour requester) {
        if (tokenHolder != null) return false;
        tokenHolder = requester;
        return true;
    }

    public bool IsTokenTaken() {
        return tokenHolder != null;
    }

    public bool AmITokenHolder(EnemyBehaviour enemyBehaviour) {
        return tokenHolder == enemyBehaviour;
    }

    public bool IsTokenHolderDefending() {
        if (tokenHolder == null) return false;
        return false;
    }

    public void ReleaseToken(EnemyBehaviour holder, EnemyBehaviour newHolder) {
        if (tokenHolder == holder) {
            tokenHolder = newHolder;
            AssignTokenToClosest();
        }
    }

    private void AssignTokenToClosest() {
        EnemyBehaviour closest = null;
        float clossestDistance = float.MaxValue;
        foreach (EnemyBehaviour ally in allies) {
            if (ally == null) continue;
            float distance = Vector3.Distance(ally.Player.transform.position, ally.transform.position);
            if (distance < clossestDistance) {
                clossestDistance = distance;
                closest = ally;
            }
        }
        if (closest != null) {
            tokenHolder = closest;
            Health health = tokenHolder.gameObject.GetComponent<Health>();
            health.isInvulnerable = false;
        }
    }

    public void AddAlly(EnemyBehaviour enemy) {
        allies.Add(enemy);
        if (tokenHolder != null) {
            Health health = enemy.gameObject.GetComponent<Health>();
            health.isInvulnerable = true;
            return;
        }
        AssignTokenToClosest();
    }
    public void RemoveAlly(EnemyBehaviour enemy) {
        allies.Remove(enemy);
        AssignTokenToClosest();
    }
}