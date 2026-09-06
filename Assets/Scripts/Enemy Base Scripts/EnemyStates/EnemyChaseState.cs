using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private EnemyBehaviour behaviour;
    public EnemyChaseState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }
    void EnemyState.Enter() {
        behaviour.Pathfinding.MakePathTo(behaviour.Player);
        if(behaviour.Pathfinding.Path == null || behaviour.Pathfinding.Path.Count == 0) {
            Debug.Log("Nisam naso put iz Chase");
        }
    }

    void EnemyState.Exit() {
    }

    void EnemyState.Update() {
        ChasePlayer();
    }

    private void ChasePlayer() {
        if (behaviour.Pathfinding.Path == null || behaviour.Pathfinding.Path.Count == 0) {
            return;
        }
        behaviour.Movement.ChaseAt(behaviour.Pathfinding.Path[0]?.gameObject, behaviour.Player);
        if (!behaviour.Pathfinding.IsEndPointInRange(behaviour.Player, behaviour.AttackDistance)) {
            if (behaviour.Movement.DistanceToTarget(behaviour.Player) > behaviour.InSightDistance + behaviour.AttackDistance) {
                behaviour.Pathfinding.ClearPath();
                behaviour.SwitchState(EnemyStateType.Return);
                return;
            }
            behaviour.Pathfinding.ClearPath();
            behaviour.Pathfinding.MakePathTo(behaviour.Player);
        }

        if (behaviour.Movement.distanceToNextPoint <= behaviour.TargetCloseDistance)
            behaviour.Pathfinding.Path.RemoveAt(0);

        if (behaviour.Movement.DistanceToTarget(behaviour.Player) <= behaviour.AttackDistance) {
            behaviour.Pathfinding.ClearPath();
            behaviour.SwitchState(EnemyStateType.Combat);
        }
    }
}
