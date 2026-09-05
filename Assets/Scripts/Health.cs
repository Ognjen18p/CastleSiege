using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {
    [Header("Health")]
    public float health = 100f;
    [SerializeField] private Image healthBar;
    public float maxHealth;

    [Header("Audio")]
    [SerializeField] private AudioClip healthDamageSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    [Header("Health Bar Animation")]
    [SerializeField] private float barLerpSpeed = 5f;

    public bool tookDamage = false;
    public bool currentlyGuarding = false;
    public bool isInvulnerable = false;

    private float targetHealth;
    private ParticleSystem splashParticles;

    private void Start() {
        splashParticles = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        maxHealth = health;
        targetHealth = health;
        currentlyGuarding = false;
        isInvulnerable = false;

        if (splashParticles != null)
            splashParticles.Stop();

        EndSlashEffect();
    }

    private void Update() {
        if (healthBar != null)
            HealthBarAnimation();
    }

    public bool TakeDamage(float damage) {
        if (currentlyGuarding || isInvulnerable) {
            return false;
        }

        if (splashParticles != null)
            splashParticles.Play();

        targetHealth -= damage;
        PlaySound(healthDamageSound);

        if (targetHealth < 0) {
            targetHealth = 0;
        }

        tookDamage = true;

        if (targetHealth <= 0) {
            PlaySound(deathSound);
        }

        return true;
    }

    private void HealthBarAnimation() {
        health = Mathf.Lerp(
            health,
            targetHealth,
            barLerpSpeed * Time.deltaTime
        );
        Debug.Log($"Health: {health}, Target Health: {targetHealth}");
        healthBar.fillAmount = health / maxHealth;

        if (Mathf.Abs(health - targetHealth) < 0.01f) {
            health = targetHealth;
            healthBar.fillAmount = health / maxHealth;
        }
    }

    public bool IsDead() {
        return targetHealth <= 0;
    }

    public void BeginGuard() {
        currentlyGuarding = true;
    }

    public void EndGuard() {
        currentlyGuarding = false;
    }

    public void BeginSlashEffect() {
        GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    public void EndSlashEffect() {
        GetComponentInChildren<TrailRenderer>().enabled = false;
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}