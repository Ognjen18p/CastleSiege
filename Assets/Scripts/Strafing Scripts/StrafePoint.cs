using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafePoint : MonoBehaviour {
    public bool available = true;
    public bool selected;
    public List<StrafePoint> neighbors;

    public void GenerateNeighbors(List<StrafePoint> allPoints, float neighborRange) {
        neighbors = new List<StrafePoint>();
        foreach (StrafePoint point in allPoints) {
            if (!point.available) continue;
            if (point == this) continue;
            if (Vector3.Distance(transform.position, point.transform.position) <= neighborRange) {
                neighbors.Add(point);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            Debug.Log(other.gameObject);
            available = false;
        }
    }

    public void DebugPaint() {
        if (selected) {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshRenderer.material.color = Color.red;
                return;
            }
        }
        else {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshRenderer.material.color = Color.white;
                return;
            }
        }
    }
}
