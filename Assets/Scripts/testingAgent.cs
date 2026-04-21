using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testingAgent : Agent {


    //Collect observations. How the agent observes the environment.
    //The idea here is to choose which information must the agent know. I want to train it to just avoid meteors. 
    //In this case, it will need its actual position, and then info about a specific target. 

    [Header("Referencias")]
    [SerializeField] private Transform targetTransform; //Un target para probar que se mueve hacia allí y que aprende. 
    [SerializeField] public GameObject gameManager;
    [SerializeField] private AudioClip thrustSFX;
    [SerializeField] private AudioClip respawnSFX;
    [SerializeField] private AudioClip explosionSFX;

    [Header("Parámetros")]
    [SerializeField] private float acceleration = 6f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float primaryGunDelay = 0.2f;
    [SerializeField] private float secondaryGunDelay = 1.0f;
    [SerializeField] private GameObject hurt;
    [SerializeField] private bool invincible;


    Rigidbody2D rb;
    Animator anim;
    private gunBehavior gun;
    private float currentGunDelay = 0f;
    private Vector2 velocity;
    private Vector3 playerInitialPosition;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<gunBehavior>();
        anim = GetComponent<Animator>();

        velocity = Vector2.zero;
        playerInitialPosition = transform.position;
        invincible = false;
    }
    public override void OnEpisodeBegin() //Función que se ejecuta una vez empieza un episodio.
    {
        Debug.Log("episodio comenso");

        transform.position = playerInitialPosition; //En este caso, lo que queremos es que la nave vuelva a la posición inicial. 
        velocity = Vector2.zero;
        targetTransform.position = new Vector3(Random.Range(playerInitialPosition.x-8,playerInitialPosition.x+8), Random.Range(playerInitialPosition.y-4,playerInitialPosition.y+4), 0);
    }

    public override void CollectObservations(VectorSensor sensor) //Esta función añade al vector sensor aquellas observaciones relevantes para el modelo. 
    {

        sensor.AddObservation((Vector2)transform.up); // Dirección recta de la nave. (2)
        sensor.AddObservation(velocity.magnitude / maxSpeed); //Velocidad normalizada en float (1)

        Transform closestAsteroid = GetClosestAsteroid();
        if (closestAsteroid != null)
        {
            Vector2 dirToTarget = (closestAsteroid.position - transform.position).normalized;
            //Debug.Log("Found asteroid! Its local direction is: " +  dirToTarget);
            sensor.AddObservation((Vector2)transform.InverseTransformDirection(dirToTarget)); //Dirección local hacia el asteroide. (2)

            float dist = Vector2.Distance(closestAsteroid.position, transform.position);
            //Debug.Log("Distance to it: " + dist);
            sensor.AddObservation(dist); //Distancia al asteroide más cercano. (1)

            Rigidbody2D targetRb = closestAsteroid.GetComponent<Rigidbody2D>();

            if (targetRb != null)
            {
                Vector2 relativeVelocity = targetRb.velocity - rb.velocity; //Velocidad relativa respecto a la nave.
                //Debug.Log("Velocidad relativa: " + relativeVelocity);
                sensor.AddObservation(transform.InverseTransformDirection(relativeVelocity)); // (2 datos)
            }
            else
            {
                Debug.Log("No existe el rb en el asteroide");
                sensor.AddObservation(Vector2.zero);
            }
        }
        else
        {
            sensor.AddObservation(Vector2.zero); // No hay asteroides
            sensor.AddObservation(0f); //No hay ninguno así que distancia cero. 

        }


    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        if (currentGunDelay > 0f) currentGunDelay -= Time.deltaTime;
        if (currentGunDelay < 0f) currentGunDelay = 0f;

        int moveAction = actions.DiscreteActions[0]; //Propulsión
        if (moveAction == 1)
        {

            if (SFXManager.instance != null && !SFXManager.instance.IsLooping)
            {
                SFXManager.instance.PlayLoopingSFX(thrustSFX, 0.125f);
            }

            if (rb != null) //Si el rigidbody se ha asignado correctamente
            {
                velocity += (Vector2)transform.up * (acceleration * Time.fixedDeltaTime); //Aplicamos la fuerza proporcionalmente con el 
                if (velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed; //Limitamos la velocidad si se pasa de la máxima
                anim.SetBool("propelling", true);
            }
        }
        else //No se está acelerando
        {

            if (SFXManager.instance != null && SFXManager.instance.IsLooping)
            {
                SFXManager.instance.StopLoopingSFX();
            }

            anim.SetBool("propelling", false);
            float speed = velocity.magnitude;
            if (speed > 0f) //Si aún la nave está en movimiento
            {
                float newSpeed = speed - (deceleration * Time.fixedDeltaTime); // v = a*t. A mayor tiempo, más frenará.
                if (newSpeed <= 0f) velocity = Vector2.zero; //Si la velocidad es negativa, la limitamos a cero.
                else velocity = velocity.normalized * newSpeed; // Si no, aplicamos la nueva velocidad. 
            }
        }

        rb.velocity = velocity;

        int rotateAction = actions.DiscreteActions[1]; //Rotación
        if (rotateAction == 1)
        {
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime); //Negativo en 2D en Z es hacia la derecha.
            //AddReward(-0.0005f); //Penalización mínima por rotar. Así rotará lo mínimo para llegar al target.
        }
        else if (rotateAction == 2)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            //AddReward(-0.0005f); //Penalización mínima por rotar. Así rotará lo mínimo para llegar al target.
        }

        int shootAction = actions.DiscreteActions[2];
        if (shootAction == 1 && currentGunDelay <= 0f) //Si hemos hecho disparo principal y no hay enfriamiento
        {
            if (gun != null) gun.ShootPrimary(); //Si la arma se ha asignado bien, disparamos.
            currentGunDelay = primaryGunDelay; //Asignamos el cooldown referente al disparo realizado
            //AddReward(-0.005f); //Penalización mínima por disparar. Así disparará lo mínimo posible.

        }
        else if (shootAction == 2 && currentGunDelay <= 0f) //Idem pero para el disparo secundario
        {
            if (gun != null) gun.ShootSecondary();
            currentGunDelay = secondaryGunDelay;
            //AddReward(-0.005f); 
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut) // Con esta función seremos capaces de controlar manualmente a la nave y así generar la demo. Toca trasladar todo el playerMovement aquí :(
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Movimiento
        discreteActions[0] = 0; //Inicializamos a que no acelera

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1; //Aceleración
        }

        // Rotación
        discreteActions[1] = 0; //Inicializamos a que no se mueve ni a izquierda ni a derecha

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
        {
            discreteActions[1] = 2; //Giro a la izquierda
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
        {
            discreteActions[1] = 1; //Giro a la derecha
        }

        // Disparo
        discreteActions[2] = 0; //Inicializamos a que no dispara

        if (Input.GetKey(KeyCode.K))
        {
            discreteActions[2] = 1; // Disparo primario
        }

        if (Input.GetKey(KeyCode.L))
        {
            discreteActions[2] = 2; // Disparo secundario
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Limit"))
        {
            AddReward(-0.001f);
            Debug.Log("Limite");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target")){
            AddReward(1f);
            Debug.Log("Target");
            EndEpisode(); //Episode ends.
        }

        if (collision.CompareTag("Asteroid") && !invincible)
        {
            gameManager.GetComponent<GameManager>().getHurt();
            Debug.Log("Asteroide");
            AddReward(-0.5f);
            HurtParticles();
            Destroy(collision.gameObject);
        }

    }

    // Función para encontrar el asteroide más cercano
    private Transform GetClosestAsteroid()
    {

        Transform areaParent = transform.parent; //WorkEnv. Buscamos sólo los asteroides más cercanos dentro del workingenvironment.

        float minDistance = Mathf.Infinity; //Inicializamos una distancia infinita (por ahora!)
        Transform closest = null;

        foreach (Transform t in areaParent) //Para todo transform dentro del WorkEnv...
        {
            if (t.CompareTag("Asteroid")) //Si detectamos un asteroide dentro del environment...
            {
                float dist = Vector2.Distance(transform.position, t.position); //Calculamos la distancia hacia él
                if (dist < minDistance) //Si está más cerca del que ya había
                {
                    minDistance = dist;
                    closest = t;
                }
            }
        }
        return closest; //Devolvemos el transform más pequeño.
    }
    public void Respawn()
    {
        SFXManager.instance.PlaySFX(respawnSFX, 0.125f);
        velocity = Vector2.zero;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.rotation = 0f;
        }
        transform.position = playerInitialPosition;
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

    public void HurtParticles()
    {
        GameObject cloneParticles = Instantiate(hurt, transform.position, transform.rotation);
        SFXManager.instance.PlaySFX(explosionSFX, 0.066f);
        Destroy(cloneParticles, 1);
    }

}
