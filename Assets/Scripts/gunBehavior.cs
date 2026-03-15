using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class gunBehavior : MonoBehaviour
{

    public GameObject bullet;
    public GameObject secondaryBullet;
    public AudioClip shootSFX;

    Transform tf;

    public int secondaryCount = 5;
    public float secondarySpreadDeg = 45f;
    void Start()
    {
        tf = transform;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShootPrimary()
    {
        GameObject clone;
        clone = Instantiate(bullet, tf.position, tf.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = GetComponentInParent<Transform>().up * 10;
        SFXManager.instance.PlaySFX(shootSFX, 0.125f);

    }

    public void ShootSecondary()
    {

        float half = secondarySpreadDeg * 0.5f;
        float step = secondarySpreadDeg / (secondaryCount - 1);

        for (int i = 0; i < secondaryCount; i++)
        {
            float angle = -half + step * i;
            Quaternion rot = tf.rotation * Quaternion.Euler(0f, 0f, angle);
            GameObject clone = Instantiate(secondaryBullet, tf.position, rot);
            var rb = clone.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // rotated forward direction times speed
                Vector2 dir = (Vector2)(rot * Vector3.up);
                rb.velocity = dir * 10f;
            }
        }
        SFXManager.instance.PlaySFX(shootSFX, 0.125f);

    }
}
