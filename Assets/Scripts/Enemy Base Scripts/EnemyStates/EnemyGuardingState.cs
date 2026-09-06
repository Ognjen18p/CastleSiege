using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardingState : EnemyState {
    private EnemyBehaviour behaviour;
    public EnemyGuardingState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }
    void EnemyState.Enter() {
        behaviour.Movement.StopMovement();
    }

    void EnemyState.Exit() {
    }

    void EnemyState.Update() {
        if(behaviour.PlayerInSight) {
            behaviour.SwitchState(EnemyStateType.Chase);
        }
    }
}

