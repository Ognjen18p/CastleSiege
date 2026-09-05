using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PauseMenu : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    public GameObject pauseCanvas;
    public GameObject settings;
    public GameObject controls;
    public GameObject volume;

    void Start() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update() {
        if (pauseCanvas == null) return;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pauseCanvas.activeSelf) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }

    }

    public void OpenSettings() {
        PlaySound(buttonClickSound);
        settings.SetActive(true);
    }
    public void OutSettings() {
        PlaySound(buttonClickSound);
        settings.SetActive(false);
    }
    public void OpenControls() {
        PlaySound(buttonClickSound);
        controls.SetActive(true);
    }
    public void OutControls() {
        PlaySound(buttonClickSound);
        controls.SetActive(false);
    }

    public void OpeningVolume() {
        PlaySound(buttonClickSound);
        volume.SetActive(!volume.activeSelf);
    }


    public void BackToMenu() {
        PlaySound(buttonClickSound);
        SceneManager.LoadScene("Menu");
    }

    public void PlayGame() {
        PlaySound(buttonClickSound);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame() {
        PlaySound(buttonClickSound);
        UnityEngine.Application.Quit();
    }
    public void OpenMenu() {
        PlaySound(buttonClickSound);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu");
    }
    public void PauseGame() {
        PlaySound(buttonClickSound);
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame() {
        PlaySound(buttonClickSound);
        Time.timeScale = 1f;

        pauseCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}
