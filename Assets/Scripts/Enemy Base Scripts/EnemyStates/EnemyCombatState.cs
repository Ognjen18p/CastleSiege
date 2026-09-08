using UnityEngine;

public class EnemyCombatState : EnemyState {
    private EnemyBehaviour behaviour;
    public EnemyCombatState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }

    public void Enter() {
    }

    public void Exit() {
    }

    public void Update() {
        if (CheckDeath()) return;
        if (!CheckInAttackRange()) return;
        CheckAttack();
        CheckShouldDefend();
    }

    private bool CheckInAttackRange() {
        float distance = Vector3.Distance(behaviour.transform.position, behaviour.Player.transform.position);
        if (distance > behaviour.AttackDistance + 5f) {
            behaviour.SwitchState(EnemyStateType.Chase);
            return false;
        }
        return true;
    }

    private void CheckAttack() {
        if (!behaviour.Combat.CanAttack) {
            behaviour.Combat.AttackCooldown();
            return;
        }

        int randomPercentage = Random.Range(0, 100);

        if (behaviour.Combat.CounterAttack(randomPercentage)) {
            behaviour.Animator.PlayCounterAttack();
            return;
        }

        if (behaviour.Combat.ComboAttack(randomPercentage)) {
            behaviour.Animator.PlayComboAttack();
            return;
        }

        if (behaviour.Combat.Attack(randomPercentage)) {
            behaviour.Animator.PlayAttack();
            return;
        }
    }
    private void CheckShouldDefend() {
        if (!behaviour.Combat.CanDefend) {
            behaviour.Combat.DefendCooldown();
            return;
        }
        int randomPercentage = Random.Range(0, 100);
        if(behaviour.Combat.ShouldDefend(randomPercentage)){
            behaviour.Animator.SetDefend(true);
        }

    }
    private bool CheckDeath() {
        if (behaviour.Health.IsDead()) {
            behaviour.Animator.PlayDead();
            behaviour.SwitchState(EnemyStateType.Dead);
            return true;
        }
        return false;
    }
}
