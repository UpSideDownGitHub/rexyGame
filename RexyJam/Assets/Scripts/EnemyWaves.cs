using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct EnemyWave
{
    public string waveName;
    public int maxRound;
    public int totalEnemyCount;
    public int currentEnemyKilledCount;
    public int currentEnemySpawnedCount;
    public int maxOnScreenEnemies;
    public int currentEnemiesOnScreen;
    public GameObject[] enemies;
    public float healthMultiplier;
    public float damageMultiplier;
}

public class EnemyWaves : MonoBehaviour
{
    public EnemyWave[] enemyWaves;
    public GameObject[] spawnPositions;
    public int currentWaveInfo;
    public int currentWave;

    public float enemySpawnRate;
    private float _timeOfNextEnemySpawn;


    [Header("Wave Info")]
    public TMP_Text waveNumber;
    public bool showWaveNumber = true;
    public float showWaveNumberTime;
    private float _timeToHideWaveNumber;

    [Header("Wave UI")]
    public Image waveCompletion;
    public TMP_Text waveCompletionNumber;

    public void Awake()
    {
        Physics2D.IgnoreLayerCollision(6, 7);        
    }

    public void Start()
    {
        currentWave = 0;
        currentWaveInfo = 0;
        waveNumber.text = "Wave 1";
        waveCompletion.fillAmount = 0;
        waveCompletionNumber.text = "Wave 1";
        _timeToHideWaveNumber = Time.time + showWaveNumberTime;
    }

    public void Update()
    {
        if (showWaveNumber && Time.time > _timeToHideWaveNumber)
        {
            showWaveNumber = false;
            waveNumber.gameObject.SetActive(false);
        }
        if (Time.time > _timeOfNextEnemySpawn)
        {
            _timeOfNextEnemySpawn = Time.time + enemySpawnRate;

            // check for end of wave
            if (enemyWaves[currentWaveInfo].currentEnemyKilledCount >= enemyWaves[currentWaveInfo].totalEnemyCount)
            {
                currentWave++;
                enemyWaves[currentWaveInfo].currentEnemiesOnScreen = 0;
                enemyWaves[currentWaveInfo].currentEnemyKilledCount = 0;
                enemyWaves[currentWaveInfo].currentEnemySpawnedCount = 0;
                // if the current wave info has ended
                if (enemyWaves[currentWaveInfo].maxRound == -1)
                {
                    enemyWaves[currentWaveInfo].maxOnScreenEnemies++;
                    enemyWaves[currentWaveInfo].totalEnemyCount++;
                    enemyWaves[currentWaveInfo].healthMultiplier += (float)currentWave / 10f;
                    enemyWaves[currentWaveInfo].damageMultiplier += (float)currentWave / 10f;
                }
                else if (currentWave > enemyWaves[currentWaveInfo].maxRound)
                    currentWaveInfo++;

                waveCompletion.fillAmount = 0;
                waveCompletionNumber.text = "Wave " + (currentWave + 1);
                waveNumber.text = "Wave " + (currentWave + 1);
                waveNumber.gameObject.SetActive(true);
                showWaveNumber = true;
                _timeToHideWaveNumber = Time.time + showWaveNumberTime;
                return;
            }

            // if can spawn another enemy in this wave
            if (enemyWaves[currentWaveInfo].currentEnemiesOnScreen < enemyWaves[currentWaveInfo].maxOnScreenEnemies &&
                enemyWaves[currentWaveInfo].currentEnemySpawnedCount < enemyWaves[currentWaveInfo].totalEnemyCount)
            {
                // spawn random enemy from enemies in wave and give random spawn pos
                var tempEnemy = Instantiate(enemyWaves[currentWaveInfo].enemies[Random.Range(0, enemyWaves[currentWaveInfo].enemies.Length)],
                    spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position, quaternion.identity);
                // set multipliers
                tempEnemy.GetComponent<Enemy>().SetMultipliers(enemyWaves[currentWaveInfo].healthMultiplier,
                    enemyWaves[currentWaveInfo].damageMultiplier);
                enemyWaves[currentWaveInfo].currentEnemiesOnScreen++;
                enemyWaves[currentWaveInfo].currentEnemySpawnedCount++;
                return;
            }
        }
    }

    public void EnemyDied()
    {
        enemyWaves[currentWaveInfo].currentEnemiesOnScreen--;
        enemyWaves[currentWaveInfo].currentEnemyKilledCount++;
        waveCompletion.fillAmount = (float)enemyWaves[currentWaveInfo].currentEnemyKilledCount / (float)enemyWaves[currentWaveInfo].totalEnemyCount;
    }
}
