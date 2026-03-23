using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPEffects;
using UnityEngine;
using TMPEffects.TMPAnimations;
using TMPEffects.Components;
using Unity.MLAgents.Policies;
using System;

public class GameManager : MonoBehaviour
{
    //public GameObject[] spritesHearts;
    public GameObject player;
    public TMP_Text scoreText;
    public TMP_Text hearts;
    public TMP_Text DifficultyText;
    public TMP_Text passiveIncrementText;
    public int lives;
    public int score;

    public AudioClip respawnSFX;

    public float timeWithoutGettingHit;
    private float incrementCooldown;
    public int scorePassiveIncrement;

    private enum Difficulty { Easy, Medium, Hard, Extreme, Impossible, God }
    private Difficulty difficultyLevel;
    public GameObject asteroidSpawner;
    private AsteroidSpawner spawner;

    public CameraShake cameraShake;

    private void Awake()
    {
    }

    void Start()
    {

        score = 0;
        lives = 3;
        scorePassiveIncrement = 1;
        incrementCooldown = 0f;
        if (scoreText != null) scoreText.text = "<sketchy>" + score.ToString();
        passiveIncrementText.text = "<sketchy>+" + scorePassiveIncrement.ToString();
        hearts.text = "<sketchy>PPP";
        difficultyLevel = Difficulty.Easy;

        spawner = asteroidSpawner != null ? asteroidSpawner.GetComponent<AsteroidSpawner>() : null;
    }

    public void addPoints(string name)
    {
        Debug.Log("Added points");
        switch (name)
        {
            case "big(Clone)":
                score += 25;
                break;
            case "medium(Clone)":
                score += 50;
                break;
            case "small(Clone)":
                score += 100;
                break;
        }

        if (scoreText != null) scoreText.text = "<sketchy>"+score.ToString();
    }

    private void checkPassiveIncrement()
    {
        //1 -- de 0 a 30s
        //2 -- de 30s a 1min 30s
        //10 -- de 2min 30s en adelante
        if (timeWithoutGettingHit >= 150f)
        {
            scorePassiveIncrement = 10;
            passiveIncrementText.color = new Color32(194, 136, 255, 255);
        }
        else if (timeWithoutGettingHit >= 90f)
        {
            scorePassiveIncrement = 5;
            passiveIncrementText.color = new Color32(244, 255, 136, 255);
        }
        else if (timeWithoutGettingHit >= 30f)
        {
            scorePassiveIncrement = 2;
            passiveIncrementText.color = new Color32(136, 255, 140, 255);
        }
        else 
        {
            passiveIncrementText.color = new Color32(255, 255, 255, 255);
        }
            passiveIncrementText.text = "<sketchy>+" + scorePassiveIncrement.ToString();


    }

    public void getHurt()
    {
        if (lives > 0)
        {
            lives -= 1;
            if (lives == 0)
            {
                Start();
                spawner.resetDifficulty();
                if (player.GetComponent<testingAgent>().enabled)
                {
                    player.GetComponent<testingAgent>().AddReward(-1f); //Reward por perder. 
                }

            }
            else if (lives > 0) hearts.text = hearts.text.Substring(0, hearts.text.Length - 1);
            StartCoroutine(DieCoroutine());
        }
        scorePassiveIncrement = 1;
        timeWithoutGettingHit = 0;
    }

    private IEnumerator DieCoroutine()
    {
        player.SetActive(false);
        cameraShake.shake = true;
        yield return new WaitForSeconds(0.3f);
        cameraShake.shake = false;
        yield return new WaitForSeconds(1.7f);
        player.SetActive(true);
        if (player.GetComponent<PlayerMovement>().enabled) player.GetComponent<PlayerMovement>().respawn();
        else if (player.GetComponent<testingAgent>().enabled) player.GetComponent<testingAgent>().Respawn();
        SFXManager.instance.PlaySFX(respawnSFX, 0.125f);

    }



    void Update()
    {
        // Cambio de dificultad
        if (score >= 5000)
        { 
            SetDifficulty(Difficulty.God);
            DifficultyText.text = "<sketchy>God";
            DifficultyText.color = new Color32(255, 57, 81, 255);
        }
        else if (score >= 4000) {
            SetDifficulty(Difficulty.Impossible);
            DifficultyText.text = "<sketchy>Impossible";
            DifficultyText.color = new Color32(194, 136, 255, 255);
        }
        else if (score >= 3000)
        {
            SetDifficulty(Difficulty.Extreme);
            DifficultyText.text = "<sketchy>Extreme";
            DifficultyText.color = new Color32(255, 136, 238, 255);
        }
        else if (score >= 2000)
        {
            SetDifficulty(Difficulty.Hard);
            DifficultyText.text = "<sketchy>Hard";
            DifficultyText.color = new Color32(255, 150, 136, 255);
        }
        else if (score >= 1000)
        {
            SetDifficulty(Difficulty.Medium);
            DifficultyText.text = "<sketchy>Medium";
            DifficultyText.color = new Color32(244, 255, 136, 255);
        }
        else
        {
            SetDifficulty(Difficulty.Easy);
            DifficultyText.text = "<sketchy>Easy";
            DifficultyText.color = new Color32(136, 255, 140, 255);
        }

        //Passive points increment
        incrementCooldown += Time.deltaTime;
        timeWithoutGettingHit += Time.deltaTime;

        checkPassiveIncrement();

        if (incrementCooldown >= 1f) //Passive points increment each second
        {
            score += scorePassiveIncrement;
            scoreText.text = "<sketchy>" + score.ToString();
            incrementCooldown = 0f;
        }

    }

    private void SetDifficulty(Difficulty newDifficulty)
    {
        if (newDifficulty == difficultyLevel) return;

        difficultyLevel = newDifficulty;

        if (spawner != null)
        {
            spawner.increaseDifficulty();
        }
        Debug.Log($"Difficulty changed to {difficultyLevel}");
    }
}