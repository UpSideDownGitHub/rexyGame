using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using Random = UnityEngine.Random;


[Serializable]
public struct EnemyInfo
{
    public string enemyName;
    public GameObject enemyObject;
    [Range(0f, 1f)] public float SpawnChance;
}
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
    public EnemyInfo[] enemies;
}


public class EnemyWaves : MonoBehaviour
{
    public EnemyWave[] enemyWaves;
    public GameObject[] spawnPositions;
    public GameObject[] warningPositions;
    public int currentWaveInfo;
    public int currentWave;

    public float minEnemySpawnRate;
    public float maxEnemySpawnRate;
    private float _timeOfNextEnemySpawn;

    [Header("Wave UI")]
    public HealthGaugeFunctions healthGaugeFunctions;

    [Header("Sounde")]
    public AudioSource source;

    public void Start()
    {
        currentWave = 0;
        currentWaveInfo = 0;
        healthGaugeFunctions.SetWaveSliderIU(0);
        healthGaugeFunctions.SetWavesUI(currentWave + 1);
    }

    public void Update()
    {
        if (Time.time > _timeOfNextEnemySpawn)
        {
            _timeOfNextEnemySpawn = Time.time + Random.Range(minEnemySpawnRate, maxEnemySpawnRate);

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
                    enemyWaves[currentWaveInfo].maxOnScreenEnemies += currentWave;
                    enemyWaves[currentWaveInfo].totalEnemyCount += currentWave;
                }
                else if (currentWave > enemyWaves[currentWaveInfo].maxRound)
                    currentWaveInfo++;

                healthGaugeFunctions.SetWaveSliderIU(0);
                healthGaugeFunctions.SetWavesUI(currentWave + 1);
                healthGaugeFunctions.SetWaveText(currentWave + 1);
                healthGaugeFunctions.NewWaveBulbs();
                return;
            }

            // if can spawn another enemy in this wave
            if (enemyWaves[currentWaveInfo].currentEnemiesOnScreen < enemyWaves[currentWaveInfo].maxOnScreenEnemies &&
                enemyWaves[currentWaveInfo].currentEnemySpawnedCount < enemyWaves[currentWaveInfo].totalEnemyCount)
            {
                //// spawn random enemy from enemies in wave and give random spawn pos
                float ran = Random.value;
                GameObject tempEnemy = null;
                for (int i = 0; i < enemyWaves[currentWaveInfo].enemies.Length; i++)
                {
                    if (i == 0)
                    {
                        if (ran <= enemyWaves[currentWaveInfo].enemies[i].SpawnChance)
                        {
                            var rand = Random.Range(0, spawnPositions.Length);
                            tempEnemy = Instantiate(enemyWaves[currentWaveInfo].enemies[i].enemyObject,
                                spawnPositions[rand].transform.position, quaternion.identity);
                            warningPositions[rand].SetActive(true);
                        }
                    }
                    else if (ran > enemyWaves[currentWaveInfo].enemies[i - 1].SpawnChance &&
                        ran <= enemyWaves[currentWaveInfo].enemies[i].SpawnChance)
                    {
                        var rand = Random.Range(0, spawnPositions.Length);
                        tempEnemy = Instantiate(enemyWaves[currentWaveInfo].enemies[i].enemyObject,
                            spawnPositions[rand].transform.position, quaternion.identity);
                        warningPositions[rand].SetActive(true);
                    }

                }
                enemyWaves[currentWaveInfo].currentEnemiesOnScreen++;
                enemyWaves[currentWaveInfo].currentEnemySpawnedCount++;
                return;
            }
        }
    }

    public void EnemyDied()
    {
        source.Play();
        enemyWaves[currentWaveInfo].currentEnemiesOnScreen--;
        enemyWaves[currentWaveInfo].currentEnemyKilledCount++;
        healthGaugeFunctions.SetWaveSliderIU((float)enemyWaves[currentWaveInfo].currentEnemyKilledCount / (float)enemyWaves[currentWaveInfo].totalEnemyCount);
    }
}
