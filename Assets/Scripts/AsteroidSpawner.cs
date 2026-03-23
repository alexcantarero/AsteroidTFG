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
            GameObject asteroid = Instantiate(
                asteroids[Random.Range(0, asteroids.Length)],
                spawners[Random.Range(0, spawners.Length)].transform.position,
                Quaternion.identity);
            asteroid.GetComponent<asteroidBehavior>().asteroidObjective =  new Vector2(Random.Range(spawners[2].transform.position.x, spawners[3].transform.position.x), Random.Range(spawners[0].transform.position.y, spawners[1].transform.position.y));

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

    internal void resetDifficulty()
    {

        spawnCooldown = 3f;
    
    
    }
}
