using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Player player;
    public float speed, health, damage;
    public Color color;
    public List<Vector2Int> path;

    private Vector3 target;
    private Vector2 tileDims;
    private float tileSize;

    private void Awake() {
        transform.GetComponent<MeshRenderer>().material.color = color;
    }

    private void Start() {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();
        // Fetch variables from the GroundController Script
        tileSize = transform.parent.parent.parent.GetComponent<GroundController>().tileSize;
        tileDims = transform.parent.parent.parent.GetComponent<GroundController>().tileDims;

        // Fetch path from the spawner and clean up so path starts at 0 instead of -1
        path = transform.parent.GetComponent<SpawnerController>().path;
        // path.RemoveAt(path.Count-1);
        // path.Reverse();
        StartCoroutine(FollowPath());
    }
 
    private void Update() {

    }
    
    // Allows enemy to follow the path and calls rotation coroutine
    IEnumerator FollowPath(){
        int nextPathIndex = path.Count-1;
        target = new Vector3(path[nextPathIndex].x*tileSize-tileSize*tileDims.x/2+tileSize/2, transform.position.y, 
                             path[nextPathIndex].y*tileSize-tileSize*tileDims.y/2+tileSize/2);
        Vector3 direction = target - transform.position;
        float angle = 90-Mathf.Atan2(direction.z, direction.x)*Mathf.Rad2Deg;
        transform.Rotate(new Vector3(0, angle, 0));

        while(true){
            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, target, speed*Time.deltaTime);

            // If on target
            if(transform.position == target){
                // Update next path index
                nextPathIndex--;

                // Break if reached end
                if(nextPathIndex<0) break;

                // Set target
                target = new Vector3(path[nextPathIndex].x*tileSize-tileSize*tileDims.x/2+tileSize/2, transform.position.y, 
                                     path[nextPathIndex].y*tileSize-tileSize*tileDims.y/2+tileSize/2);

                // Rotate to next tile
                direction = target - transform.position;
                angle = 90-Mathf.Atan2(direction.z, direction.x)*Mathf.Rad2Deg;
                transform.Rotate(new Vector3(0, angle, 0));
            }
            yield return null;
        }
        OnReachingPlayerSpawn();
    }

    void OnReachingPlayerSpawn(){
        player.health -= damage;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Worked");
        if(other.transform.tag=="Projectile"){
            player.money+=50;
            Destroy(gameObject);
        }
    }


}
