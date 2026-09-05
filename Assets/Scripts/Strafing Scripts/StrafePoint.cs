using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafePoint : MonoBehaviour {
    public bool selected;
    public List<StrafePoint> neighbors;

    public void GenerateNeighbors(List<StrafePoint> allPoints, float neighborRange) {
        neighbors = new List<StrafePoint>();
        foreach (var point in allPoints) {
            if (point == this) continue;
            if (Vector3.Distance(transform.position, point.transform.position) <= neighborRange) {
                neighbors.Add(point);
            }
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
