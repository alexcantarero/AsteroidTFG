using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    public GameObject gameManager;
    Rigidbody2D rb;
    Transform tf;
    Animator anim;

    Vector2 velocity;

    // Parámetros expuestos
    public float acceleration = 6f;     
    public float deceleration = 8f;     
    public float maxSpeed = 6f;         
    public float rotationSpeed = 180f;  
    public bool invincible = false;

    gunBehavior gun;
    public float primaryGunDelay = 0.2f;
    public float secondaryGunDelay = 1.0f;
    float currentGunDelay = 0.0f;
    public Image primaryFillBar;
    private float currentCooldownDuration; //Primary or secondary gun delay


    public GameObject hurt;

    public AudioClip respawnSFX;
    public AudioClip explosionSFX;
    public AudioClip thrustSFX;

    private Vector3 initialPlayerPosition;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tf = transform;
        gun = GetComponentInChildren<gunBehavior>();
        anim = GetComponent<Animator>();

        velocity = Vector2.zero;
        StartCoroutine(HurtAnimation());

        primaryFillBar.fillAmount = 1;
        initialPlayerPosition = transform.position;
        Debug.Log("initialPlayerPosition set to" +  initialPlayerPosition);
        



    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            toggleGodMode();
        }

        if (currentGunDelay > 0f)
        {
            if (primaryFillBar != null)
            {
                float progress = 1f - currentGunDelay / currentCooldownDuration; //Percentage based on primary or secondary gun delay
                primaryFillBar.fillAmount = progress;
            }
        }
        else
        {
            if (primaryFillBar != null) primaryFillBar.fillAmount = 1f;
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

        if (rotationDelta != 0f) transform.Rotate(0f,0f, rotationDelta);
        

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
        if (Input.GetKey(KeyCode.K))   
        {
            if (currentGunDelay == 0)
            {
                Debug.Log("You pressed K and gunDelay is 0");
                gun.ShootPrimary();
                currentGunDelay = primaryGunDelay;
                currentCooldownDuration = primaryGunDelay;
            }
        }
        if (Input.GetKey(KeyCode.L))
        {
            if (currentGunDelay == 0)
            {
                Debug.Log("You pressed L and gunDelay is 0");
                gun.ShootSecondary();
                currentGunDelay = secondaryGunDelay;
                currentCooldownDuration = secondaryGunDelay;
            }
        }
    }

    private void toggleGodMode()
    {
        invincible = !invincible;
        if (invincible) primaryGunDelay = secondaryGunDelay = 0.05f;
        else { primaryGunDelay = 0.2f; secondaryGunDelay = 1.0f; }
        Debug.Log("God mode set to " + invincible); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) return;
        if (collision.gameObject.CompareTag("Asteroid") && !invincible)
        {
            //Debug.Log("Hola");
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
        Debug.Log(initialPlayerPosition);
        transform.position = initialPlayerPosition;
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

    void fillCooldownBar(float amount)
    {
        primaryFillBar.fillAmount += amount;
    
    }


}