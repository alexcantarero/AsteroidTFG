using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondaryBulletBehavior : MonoBehaviour
{
    [SerializeField] private float ttl;

    void Start()
    {

        Destroy(gameObject, ttl);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Limit"))
        {
            Destroy(gameObject);
            //If the bullet does not hit anything, the player loses points!
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Debug.Log("Bullet destroyed");
            Destroy(gameObject);

        }
    }
}
