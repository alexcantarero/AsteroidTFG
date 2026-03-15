using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class testingAgent : Agent {


    //Collect observations. How the agent observes the environment.
    //The idea here is to choose which information must the agent know. I want to train it to just avoid meteors. 
    //In this case, it will need its actual position, and then info about a specific target. ( to test)

    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
    }

    [SerializeField] private Transform targetTransform;
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position); //3 floats
        sensor.AddObservation(targetTransform.position); //3 floats again
    }
    //Contains our actions as floats or ints. ML Algorithms only work with numbers. 
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 6f;
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target")){
            SetReward(1f);
            EndEpisode(); //Episode ends. Let's restart the game.

        }

    }

}
