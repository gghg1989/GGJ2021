using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    //create class as a singleton
    private static EnemySpawnerScript instance;
    public EnemySpawnerScript()
    {
        if(instance == null)
        {
            instance = this;
        }

        else
        {
            Debug.LogError("Singleton EnemySpawnerScript Already Exists");
        }
    }
    public static EnemySpawnerScript spawnerScriptSingleton
    {
        get
        {
            if(instance == null)
            {
                new EnemySpawnerScript();
            }
            return instance;
        }
    }


    //testing or temporary variables
    public GameObject tempSpawnLocation;
    int tempSpawnCounter;
    public GameObject temporaryThePlayer;


    //game object and variable declaration
    public int enemiesInObjectPool; //defines the number of enemies to prespawn. Needs to be greater than the number of simultaneous enemies on the screen.
    public GameObject enemyPrefabGameObject;
    GameObject[] enemyList;


    //spawn timing variables
    public float spawnInterval;
    TimerClass spawnTimer = new TimerClass();
    
    // Start is called before the first frame update
    void Start()
    {
        enemyList = new GameObject[enemiesInObjectPool];
        GenerateEnemyList();
        spawnTimer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnTimer.GetTime() > spawnInterval && tempSpawnCounter < enemiesInObjectPool) //spawncounter for testing only
        {
            SpawnEnemy(DetermineSpawnLocation());

            spawnTimer.ResetTimer();

            //for testing only
            tempSpawnCounter++;


        }
    }

    //generates pool of enemies that we will reuse as they are killed and spawned. Need to define enough enemies in the enemiesInObjectPool variable
    //Need as many enemies as we would ever expect to be on the screen simultaneously
    void GenerateEnemyList()
    {
        GameObject enemyInstance;


        for(int i = 0; i < enemiesInObjectPool; i++)
        {
            enemyInstance = Instantiate(enemyPrefabGameObject);
            enemyInstance.SetActive(false);
            enemyList[i] = enemyInstance;
        }

    }

    Vector2 DetermineSpawnLocation()
    {
        //where does the enemy spawn

        Vector2 spawnLocation;

        //randomly generate a location based on the tiles we have?
        spawnLocation = tempSpawnLocation.transform.position;

        return spawnLocation;
    }
    
    
    void SpawnEnemy(Vector2 spawnLocation)
    {
        //spawns enemy at spawn location

        for(int i = 0; i < enemiesInObjectPool; i++)
        {
            
            if(!enemyList[i].activeSelf)
            {
                enemyList[i].transform.position = spawnLocation;
                enemyList[i].SetActive(true);
                break;
            }

            else if(i == enemiesInObjectPool - 1)
            {
                Debug.Log("not enough enemies in object pool, ran out of already spawned enemies");
            }
        }

    }



}
