using System.Collections.Generic;
using UnityEngine;


public class FlockManager : MonoBehaviour {
    public GameObject birdPrefab;
    public int flockSize = 50;
    public Vector3 spawnAreaSize = new Vector3(50f, 10f, 50f);

    public float leaderSpeed = 100f;
    public float followerSpeed = 105f;  
    public float turnSpeed = 20f;        

    public float neighborRadius = 40f;
    public float separationRadius = 15f;
    public float leaderWeight = 12f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 0.8f;
    public float separationWeight = 1.5f;

    public float maxBoundaryTurnOffset = 40f;

    public List<Bird> AllBirds { get; private set; } = new List<Bird>();
    public Bird Leader { get; private set; }

    void Start() {
        SpawnFlock();
    }

    void SpawnFlock() {
        for (int i = 0; i < flockSize; i++) {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnAreaSize.x, spawnAreaSize.x),
                Random.Range(-spawnAreaSize.y, spawnAreaSize.y),
                Random.Range(-spawnAreaSize.z, spawnAreaSize.z));

            GameObject birdObj = Instantiate(birdPrefab, spawnPos, Random.rotation, transform);

            Bird bird = birdObj.GetComponent<Bird>();
            if (bird == null)
                bird = birdObj.AddComponent<Bird>();

            bool isLeader = (i == 0);
            bird.Init(this, isLeader);

            if (isLeader)
                Leader = bird;

            AllBirds.Add(bird);
        }
    }
}