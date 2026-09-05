using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBehaviour : MonoBehaviour {
    [Header("Stone Behaviour Inspector")]
    [SerializeField] private float xOffsetForHandPlacement;
    [SerializeField] private float yOffsetForHandPlacement;
    [SerializeField] private float zOffsetForHandPlacement;

    public GameObject bossHand;
    public GameObject stonePrefab;

    public GameObject boss;

    public bool taken = false;
    public bool thrown = false;

    // Update is called once per frame
    void Update() {
        Behaviour();
    }

    void Behaviour() {
        if (taken)
            transform.position = new Vector3(bossHand.transform.position.x + xOffsetForHandPlacement, bossHand.transform.position.y - yOffsetForHandPlacement, bossHand.transform.position.z - zOffsetForHandPlacement);
        if (thrown) {
            Instantiate(stonePrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
            Destroy(gameObject, 1);
            taken = false;
            thrown = false;
        }
    }

}
