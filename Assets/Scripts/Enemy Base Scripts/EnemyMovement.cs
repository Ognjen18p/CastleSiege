using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float walkSpeed;
    public float runSpeed;
    [SerializeField] protected float rotationSpeed;

    public float distanceToStrafePoint { get; private set; }
    private Rigidbody rb;
    private EnemyAnimator animator;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<EnemyAnimator>();
    }
    private void Update() {
        animator.TrackMovementVelocity(rb.velocity.x, rb.velocity.z);
    }

    public float DistanceToTarget(GameObject target) {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    protected void ResetVelocityAndSpeed() {
        rb.velocity = Vector3.zero;
    }

    public void LookAt(GameObject target) {
        transform.LookAt(target.transform.position);
    }

    public void RotateTowards(GameObject point) {
        Vector3 rotateDirection = (point.transform.position - transform.position).normalized;
        rotateDirection.y = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rotateDirection), rotationSpeed * Time.deltaTime);
    }

    public void MoveTowards(Vector3 direction, float speed) {
        rb.velocity = direction * speed;
    }

    public void ChaseAt(GameObject point, GameObject lookAt) {
        distanceToStrafePoint = Vector3.Distance(transform.position, point.transform.position);
        RotateTowards(lookAt);
        MoveTowards(transform.forward, runSpeed);
    }

    public void MoveTo(GameObject point, GameObject lookAt) {
        distanceToStrafePoint = Vector3.Distance(transform.position, point.transform.position);
        Vector3 direction = (point.transform.position - transform.position).normalized;
        RotateTowards(lookAt);
        MoveTowards(direction, walkSpeed);
    }

    public void StopMovement() {
        rb.velocity = Vector3.zero;
    }
}
