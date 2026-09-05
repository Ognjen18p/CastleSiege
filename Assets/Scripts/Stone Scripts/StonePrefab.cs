using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePrefab : MonoBehaviour {
    private GameObject player;
    private Rigidbody rb;
    public float force;
    private bool thrown = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    void Start() {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0.5f; 
        }
    }

    // Update is called once per frame
    void Update() {
        if (!thrown) {
            Vector3 direction = player.transform.position - transform.position;
            direction.y += Random.Range(-2, 2) + 2;
            direction.x += Random.Range(-2, 2) + 2;
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
            Destroy(gameObject, 3);
            thrown = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            PlaySound(hitSound);

            player.GetComponent<Health>().TakeDamage(100);

            StartCoroutine(DestroyAfterSound());
        }
    }

    private IEnumerator DestroyAfterSound() {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}