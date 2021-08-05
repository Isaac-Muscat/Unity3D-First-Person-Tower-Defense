using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerPrefab, groundPrefab;
    public List<SpawnerController> spawners;
    public float timeBetweenSpawns = 8f;
    public float timeBetweenSpawnRateIncrease = 30f;
    float deltaTime = 0;
    float deltaTime2 = 0;

    void Awake()
    {
        // GameObject ground = Instantiate(groundPrefab) as GameObject;
        // GameObject player = Instantiate(playerPrefab) as GameObject;
        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(0, 5, 0);
    }

    private void Start() {
        spawners = new List<SpawnerController>();
        GameObject[] spawnerPrefabs = GameObject.FindGameObjectsWithTag("Spawner");
        for(int i = 0; i<spawnerPrefabs.Length; i++)
        {
            spawners.Add(spawnerPrefabs[i].GetComponent<SpawnerController>());
        }
    }

    private void Update() {
        deltaTime+=Time.deltaTime;
        if(deltaTime>=timeBetweenSpawns){
            foreach (SpawnerController spawner in spawners) {
                spawner.SpawnEnemy();
            }
            deltaTime=0;
        }

        deltaTime2+=Time.deltaTime;
        if(deltaTime2>=timeBetweenSpawnRateIncrease&&timeBetweenSpawns>1){
            timeBetweenSpawns-=0.25f;
            deltaTime2=0;
        }

    }

}
