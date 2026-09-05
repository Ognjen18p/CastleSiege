using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardingState : EnemyState {
    private EnemyBehaviour enemyBehaviour;
    public EnemyGuardingState(EnemyBehaviour enemyBehaviour) {
        this.enemyBehaviour = enemyBehaviour;
    }
    void EnemyState.Enter() {
        throw new System.NotImplementedException();
    }

    void EnemyState.Exit() {
        throw new System.NotImplementedException();
    }

    void EnemyState.Update() {
        throw new System.NotImplementedException();
    }
}
