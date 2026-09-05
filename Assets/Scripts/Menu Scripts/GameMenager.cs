using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenager : MonoBehaviour {
    public GameObject introPane;

    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip backgroundMusic;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private bool offCursor;

    private void Awake() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (musicSource != null && backgroundMusic != null) {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void Update() {
        if (!offCursor) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            offCursor = true;
        }
    }

    public void CloseIntro() {
        Time.timeScale = 1f;

        PlaySound(buttonClickSound);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        introPane.SetActive(false);
    }

    private void PlaySound(AudioClip clip) {
        if (sfxSource != null && clip != null) {
            sfxSource.PlayOneShot(clip);
        }
    }
}