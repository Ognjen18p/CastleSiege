using UnityEngine;

public class EnemyReturnState : EnemyState {
    private EnemyBehaviour behaviour;
    private float returnWaitTimer;
    private const float returnWaitTime = 1.5f;
    private bool pathCreated;

    public EnemyReturnState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }

    public void Enter() {
        behaviour.Movement.StopMovement();
        EnemyCommunicationLine.getInstance?.ReleaseSlot(behaviour);
        returnWaitTimer = 0f;
        pathCreated = false;
        behaviour.Pathfinding.ClearPath();
    }

    public void Update() {
        if (behaviour.Movement.PlayerInSight) {
            behaviour.SwitchState(EnemyStateType.Chase);
            return;
        }

        if (!pathCreated) {
            returnWaitTimer += Time.deltaTime;
            behaviour.Movement.StopMovement();
            if (returnWaitTimer < returnWaitTime)
                return;

            behaviour.Pathfinding.MakePathTo(behaviour.Movement.GuardingPoint);
            pathCreated = true;
        }

        ReturnToGuardPoint();
    }

    public void Exit() {
        behaviour.Movement.StopMovement();
        behaviour.Movement.LookAt(behaviour.Movement.GuardingSightPoint);
        behaviour.Pathfinding.ClearPath();
    }

    private void ReturnToGuardPoint() {
        float distanceToGuardPoint = behaviour.Movement.DistanceToTarget(behaviour.Movement.GuardingPoint);

        if (distanceToGuardPoint <= behaviour.TargetCloseDistance + 5f) {
            behaviour.Movement.StopMovement();
            behaviour.Movement.LookAt(behaviour.Movement.GuardingSightPoint);
            behaviour.SwitchState(EnemyStateType.Guard);
            return;
        }

        if (behaviour.Pathfinding.Path == null || behaviour.Pathfinding.Path.Count == 0) {
            behaviour.Pathfinding.MakePathTo(behaviour.Movement.GuardingPoint);
            return;
        }

        StrafePoint nextPoint = behaviour.Pathfinding.Path[0];
        behaviour.Movement.MoveTo(nextPoint.gameObject, behaviour.Movement.GuardingPoint);

        if (behaviour.Movement.distanceToNextPoint <= behaviour.TargetCloseDistance) {
            behaviour.Pathfinding.Path.RemoveAt(0);
        }
    }
}