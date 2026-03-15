using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class gunBehavior : MonoBehaviour
{

    public GameObject bullet;
    public AudioClip shootSFX;

    Transform tf;
    void Start()
    {
        tf = transform;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot() 
    {

        GameObject clone;
        clone = Instantiate(bullet,tf.position,tf.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = GetComponentInParent<Transform>().up*10;
        SFXManager.instance.PlaySFX(shootSFX,0.125f);

    }
}
