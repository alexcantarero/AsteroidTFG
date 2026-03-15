using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    public GameObject gameManager;
    Rigidbody2D rb;
    BoxCollider2D box;
    Transform tf;
    SpriteRenderer sr;
    Animator anim;

    Vector2 velocity;

    // parámetros expuestos
    public float acceleration = 6f;     
    public float deceleration = 8f;     
    public float maxSpeed = 6f;         
    public float rotationSpeed = 180f;  
    public bool invincible = false;

    gunBehavior gun;
    public float gunDelay = 0.2f;
    float currentGunDelay = 0.0f;

    public GameObject hurt;

    public AudioClip respawnSFX;
    public AudioClip explosionSFX;
    public AudioClip thrustSFX;

    private bool isPlaying;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        tf = transform;
        gun = GetComponentInChildren<gunBehavior>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        velocity = Vector2.zero;
        StartCoroutine(HurtAnimation());

        isPlaying = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            toggleGodMode();
        }

    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        if(currentGunDelay > 0 ) currentGunDelay -= Time.deltaTime;
        if (currentGunDelay < 0) currentGunDelay = 0;

        // --- Entrada ---
        bool pressingForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool turningLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool turningRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        // ---- SFX PROPEL ----
        if (pressingForward)
        {
            if (SFXManager.instance != null && !SFXManager.instance.IsLooping)
            {
                SFXManager.instance.PlayLoopingSFX(thrustSFX, 0.125f);
            }
        }
        else
        {
            if (SFXManager.instance != null && SFXManager.instance.IsLooping)
            {
                SFXManager.instance.StopLoopingSFX();
            }
        }

        // --- Rotación: rota sobre sí mismo y NO modifica el vector 'velocity' ---
        float rotationDelta = 0f;
        if (turningLeft) rotationDelta = rotationSpeed * Time.fixedDeltaTime;
        else if (turningRight) rotationDelta = -rotationSpeed * Time.fixedDeltaTime;

        if (rotationDelta != 0f)
        {
            rb.MoveRotation(rb.rotation + rotationDelta);
        }

        // --- Determinar dirección "forward" según la opción ---
        Vector2 forward = (Vector2)tf.up;

        // --- Aceleración: al pulsar W añadimos velocidad en la dirección que mira el transform ahora ---
        if (pressingForward)
        {
            velocity += forward * (acceleration * Time.fixedDeltaTime); // v = a*t

            if (velocity.magnitude > maxSpeed) //Si sobrepasa el límite lo limitamos
            {
                velocity = velocity.normalized * maxSpeed;
            }
            anim.SetBool("propelling", true);
        }
        else
        {
            anim.SetBool("propelling", false);
            // --- Deceleración: si no pulsas W se reduce la magnitud del vector de velocidad ---
            float speed = velocity.magnitude;
            if (speed > 0f) // Si aún está en movimiento 
            {
                float newSpeed = speed - deceleration * Time.fixedDeltaTime; // v = a*t
                if (newSpeed <= 0f) // Si la velocidad decelerada es menor a cero, la limitamos a cero
                {
                    velocity = Vector2.zero;
                }
                else
                {
                    velocity = velocity.normalized * newSpeed; //Aplicamos la nueva velocidad
                }
            }
        }



        // --- Aplicar velocidad al Rigidbody2D ---
        rb.velocity = velocity;

        //----------------- SHOOT ---------------------
        if (Input.GetKey(KeyCode.Space))   
        {
            if (currentGunDelay == 0)
            {
                gun.Shoot();
                currentGunDelay = gunDelay;
            }
        }



    }

    private void toggleGodMode()
    {
        invincible = !invincible;
        if (invincible) gunDelay = 0.05f;
        else gunDelay = 0.2f;

            Debug.Log("God mode set to " + invincible);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Asteroid") && !invincible)
        {
            if (gameManager.GetComponent<GameManager>().lives == 1) SceneManager.LoadScene("1");
            gameManager.GetComponent<GameManager>().getHurt();
            HurtParticles();
            Destroy(collision.gameObject);
            
        }

    }

    public void HurtParticles()
    {
        GameObject cloneParticles = Instantiate(hurt, transform.position, transform.rotation);
        SFXManager.instance.PlaySFX(explosionSFX, 0.066f);
        Destroy(cloneParticles, 1);
    }

    public void respawn() 
    {
        SFXManager.instance.PlaySFX(respawnSFX, 0.125f);
        velocity = Vector2.zero;             
        if (rb != null)
        {
            rb.velocity = Vector2.zero;      
            rb.angularVelocity = 0f;         
            rb.rotation = 0f;                
        }

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        StartCoroutine(HurtAnimation());
    }

    private IEnumerator HurtAnimation() 
    {

        anim.SetBool("isHurt", true);
        invincible = true;
        yield return new WaitForSeconds(1f);
        anim.SetBool("isHurt", false);
        invincible = false;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }


}