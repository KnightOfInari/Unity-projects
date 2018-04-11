using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public GameObject explosionEffect;

    public float delay = 3f;
    public float radius = 5f;
    public float force = 700;

    float countdown;
    bool hasExploded = false;
	// Use this for initialization
	void Start () {
        countdown = delay;
	}
	
	// Update is called once per frame
	void Update () {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;

        }
	}

    void Explode()
    {
        Debug.Log("BOOOM");

        GameObject boom = Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyOjbect in collidersToDestroy)
        {
           Destructible dest = nearbyOjbect.GetComponent<Destructible>();
            if (dest != null)
            {
                dest.Destroy();
            }
        }
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        Destroy(gameObject);
        Destroy(boom, 3);
    }
}
