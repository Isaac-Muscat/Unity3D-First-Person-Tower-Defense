using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float velocity = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward*velocity*Time.deltaTime);
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
    }
}
