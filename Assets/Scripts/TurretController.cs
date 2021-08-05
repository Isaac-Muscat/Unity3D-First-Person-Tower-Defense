using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Vector2 tileDims;
    public LayerMask enemyLayer;
    public float attackSphereRadius = 30f;
    public float attackDelaySeconds = 2f;
    public float attackDamage = 50f;
    public GameObject projectilePrefab;
    public Collider[] colliders;
    public Transform target;

    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        deltaTime = 0;
        float tileSize = GameObject.Find("Ground").GetComponent<GroundController>().tileSize;
        transform.localScale = Vector3.one*tileSize/2;
        transform.Translate(new Vector3(0, tileSize/4,0));
        tileDims = GameObject.Find("Ground").GetComponent<GroundController>().tileDims;
    }

    // Update is called once per frame
    void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, attackSphereRadius, enemyLayer);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i].transform.tag=="Enemy"){
                    target = colliders[i].transform;
                    break;
                }
            }
        }

        deltaTime+=Time.deltaTime;
        if(deltaTime>attackDelaySeconds&&colliders.Length>0){
            deltaTime=0;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.transform.LookAt(target);
        }

    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, attackSphereRadius);
    }
}
