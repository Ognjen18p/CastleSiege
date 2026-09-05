using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTrigger : MonoBehaviour {
    public GameObject introPane;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player entered the trigger");
            //introPane.SetActive(true);
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            //this.gameObject.SetActive(false);
        }
    }
}
