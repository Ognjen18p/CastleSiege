using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MenuMenager : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    public GameObject menu;
    public GameObject settings;
    public GameObject volume;

    void Start() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update() {
    }

    public void OpenSettings() {
        PlaySound(buttonClickSound);
        settings.SetActive(true);
    }
    public void OutSettings() {
        PlaySound(buttonClickSound);
        settings.SetActive(false);
    }

    public void OpeningVolume() {
        PlaySound(buttonClickSound);
        volume.SetActive(!volume.activeSelf);
    }

    public void PlayGame() {
        PlaySound(buttonClickSound);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Game");
    }
    public void OpenMenu() {
        PlaySound(buttonClickSound);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        PlaySound(buttonClickSound);
        UnityEngine.Application.Quit();
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}
