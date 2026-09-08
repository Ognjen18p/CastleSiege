using System.Collections.Generic;
using UnityEngine;

public class EnemyCommunicationLine : MonoBehaviour {
    public static EnemyCommunicationLine getInstance;

    private EnemyBehaviour mainTarget;
    private EnemyBehaviour leftTarget;
    private EnemyBehaviour rightTarget;

    public EnemyBehaviour MainTarget => mainTarget;
    public EnemyBehaviour LeftTarget => leftTarget;
    public EnemyBehaviour RightTarget => rightTarget;

    private void Awake() {
        getInstance = this;
    }
    private void Update() {
        CheckMainTarget();
    }

    private void CheckMainTarget() {
        if (mainTarget != null && !mainTarget.Health.IsDead())
            return;

        mainTarget = null;

        PromoteClosestStrafeTarget();
    }
    private void PromoteClosestStrafeTarget() {
        EnemyBehaviour newMainTarget = null;

        if (leftTarget != null && !leftTarget.Health.IsDead())
            newMainTarget = leftTarget;

        if (rightTarget != null && !rightTarget.Health.IsDead()) {
            if (newMainTarget == null) {
                newMainTarget = rightTarget;
            }
            else {
                float leftDistance = Vector3.Distance(leftTarget.transform.position, leftTarget.Player.transform.position);
                float rightDistance = Vector3.Distance(rightTarget.transform.position, rightTarget.Player.transform.position);

                if (rightDistance < leftDistance)
                    newMainTarget = rightTarget;
            }
        }

        if (newMainTarget == null)
            return;

        if (newMainTarget == leftTarget)
            leftTarget = null;

        if (newMainTarget == rightTarget)
            rightTarget = null;

        mainTarget = newMainTarget;
        mainTarget.SwitchState(EnemyStateType.Combat);
    }

    public bool TryTakeMainTarget(EnemyBehaviour enemy) {
        if (mainTarget != null)
            return false;

        mainTarget = enemy;
        return true;
    }

    public int TryTakeStrafeTarget(EnemyBehaviour enemy) {
        int randomSide = Random.Range(0, 2);

        if (randomSide == 0) {
            if (leftTarget == null) {
                leftTarget = enemy;
                return -1;
            }

            if (rightTarget == null) {
                rightTarget = enemy;
                return 1;
            }
        }
        else {
            if (rightTarget == null) {
                rightTarget = enemy;
                return 1;
            }

            if (leftTarget == null) {
                leftTarget = enemy;
                return -1;
            }
        }

        return 0;
    }

    public bool AmIMainTarget(EnemyBehaviour enemy) {
        return mainTarget == enemy;
    }

    public bool AmILeftTarget(EnemyBehaviour enemy) {
        return leftTarget == enemy;
    }

    public bool AmIRightTarget(EnemyBehaviour enemy) {
        return rightTarget == enemy;
    }

    public void ReleaseSlot(EnemyBehaviour enemy) {
        bool releasedMain = false;

        if (AmIMainTarget(enemy)) {
            mainTarget = null;
            releasedMain = true;
        }
        if (AmILeftTarget(enemy))
            leftTarget = null;
        if (AmIRightTarget(enemy))
            rightTarget = null;
        if (releasedMain)
            PromoteClosestStrafeTarget();
    }

    public bool HasFreeStrafeSlot() {
        return leftTarget == null || rightTarget == null;
    }

    public bool IsMainTargetTaken() {
        return mainTarget != null;
    }
}