using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public List<Vector2Int> path;
    public GameObject enemyPrefab;
    public List<GameObject> enemies;
    void Awake()
    {
        enemies = new List<GameObject>();
    }

    public void SpawnEnemy(){
        GameObject enemy = Instantiate(enemyPrefab) as GameObject;
        enemy.transform.parent = transform;
        enemy.transform.position = new Vector3(transform.position.x, enemyPrefab.GetComponent<Renderer>().bounds.size.y/2+transform.localScale.y/2, transform.position.z);
        enemies.Add(enemy);
    }
}
