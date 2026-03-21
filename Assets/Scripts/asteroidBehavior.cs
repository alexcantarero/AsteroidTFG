using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

public class asteroidBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D body;
    float speed;


    public GameObject explosion;
    ParticleSystem parts;

    public GameObject[] subAsteroids;

    public AudioClip explosionSFX;

    private bool bulletHasCollided;

    [SerializeField] private GameObject GameManager;
    void Start()
    {
        
        switch (name)
        {
            case "small(Clone)":
                speed = 4f;
                break;
            case "medium(Clone)":
                speed = 3.5f;
                break;
            case "big(Clone)":
                speed = 3f;
                break;
        }

        transform.Rotate(0,0,Random.Range(0,364));

        body = GetComponent<Rigidbody2D>();

        Vector2 point = new Vector2(Random.Range(-8f, 8f), Random.Range(-5f, 5f));

        Vector2 direction = (point - (Vector2)transform.position).normalized;

        body.velocity = direction * speed;

        parts = explosion.GetComponent<ParticleSystem>();

        bulletHasCollided = false;



    }

    // Update is called once per frame
    void Update()
    { 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !bulletHasCollided)
        {
            bulletHasCollided = true;
            //División
            if (name == "big(Clone)" || name == "medium(Clone)")
            {
                
                Instantiate(subAsteroids[0], gameObject.transform.position, Quaternion.identity);
                Instantiate(subAsteroids[1], gameObject.transform.position, Quaternion.identity);
            }
            //Puntaje
            GameManager.GetComponent<GameManager>().addPoints(name);
            Debug.Log("Added points");
            explosionEffect();
            

        }
        if (collision.gameObject.CompareTag("DeadZone")) Destroy(gameObject);
    }

    private void explosionEffect()
    {

        GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
        if (name == "small(Clone)") effect.transform.localScale = Vector3.one * 0.25f;
        else if (name == "medium(Clone)") effect.transform.localScale = Vector3.one * 0.5f;
        Destroy(gameObject);
        float totalDuration = parts.duration + parts.startLifetime;
        Destroy(effect, 1);

        SFXManager.instance.PlaySFX(explosionSFX, 0.066f);


    }



}
