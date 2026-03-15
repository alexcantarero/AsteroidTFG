using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    public float shakeAmount = 0.01f;
    public GameObject HUD;
    private Vector3 initialPosition;
    private Vector3 initialHUDPosition;

    public bool shake;
    void Start()
    {
        initialPosition = transform.position;
        initialHUDPosition = HUD.transform.position;
        shake = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (shake)
        {
            HUD.transform.position = HUD.transform.position + Random.insideUnitSphere * shakeAmount;
            transform.position = initialPosition + Random.insideUnitSphere * shakeAmount;
        }
        else
        {
            HUD.transform.position = initialHUDPosition;
            transform.position = initialPosition;
        }
    }
}
