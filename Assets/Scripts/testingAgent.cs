using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class testingAgent : Agent {


    //Collect observations. How the agent observes the environment.
    //The idea here is to choose which information must the agent know. I want to train it to just avoid meteors. 
    //In this case, it will need its actual position, and then info about a specific target. 


    [Header("Referencias")]
    [SerializeField] private Transform targetTransform; //Un target para probar que se mueve hacia allí y que aprende. 

    [Header("Parámetros")]
    [SerializeField] private float acceleration = 6f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float primaryGunDelay = 0.2f;
    [SerializeField] private float secondaryGunDelay = 1.0f;



    Rigidbody2D rb;
    private gunBehavior gun;
    private float currentGunDelay = 0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<gunBehavior>();
    }

    public override void OnEpisodeBegin() //Función que se ejecuta una vez empieza un episodio.
    {
        transform.position = Vector3.zero; //En este caso, lo que queremos es que la nave vuelva a la posición inicial. 
    }

    public override void CollectObservations(VectorSensor sensor) //Esta función añade al vector sensor aquellas observaciones relevantes para el modelo. 
    {
        //En este caso, nos interesan dos: la posición de la nave y la posición del target. 
        sensor.AddObservation(transform.position.x); //3 floats (x,y,z)
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(targetTransform.position.x); //3 floats de nuevo (x,y,z)
        sensor.AddObservation(targetTransform.position.y);
        sensor.AddObservation(targetTransform.position.z);
        //Por esto necesitamos un vector de observaciones (space size) de 6, para almacenar estas seis variables.
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        if (currentGunDelay > 0f) currentGunDelay -= Time.deltaTime;
        if (currentGunDelay < 0f) currentGunDelay = 0f;

        int moveAction = actions.DiscreteActions[0]; //Propulsión
        if (moveAction == 1)
        {
            if (rb != null) //Si el rigidbody se ha asignado correctamente
            {
                Vector2 force = (Vector2)transform.up * acceleration; //Fuerza: dirección * aceleración

                rb.velocity += force * Time.deltaTime; //Aplicamos la fuerza proporcionalmente con el tiempo
                if (rb.velocity.magnitude > maxSpeed) //Limitamos la velocidad
                    rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        
        int rotateAction = actions.DiscreteActions[1]; //Rotación
        if (rotateAction == 1)
        {
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime); //Negativo en 2D en Z es hacia la derecha.
        }
        else if (rotateAction == 2)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        int shootAction = actions.DiscreteActions[2];
        if (shootAction == 1 && currentGunDelay <= 0f) //Si hemos hecho disparo principal y no hay enfriamiento
        {
            if (gun != null) gun.ShootPrimary(); //Si la arma se ha asignado bien, disparamos.
            currentGunDelay = primaryGunDelay; //Asignamos el cooldown referente al disparo realizado
        }
        else if (shootAction == 2 && currentGunDelay <= 0f) //Idem pero para el disparo secundario
        {
            if (gun != null) gun.ShootSecondary();
            currentGunDelay = secondaryGunDelay;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target")){
            SetReward(1f);
            EndEpisode(); //Episode ends. Let's restart the game.

        }

    }

}
