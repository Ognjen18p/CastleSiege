using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseMenu : MonoBehaviour {
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    public bool unactive = false;
    [SerializeField] private GameObject player;
    private PlayerWeapons weapons;

    void Start() {
        weapons = player.GetComponent<PlayerWeapons>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update() {

    }

    public void ChooseAxe() {
        PlaySound(buttonClickSound);
        weapons.set_current_weapon(PlayerWeapons.Weapon.axe);
        Time.timeScale = 1;
        gameObject.SetActive(false);
        unactive = true;
    }

    public void ChooseSword() {
        PlaySound(buttonClickSound);
        weapons.set_current_weapon(PlayerWeapons.Weapon.long_sword);
        Time.timeScale = 1;
        gameObject.SetActive(false);
        unactive = true;
    }
     public void Continue() {
        PlaySound(buttonClickSound);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}