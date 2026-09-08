using UnityEngine;

public class EnemyStrafeState : EnemyState {
    private enum StrafeAction {
        Moving,
        Waiting,
        Attacking,
        Returning
    }

    private EnemyBehaviour behaviour;
    private StrafeAction currentAction;
    private int strafeSide;
    private StrafePoint targetPoint;
    private StrafePoint lastStrafePoint;

    private const float minAngle = 40f;
    private const float maxAngle = 90f;

    [SerializeField] private float strafeCooldown = 2.5f;
    [SerializeField] private float attackWaitTime = 1.2f;
    private float strafeTimer;
    private float attackWaitTimer;
    private bool attackPlayed;

    public EnemyStrafeState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }

    public void Enter() {
        EnemyCommunicationLine communication = EnemyCommunicationLine.getInstance;
        if (communication == null)
            return;

        if (communication.AmILeftTarget(behaviour))
            strafeSide = -1;
        else if (communication.AmIRightTarget(behaviour))
            strafeSide = 1;
        else
            strafeSide = communication.TryTakeStrafeTarget(behaviour);

        if (strafeSide == 0) {
            behaviour.SwitchState(EnemyStateType.Chase);
            return;
        }

        targetPoint = FindBestStrafePoint();
        if (targetPoint == null) {
            behaviour.SwitchState(EnemyStateType.Chase);
            return;
        }

        MakePathTo(targetPoint.gameObject);
        currentAction = StrafeAction.Moving;
    }

    public void Update() {
        float distanceToPlayer = behaviour.Movement.DistanceToTarget(behaviour.Player);

        if (distanceToPlayer > behaviour.StrafeDistance + behaviour.TargetCloseDistance) {
            EnemyCommunicationLine.getInstance?.ReleaseSlot(behaviour);
            behaviour.SwitchState(EnemyStateType.Chase);
            return;
        }

        switch (currentAction) {
            case StrafeAction.Moving:
                UpdateMoving();
                break;
            case StrafeAction.Waiting:
                UpdateWaiting();
                break;
            case StrafeAction.Attacking:
                UpdateAttacking();
                break;
            case StrafeAction.Returning:
                UpdateReturning();
                break;
        }
    }

    public void Exit() {
        behaviour.Movement.StopMovement();
        behaviour.Pathfinding.ClearPath();
    }

    private void UpdateMoving() {
        if (!FollowPath(targetPoint.gameObject))
            return;

        behaviour.Movement.StopMovement();
        behaviour.Movement.RotateTowards(behaviour.Player);
        BeginWaiting();
    }

    private void BeginWaiting() {
        currentAction = StrafeAction.Waiting;
        strafeTimer = 0f;
        behaviour.Movement.StopMovement();
        behaviour.Movement.RotateTowards(behaviour.Player);
    }

    private void UpdateWaiting() {
        behaviour.Movement.StopMovement();
        behaviour.Movement.RotateTowards(behaviour.Player);

        strafeTimer += Time.deltaTime;
        if (strafeTimer < strafeCooldown)
            return;

        strafeTimer = 0f;
        ChooseNextAction();
    }

    private void ChooseNextAction() {
        int randomPercentage = Random.Range(0, 100);
        if (behaviour.Combat.StrafeBasicAttack(randomPercentage)) {
            BeginStrafeAttack();
            return;
        }
        ChangeStrafePoint();
    }

    private void BeginStrafeAttack() {
        lastStrafePoint = targetPoint;
        attackPlayed = false;
        attackWaitTimer = 0f;

        behaviour.Pathfinding.ClearPath();
        MakePathTo(behaviour.Player);
        currentAction = StrafeAction.Attacking;
    }

    private void UpdateAttacking() {
        float distanceToPlayer = behaviour.Movement.DistanceToTarget(behaviour.Player);

        if (distanceToPlayer > behaviour.AttackDistance) {
            FollowPath(behaviour.Player);
            return;
        }

        behaviour.Movement.StopMovement();
        behaviour.Movement.RotateTowards(behaviour.Player);

        if (!attackPlayed) {
            behaviour.Animator.PlayAttack();
            attackPlayed = true;
            attackWaitTimer = 0f;
        }

        attackWaitTimer += Time.deltaTime;
        if (attackWaitTimer < attackWaitTime)
            return;

        BeginReturn();
    }

    private void BeginReturn() {
        if (lastStrafePoint == null) {
            BeginWaiting();
            return;
        }

        behaviour.Pathfinding.ClearPath();
        MakePathTo(lastStrafePoint.gameObject);
        currentAction = StrafeAction.Returning;
    }

    private void UpdateReturning() {
        if (!FollowPath(lastStrafePoint.gameObject))
            return;

        targetPoint = lastStrafePoint;
        behaviour.Movement.StopMovement();
        behaviour.Movement.RotateTowards(behaviour.Player);
        BeginWaiting();
    }

    private void ChangeStrafePoint() {
        if (targetPoint == null) {
            BeginWaiting();
            return;
        }

        foreach (StrafePoint neighbor in targetPoint.neighbors) {
            if (neighbor == null)
                continue;

            Vector3 direction = neighbor.transform.position - behaviour.Player.transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude <= 0.01f)
                continue;

            float angle = Vector3.SignedAngle(behaviour.Player.transform.forward, direction.normalized, Vector3.up);
            float distance = Vector3.Distance(neighbor.transform.position, behaviour.Player.transform.position);
            bool correctSide = strafeSide < 0 ? angle <= -minAngle && angle >= -maxAngle : angle >= minAngle && angle <= maxAngle;
            bool correctDistance = Mathf.Abs(distance - behaviour.StrafeDistance) <= behaviour.TargetCloseDistance;

            if (!correctSide || !correctDistance)
                continue;

            targetPoint = neighbor;
            behaviour.Pathfinding.ClearPath();
            MakePathTo(targetPoint.gameObject);
            currentAction = StrafeAction.Moving;
            return;
        }
        BeginWaiting();
    }

    private void MakePathTo(GameObject destination) {
        behaviour.Pathfinding.MakePathTo(destination);
    }

    private bool FollowPath(GameObject finalDestination) {
        if (behaviour.Pathfinding.Path == null || behaviour.Pathfinding.Path.Count == 0) {
            return behaviour.Movement.DistanceToTarget(finalDestination) <= behaviour.TargetCloseDistance;
        }

        StrafePoint nextPoint = behaviour.Pathfinding.Path[0];
        behaviour.Movement.MoveTo(nextPoint.gameObject, behaviour.Player);

        if (behaviour.Movement.distanceToNextPoint <= behaviour.TargetCloseDistance) {
            behaviour.Pathfinding.Path.RemoveAt(0);
        }
        return false;
    }

    private StrafePoint FindBestStrafePoint() {
        StrafePoint bestPoint = null;
        float bestScore = float.MaxValue;

        foreach (StrafePoint point in behaviour.Pathfinding.strafing.strafePoints) {
            if (point == null)
                continue;
            Vector3 direction = point.transform.position - behaviour.Player.transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude <= 0.01f)
                continue;
            float angle = Vector3.SignedAngle(behaviour.Player.transform.forward, direction.normalized, Vector3.up);
            bool correctSide;

            if (strafeSide < 0) {
                correctSide = angle <= -minAngle && angle >= -maxAngle;
            }
            else {
                correctSide = angle >= minAngle && angle <= maxAngle;
            }
            if (!correctSide)
                continue;
            float distanceToPlayer = Vector3.Distance(point.transform.position, behaviour.Player.transform.position);
            float distanceDifference = Mathf.Abs(distanceToPlayer - behaviour.StrafeDistance);

            if (distanceDifference > behaviour.TargetCloseDistance)
                continue;
            if (distanceDifference < bestScore) {
                bestScore = distanceDifference;
                bestPoint = point;
            }
        }
        return bestPoint;
    }
}