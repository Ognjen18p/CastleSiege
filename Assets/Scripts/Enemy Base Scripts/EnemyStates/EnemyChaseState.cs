
using UnityEngine;

public class EnemyChaseState : EnemyState {
    private EnemyBehaviour behaviour;
    public EnemyChaseState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }
    void EnemyState.Enter() {
        EnemyCommunicationLine.getInstance?.ReleaseSlot(behaviour);
        behaviour.Pathfinding.MakePathTo(behaviour.Player);
        if (behaviour.Pathfinding.Path == null || behaviour.Pathfinding.Path.Count == 0) {
            Debug.Log("Nisam naso put iz Chase");
        }
    }

    void EnemyState.Exit() {
        behaviour.Movement.StopMovement();
        behaviour.Pathfinding.ClearPath();
    }

    void EnemyState.Update() {
        ChasePlayer();
    }

    private void ChasePlayer() {
        float distanceToPlayer = behaviour.Movement.DistanceToTarget(behaviour.Player);

        EnemyCommunicationLine communication = EnemyCommunicationLine.getInstance;
        if (distanceToPlayer <= behaviour.StrafeDistance) {
            if (!communication.IsMainTargetTaken()) {
                communication.TryTakeMainTarget(behaviour);
            }

            if (!communication.AmIMainTarget(behaviour)) {
                int side = communication.TryTakeStrafeTarget(behaviour);
                if (side != 0) {
                    behaviour.SwitchState(EnemyStateType.Strafe);
                    return;
                }
            }
        }
        if (distanceToPlayer <= behaviour.AttackDistance && communication.AmIMainTarget(behaviour)) {
            behaviour.SwitchState(EnemyStateType.Combat);
            return;
        }

        if (behaviour.Pathfinding.Path == null || behaviour.Pathfinding.Path.Count == 0) {
            behaviour.Pathfinding.MakePathTo(behaviour.Player);
            return;
        }
        StrafePoint currentPoint = behaviour.Pathfinding.Path[0];
        behaviour.Movement.ChaseAt(currentPoint?.gameObject, currentPoint.gameObject);
        if (!behaviour.Pathfinding.IsEndPointInRange(behaviour.Player, behaviour.AttackDistance)) {
            if (!behaviour.Movement.PlayerInSight) {
                behaviour.SwitchState(EnemyStateType.Return);
                return;
            }
            behaviour.Pathfinding.MakePathTo(behaviour.Player);
        }
        if (behaviour.Movement.distanceToNextPoint <= behaviour.TargetCloseDistance) {
            behaviour.Pathfinding.Path.RemoveAt(0);
        }
    }
}
