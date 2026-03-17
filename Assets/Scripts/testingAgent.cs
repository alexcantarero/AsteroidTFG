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

    public override void OnEpisodeBegin() //Función que se ejecuta una vez empieza un episodio.
    {
        transform.position = Vector3.zero; //En este caso, lo que queremos es que la nave vuelva a la posición inicial. 
    }

    [SerializeField] private Transform targetTransform; //Un target para probar que se mueve hacia allí y que aprende. 
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

    public override void OnActionReceived(ActionBuffers actions) // Contiene nuestras acciones como floats (continua, de -1 a 1) o  ints (discreta, de 0 en uno en uno.)
    {

        // Rama 0: Movimiento (0: Nada, 1: Acelerar)
        int moveAction = actions.DiscreteActions[0];
        if (moveAction == 1) { 
            /* Aplicar fuerza hacia adelante */
        
        
        
        }

        // Rama 1: Rotación (0: Nada, 1: Rotar a la derecha, 2: Rotar a la izquierda)
        int rotateAction = actions.DiscreteActions[1];
        if (rotateAction == 1) { 
            /* Rotar a la derecha */
        
        }
        else if (rotateAction == 2) { 
            /* Rotar a la izquierda */
        
        }

        // Rama 2: Disparo (0: Nada, 1: Disparo principal, 2: Disparo secundario)
        int shootAction = actions.DiscreteActions[2];
        if (shootAction == 1) { 
            /* Disparar Principal */
        
        
        }
        else if (shootAction == 2) { 
            /* Disparar Secundario */
        
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
