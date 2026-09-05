using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strafing : MonoBehaviour {

    [Header("Strafe Points Inspector")]
    public List<StrafePoint> strafePoints;
    [SerializeField] private GameObject strafePointPref;
    [SerializeField] private float density; 
    private float sizeX;
    private float sizeZ;


    private void Start() {
        sizeX = transform.localScale.x;
        sizeZ = transform.localScale.z; 
        strafePoints = new List<StrafePoint>();
        CreatePoints();
    }

    private void CreatePoints() {
        Vector3 startPos = new Vector3(transform.position.x - sizeX / 2f, transform.position.y + 10, transform.position.z - sizeZ / 2f);
        for (int x = 0; x < sizeX / density; x++) {
            for(int z = 0; z < sizeZ / density; z++) {
                Vector3 pointPos = new Vector3(startPos.x + (x * density), startPos.y, startPos.z + (z * density));
                GameObject newPoint = Instantiate(strafePointPref, pointPos, Quaternion.identity);
                strafePoints.Add(newPoint.GetComponent<StrafePoint>());
            }
        }
        foreach (StrafePoint point in strafePoints) {
            point.GenerateNeighbors(strafePoints, density * 1.5f);
        }

    }

}