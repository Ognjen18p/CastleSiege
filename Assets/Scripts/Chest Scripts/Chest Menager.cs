using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestManager : MonoBehaviour {
    [SerializeField] private GameObject chooseMenu;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            animator.SetTrigger("Open");
        }
    }

    public void Opend() {
        chooseMenu.SetActive(true);
        Time.timeScale = 0;
    }
}