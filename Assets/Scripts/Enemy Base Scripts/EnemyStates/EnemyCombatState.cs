using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatState : EnemyState
{
    private EnemyBehaviour behaviour;
    public EnemyCombatState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }

    public void Enter() {
        
    }

    public void Exit() {
    }

    public void Update() {
        CheckInAttackRange();
        CheckAttack();
        CheckShouldDefend();
        CheckDeath();
    }

    private void CheckInAttackRange() {
        float distance = Vector3.Distance(behaviour.transform.position, behaviour.Player.transform.position);
        if (behaviour.AttackDistance < distance) {
            behaviour.SwitchState(EnemyStateType.Chase);
        }
    }

    private void CheckAttack() {
        if (behaviour.Combat.ShouldComboAttack()) {
            behaviour.Animator.PlayComboAttack();
            return;
        }
        if(behaviour.Combat.ShouldAttack()){
            behaviour.Animator.PlayAttack();
            return;
        }
        if(behaviour.Combat.ShouldCounterAttack()){
            behaviour.Animator.PlayCounterAttack();
            return;
        }
    }

    private void CheckShouldDefend() {
        behaviour.Animator.SetDefend(behaviour.Combat.ShouldDefend());
    }

    private void CheckDeath() {
        if (behaviour.Health.IsDead()) {
            behaviour.SwitchState(EnemyStateType.Dead);
        }
    }
}
