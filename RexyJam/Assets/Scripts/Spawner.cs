using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Transform[] Spawnpoints;

    [Header("EnemiesPrefabs")]
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    [Header("EnemiesArray")]
    public GameObject[] Enemies;

    [Header("SpawnDelay")]
    public bool spawnDelayBool = true;
    public float spawnDelayFloat = 5;
    
    //Not used in this code yet
    public float delayMinusAmount = 0.05f;
    
    public int EnemiesSpawned = 0;

    public int WaveNumber;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnDelayBool == true)
        {
            if (WaveNumber < 5)
            {
                if (EnemiesSpawned <= 4)
                {
                    Instantiate(Enemy1, Spawnpoints[Random.Range(0, 4)].transform.position, transform.rotation);
                    spawnDelayBool = false;
                    EnemiesSpawned++;
                    StartCoroutine(Delay());
                }

                //This should be called when players kill all enemies, now i'm just testing functionality
                else
                {
                    WaveIncrease();
                }
            }

            else if ((WaveNumber >= 5) && (WaveNumber < 10))
            {
                spawnDelayFloat = 4;

                if (EnemiesSpawned <= 9)
                {
                    Instantiate(Enemies[Random.Range(0, 2)], Spawnpoints[Random.Range(0, 4)].transform.position, transform.rotation);

                    spawnDelayBool = false;
                    EnemiesSpawned++;
                    StartCoroutine(Delay());
                }
                //This should be called when players kill all enemies, now i'm just testing functionality
                else
                {
                    WaveIncrease();
                }

            }

            else if ((WaveNumber >= 10) && (WaveNumber < 15))
            {
                spawnDelayFloat = 3;
                if (EnemiesSpawned <= 19)
                {
                    Instantiate(Enemies[Random.Range(0, 3)], Spawnpoints[Random.Range(0, 4)].transform.position, transform.rotation);

                    spawnDelayBool = false;
                    EnemiesSpawned++;
                    StartCoroutine(Delay());
                }
                //This should be called when players kill all enemies, now i'm just testing functionality
                else
                {
                    WaveIncrease();
                }

            }
        }

    }

    public void WaveIncrease()
    {
        WaveNumber++;
        EnemiesSpawned = 0;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(spawnDelayFloat);
        
        spawnDelayBool = true;
    }

    
}
