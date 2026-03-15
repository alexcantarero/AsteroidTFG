using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AsteroidSpawner : MonoBehaviour
{

    public GameObject[] spawners;
    public GameObject[] asteroids;

    float spawnCooldown = 3f;
    float currentTime = 0f;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            Instantiate(
                asteroids[Random.Range(0, asteroids.Length)],
                spawners[Random.Range(0, spawners.Length)].transform.position,
                Quaternion.identity);
            currentTime = spawnCooldown;
        }
    }

    internal void increaseDifficulty()
    {
        if (spawnCooldown > 0.5f)
        {
            spawnCooldown -= 0.5f;
            Debug.Log("Paso de nivel, cooldown a" + spawnCooldown);
        }
    }
}
