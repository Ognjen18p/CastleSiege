using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {
    [Header("Player Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotateSpeed;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepsSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;

    public float horizontalInput;
    public float verticalInput;

    private float mouseX;
    private Rigidbody rigidbody;
    private PlayerAnimator playerAnimator;
    private PlayerCombat playerCombat;
    private Health health;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;

        playerAnimator = GetComponent<PlayerAnimator>();
        playerCombat = GetComponent<PlayerCombat>();
        health = GetComponent<Health>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (footstepsSource != null) {
            footstepsSource.loop = true;
            footstepsSource.playOnAwake = false;
        }
    }

    void Update() {
        TrackInput();
        Rotation();
        CheckDeath();
        HandleFootsteps();

    }
    private void FixedUpdate() {
        Movement();
    }
    private void CheckDeath() {
        if (health.health <= 0)
            StartCoroutine(DeathWait());
    }

    IEnumerator DeathWait() {
        playerAnimator.DeathAnimation();

        if (footstepsSource != null)
            footstepsSource.Stop();

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene("Lost");
    }


    private void TrackInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
    }

    private void Movement() {
        if (playerCombat.lockedTarget && playerCombat.lockTarget != null) return;

        bool focused = playerCombat.lockedTarget && playerCombat.lockTarget != null;
        float final_speed = focused ? walkSpeed : Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 move_direction = transform.right * horizontalInput + transform.forward * verticalInput;
        move_direction.Normalize();

        Vector3 targetVelocity = move_direction * final_speed;
        if (move_direction.magnitude < 0.01f)
            targetVelocity = Vector3.zero;

        playerAnimator.TrackMovementVelocity(horizontalInput, verticalInput, focused);
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, targetVelocity, acceleration * Time.deltaTime);
    }

    private void Rotation() {
        if (playerCombat.lockedTarget && playerCombat.lockTarget != null) {
            Vector3 direction = (playerCombat.lockTarget.transform.position - transform.position).normalized;
            direction.y = 0;
            if (direction.magnitude > 0.01f) {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed / 2 * Time.deltaTime);
            }
            return;
        }
        else if (Mathf.Abs(mouseX) > 0.01f) {
            Vector3 rotation = new Vector3(0, mouseX * Time.deltaTime * rotateSpeed, 0);

            transform.Rotate(rotation, Space.Self);
        }
    }

    private void HandleFootsteps() {
        if (footstepsSource == null)
            return;

        bool isMoving = Mathf.Abs(horizontalInput) > 0.01f ||
            Mathf.Abs(verticalInput) > 0.01f;

        if (!isMoving || playerCombat.lockedTarget) {
            if (footstepsSource.isPlaying)
                footstepsSource.Stop();
            return;
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        AudioClip targetClip = isRunning ? runSound : walkSound;

        if (targetClip == null)
            return;

        if (footstepsSource.clip != targetClip) {
            footstepsSource.clip = targetClip;
            footstepsSource.Play();
        }
        else if (!footstepsSource.isPlaying) {
            footstepsSource.Play();
        }
    }
}