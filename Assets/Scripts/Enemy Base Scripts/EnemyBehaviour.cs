using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    public Dictionary<EnemyStateType, EnemyState> states;
    private EnemyState currentState;
    private EnemyStateType currentStateType;

    [Header("Enemy Components")]
    private EnemyMovement movement;
    private EnemyCombat combat;
    private EnemyAnimator animator;
    private EnemyPathfinding pathfinding;
    private Health health;
    private GameObject player;

    public EnemyMovement Movement => movement;
    public EnemyCombat Combat => combat;
    public EnemyAnimator Animator => animator;
    public EnemyPathfinding Pathfinding => pathfinding; 
    public Health Health => health; 
    public GameObject Player => player; 


    [Header("Enemy Configs")]
    private float targetCloseDistance = 15f;
    private float attackDistance = 20f;
    private float strafeDistance = 50f;
    private float inSightDistance = 200f;
    public float TargetCloseDistance => targetCloseDistance;
    public float AttackDistance => attackDistance;
    public float StrafeDistance => strafeDistance;
    public float InSightDistance => inSightDistance;

    private void Start() {
        states = new Dictionary<EnemyStateType, EnemyState>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        animator = GetComponent<EnemyAnimator>();
        pathfinding = GetComponent<EnemyPathfinding>();
        health = GetComponent<Health>();
        player = GameObject.FindGameObjectWithTag("Player");

        InitializeStates();
        SwitchState(EnemyStateType.Guard);
    }

    private void Update() {
        currentState?.Update();
    }

    private void InitializeStates() {
        states[EnemyStateType.Guard] = new EnemyGuardingState(this);

        states[EnemyStateType.Chase] = new EnemyChaseState(this);
        states[EnemyStateType.Combat] = new EnemyCombatState(this);
        states[EnemyStateType.Strafe] = new EnemyStrafeState(this);
        states[EnemyStateType.Return] = new EnemyReturnState(this);
        states[EnemyStateType.Dead] = new EnemyDeadState(this);
    }

    public void SwitchState(EnemyStateType newStateType) {
        Debug.Log(newStateType);
        if (currentState != null && currentStateType == newStateType) return;
        currentState?.Exit();
        currentStateType = newStateType;
        currentState = states[newStateType];
        currentState?.Enter();
    }

}
