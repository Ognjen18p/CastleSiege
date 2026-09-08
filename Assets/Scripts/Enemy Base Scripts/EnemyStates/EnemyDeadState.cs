using UnityEngine;

public class EnemyDeadState : EnemyState {
    private EnemyBehaviour behaviour;

    private float deadTimer = 0f;
    private float deadDelay = 3f;

    public EnemyDeadState(EnemyBehaviour enemyBehaviour) {
        behaviour = enemyBehaviour;
    }

    public void Enter() {
        behaviour.Movement.StopMovement();

        EnemyCommunicationLine.getInstance?.ReleaseSlot(behaviour);

        deadTimer = 0f;
    }

    public void Update() {
        deadTimer += Time.deltaTime;

        if (deadTimer >= deadDelay) {
            Object.Destroy(behaviour.gameObject);
        }
    }

    public void Exit() {
    }
}