using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bird : MonoBehaviour {
    [HideInInspector] public bool isLeader;

    private FlockManager manager;
    private Vector3 targetDirection;

    public void Init(FlockManager flockManager, bool leader) {
        manager = flockManager;
        isLeader = leader;
        targetDirection = transform.forward;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update() {
        if (manager == null) return;

        if (!isLeader) {
            ComputeFlockingDirection();
        }
    }

    void FixedUpdate() {
        if (manager == null) return;
        if (targetDirection.sqrMagnitude > 0.001f) {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, manager.turnSpeed * Time.fixedDeltaTime);
        }

        float speed = isLeader ? manager.leaderSpeed : manager.followerSpeed;
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    void ComputeFlockingDirection() {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int neighborCount = 0;

        foreach (Bird other in manager.AllBirds) {
            if (other == this || other == null) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < manager.neighborRadius) {
                neighborCount++;
                alignment += other.transform.forward;
                cohesion += other.transform.position;

                if (dist < manager.separationRadius) {
                    separation += (transform.position - other.transform.position) / Mathf.Max(dist, 0.01f);
                }
            }
        }

        Vector3 desired = Vector3.zero;
        if (manager.Leader != null) {
            Vector3 towardLeader = (manager.Leader.transform.position - transform.position).normalized;
            desired += towardLeader * manager.leaderWeight;
        }

        if (neighborCount > 0) {
            alignment = (alignment / neighborCount).normalized;
            cohesion = ((cohesion / neighborCount) - transform.position).normalized;

            desired += alignment * manager.alignmentWeight;
            desired += cohesion * manager.cohesionWeight;
        }

        desired += separation * manager.separationWeight;
        desired += transform.forward * 0.5f;

        if (desired.sqrMagnitude > 0.001f)
            targetDirection = desired.normalized;
    }

    void OnTriggerEnter(Collider other) {
        if (!isLeader) return;
        if (!other.CompareTag("Border")) return;

        Vector3 towardCenter = (manager.transform.position - transform.position).normalized;

        float randomOffset = Random.Range(-manager.maxBoundaryTurnOffset, manager.maxBoundaryTurnOffset);
        float pitchOffset = Random.Range(-manager.maxBoundaryTurnOffset * 0.3f, manager.maxBoundaryTurnOffset * 0.3f);

        Quaternion offset = Quaternion.Euler(pitchOffset, randomOffset, 0f);
        targetDirection = (offset * towardCenter).normalized;
    }
}