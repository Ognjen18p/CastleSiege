using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnemyBehaviour : MonoBehaviour {
    [SerializeField] private GameObject guardingPoint;
    [SerializeField] private GameObject guardingSightPoint;
    [SerializeField] private GameObject questionMark;
    [SerializeField] private GameObject dangerMark;

    [Header("Audio")]
    [SerializeField] private AudioSource chaseAudioSource;
    [SerializeField] private AudioClip chaseRunSound;

    public GameObject player;

    private PlayerCombat playerCombat;
    private EnemyMovement movement;
    private EnemyCombat combat;
    private EnemyPathfinding pathfinding;
    private List<StrafePoint> path = new List<StrafePoint>();
    private Health health;

    private float pointCloseDistance = 10f;
    private float attackDistance = 30f;
    private float strafeDistance = 60f;
    private float chaseDistance = 300f;

    private bool playerInSight;
    private bool isAttackChecking;

    public enum EnemyState {
        Idle,
        Guard,
        Return,
        Chase,
        Strafe,
        Attack,
        Defend,
        Dead
    }

    public EnemyState currentState;

    public void RandomState(EnemyState[] one_of_states, EnemyState favourable, int favourable_chance) {
        int random = Random.Range(0, one_of_states.Length);
        int chance = Random.Range(0, 100);

        if (chance <= favourable_chance)
            currentState = one_of_states[random];
        else
            currentState = favourable;
    }

    private void Start() {
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        pathfinding = GetComponent<EnemyPathfinding>();
        playerCombat = player.GetComponent<PlayerCombat>();
        health = GetComponent<Health>();
        currentState = EnemyState.Guard;

        if (chaseAudioSource != null) {
            chaseAudioSource.loop = true;
            chaseAudioSource.playOnAwake = false;
        }
    }

    private void Update() {
        StateTracker();
        CheckDeath();
    }

    private void CheckDeath() {
        if (health.health <= 0) {
            SceneManager.LoadScene("Win");
        }
    }

    private void UpdateVisualMarks(EnemyState state) {
        bool showDanger = state == EnemyState.Chase || state == EnemyState.Attack || state == EnemyState.Defend || state == EnemyState.Idle;
        bool playDanger = state == EnemyState.Attack || state == EnemyState.Defend;
        bool showQuestion = state == EnemyState.Return;

        if (dangerMark != null)
            dangerMark.gameObject.SetActive(showDanger);

        if (questionMark != null)
            questionMark.SetActive(showQuestion);
    }

    private void StateSwitchTo(EnemyState newState) {
        if (newState != EnemyState.Chase)
            StopChaseSound();

        currentState = newState;

        switch (currentState) {
            case EnemyState.Idle:
                StartCoroutine(IdleWait());
                break;

            case EnemyState.Guard:
                dangerMark.SetActive(false);
                questionMark.SetActive(false);
                movement.LookAt(guardingSightPoint);
                movement.StopMovement();
                break;

            case EnemyState.Return:
                movement.StopMovement();
                StartCoroutine(ReturnWait());
                break;

            case EnemyState.Chase:
                StartChaseSound();
                combat.BeginChase();
                FindNewPathTowards(player);
                break;

            case EnemyState.Attack:
                pathfinding.ClearPath();
                movement.StopMovement();
                break;

            case EnemyState.Defend:
                if (combat.ShouldDefend())
                    combat.PlayDefend(true);
                break;

            case EnemyState.Dead:
                StopChaseSound();
                combat.PlayDead();
                StartCoroutine(DeathWait());
                break;
        }

        UpdateVisualMarks(newState);
    }

    private void StateTracker() {
        if (health.health <= 0)
            StateSwitchTo(EnemyState.Dead);

        switch (currentState) {
            case EnemyState.Idle:
                if (IsPlayerAttacking())
                    StateSwitchTo(EnemyState.Defend);
                break;

            case EnemyState.Guard:
                movement.RotateTowards(guardingSightPoint);
                CheckPlayerInSight();
                break;

            case EnemyState.Return:
                CheckPlayerInSight();
                ReturnThroughPath();
                break;

            case EnemyState.Chase:
                ChaseThroughPath();
                break;

            case EnemyState.Attack:
                movement.RotateTowards(player);

                if (!IsPlayerInRange())
                    break;

                if (IsPlayerAttacking()) {
                    StateSwitchTo(EnemyState.Defend);
                    break;
                }

                if (combat.ShouldAttack())
                    combat.PlayAttack();
                else
                    StateSwitchTo(EnemyState.Idle);

                break;

            case EnemyState.Defend:
                if (!IsPlayerAttacking()) {
                    combat.PlayDefend(false);
                    StateSwitchTo(EnemyState.Attack);
                }
                break;

            case EnemyState.Dead:
                break;
        }
    }

    private void StartChaseSound() {
        if (chaseAudioSource == null || chaseRunSound == null)
            return;

        chaseAudioSource.clip = chaseRunSound;

        if (!chaseAudioSource.isPlaying)
            chaseAudioSource.Play();
    }

    private void StopChaseSound() {
        if (chaseAudioSource != null && chaseAudioSource.isPlaying)
            chaseAudioSource.Stop();
    }

    private void FindNewPathTowards(GameObject target) {
        pathfinding.ClearPath();
        path = pathfinding.GetAStarPath(target);
    }

    private void CheckPlayerInSight() {
        if (playerInSight) {
            if (currentState != EnemyState.Chase)
                StateSwitchTo(EnemyState.Chase);
        }
    }

    private void ChaseThroughPath() {
        if (path == null || path.Count == 0)
            return;

        StrafePoint nextPoint = path[0];
        movement.ChaseAt(nextPoint.gameObject, player);

        if (!pathfinding.IsTargetInGap(player, attackDistance)) {
            if (!playerInSight && movement.DistanceToTarget(player) > chaseDistance + attackDistance) {
                StateSwitchTo(EnemyState.Return);
                path.Clear();
                return;
            }

            FindNewPathTowards(player);
            return;
        }

        if (movement.distanceToStrafePoint <= pointCloseDistance)
            path.RemoveAt(0);

        if (movement.DistanceToTarget(player) <= attackDistance) {
            pathfinding.ClearPath();
            path.Clear();
            StateSwitchTo(EnemyState.Attack);
        }
    }

    private void ReturnThroughPath() {
        if (path == null || path.Count == 0)
            return;

        StrafePoint nextPoint = path[0];
        movement.MoveTo(nextPoint.gameObject, guardingPoint);

        if (movement.distanceToStrafePoint <= pointCloseDistance)
            path.RemoveAt(0);

        if (movement.DistanceToTarget(guardingPoint) <= pointCloseDistance) {
            pathfinding.ClearPath();
            movement.LookAt(guardingSightPoint);
            StateSwitchTo(EnemyState.Guard);
        }
    }

    IEnumerator ReturnWait() {
        yield return new WaitForSeconds(2f);

        if (currentState == EnemyState.Return) {
            pathfinding.ClearPath();
            FindNewPathTowards(guardingPoint);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player"))
            playerInSight = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player"))
            playerInSight = false;
    }

    private bool IsPlayerAttacking() {
        if (playerCombat == null)
            return false;

        return playerCombat.isAttack;
    }

    private bool IsPlayerInRange() {
        if (movement.DistanceToTarget(player) > attackDistance) {
            StateSwitchTo(EnemyState.Chase);
            return false;
        }

        return true;
    }

    IEnumerator IdleWait() {
        yield return new WaitForSeconds(2f);

        if (currentState != EnemyState.Idle)
            yield break;

        StateSwitchTo(EnemyState.Attack);
    }

    IEnumerator DeathWait() {
        StopChaseSound();
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Win");
    }
}