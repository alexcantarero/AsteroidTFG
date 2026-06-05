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

    public Vector2 asteroidObjective;

    private bool bulletHasCollided;
    public Vector2 direction;
    public bool haveIBeenDestroyed;

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

        //Vector2 point = new Vector2(Random.Range(-8f, 8f), Random.Range(-5f, 5f));

        //Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        if(!haveIBeenDestroyed) direction = (asteroidObjective - (Vector2)transform.position).normalized;
        else direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

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
            GameObject bullet = collision.gameObject;
            //Comprobamos si es un bullet normal o uno de los del disparo secundario. 
            if (bullet.name.Equals("roundBullet(Clone)"))
            {
               bullet.GetComponent<secondaryBulletBehavior>().testingAgent.AddReward(1.0f);
               GameObject gameManager = bullet.GetComponent<secondaryBulletBehavior>().testingAgent.gameManager; //Añadimos los puntos al marcador.
               gameManager.GetComponent<GameManager>().addPoints(name);

                Debug.Log("Reward added for destroying asteroid! SECONDARY");
            }
            else if (bullet.name.Equals("bullet(Clone)"))
            {
                bullet.GetComponent<bulletBehavior>().testingAgent.AddReward(0.5f);
                GameObject gameManager = bullet.GetComponent<bulletBehavior>().testingAgent.gameManager; //Añadimos los puntos al marcador.
                gameManager.GetComponent<GameManager>().addPoints(name);
                Debug.Log("Reward added for destroying asteroid! PRIMARY");
            }
            bulletHasCollided = true;

            //División
            if (name == "big(Clone)" || name == "medium(Clone)")
            {
                
                GameObject ast1 = Instantiate(subAsteroids[0], gameObject.transform.position, Quaternion.identity, transform.parent);
                ast1.GetComponent<asteroidBehavior>().haveIBeenDestroyed = true;

                GameObject ast2 = Instantiate(subAsteroids[1], gameObject.transform.position, Quaternion.identity, transform.parent);
                ast2.GetComponent<asteroidBehavior>().haveIBeenDestroyed = true;

            }
            //Puntaje
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
