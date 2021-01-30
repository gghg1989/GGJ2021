using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    public bool[,] levelLayout; 
    public DungeonTileClass[,] theDungeon;

    public bool isGeneratingTutorial;
    

    public int levelSteps;
    public float levelComplexity;
    public int levelSizeX;
    public int levelSizeY;
    public RandNumberManager randomGenerator;
    public float minimumExitDistance;

    public float spacingInterval;

    public int numberOfEnemies;
    public float enemyChance;
    public float enemyChanceAdder;
    public float enemyAdderMultiplier;

    public int numberOfTreasures;
    public float treasureChance;
    public float treasureChanceAdder;
    public float treasureAdderMultiplier;

    public float monsterStartingDistance;


    public GameObject dungeonTile00; //not used
    public GameObject dungeonTile01;
    public GameObject dungeonTile02;
    public GameObject dungeonTile03;
    public GameObject dungeonTile04;
    public GameObject dungeonTile05;
    public GameObject dungeonTile06;
    public GameObject dungeonTile07;
    public GameObject dungeonTile08;
    public GameObject dungeonTile09;
    public GameObject dungeonTile10;
    public GameObject dungeonTile11;
    public GameObject dungeonTile12;
    public GameObject dungeonTile13;
    public GameObject dungeonTile14;
    public GameObject dungeonTile15;
    GameObject spawnedTile;

    [SerializeField] int startPositionX;
    [SerializeField] int startPositionY;

    int endPositionX;
    int endPositionY;

    public GameObject playerGameObject;
    GameObject playerInstance;

    public GameObject enemyGameObject;
    GameObject enemyInstance;

    public GameObject treasureGameObject;
    GameObject treasureInstance;

    public GameObject theExit;
    GameObject exitInstance;

    public Camera mainCamera;

    int currentSteps;
    public int consecutiveSteps;

    public float multiStepChance;
    GameObject background;




    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        randomGenerator = new RandNumberManager();
        levelLayout = new bool[levelSizeX + 1, levelSizeY + 1];
        theDungeon = new DungeonTileClass[levelSizeX + 1, levelSizeY + 1];

        background = GameObject.Find("LevelBackground");

        PopulateDungeonTiles();
        GenerateLevel();
        PlaceDungeonTiles();
        PlacePlayer();
        PlaceExit();
        
        PlaceEnemies();
        PlaceTreasure();
        

    }

    void Update()
    {

    }

    void PopulateDungeonTiles()
    {
        for (int x = 0; x < levelSizeX + 1; x++)
        {
            for (int y = 0; y < levelSizeY + 1; y++)
            {
                theDungeon[x, y] = new DungeonTileClass();
            }
        }
    }

    void GenerateLevel()
    {
        
        int travelDirection;
        int currentPositionX;
        int currentPositionY;
        int previousPositionX;
        int previousPositionY;

        int treasureCount = 0;
        int enemyCount = 0;
        

        //startPositionX = randomGenerator.GenerateRandomInt(1, levelSizeX-1);
        //startPositionY = randomGenerator.GenerateRandomInt(1, levelSizeY-1);

        startPositionX = randomGenerator.GenerateRandomInt(levelSizeX/4, levelSizeX*3/4);
        startPositionY = randomGenerator.GenerateRandomInt(levelSizeY/4, levelSizeY*3/4);

        background.transform.position = new Vector3(levelSizeX/2*spacingInterval, levelSizeY/2*spacingInterval, 0);
        background.transform.localScale = new Vector3(levelSizeX * spacingInterval / 108f, levelSizeX * spacingInterval / 108f, 1);


        currentPositionX = startPositionX;
        currentPositionY = startPositionY;

        previousPositionX = currentPositionX;
        previousPositionY = currentPositionY;

        travelDirection = randomGenerator.GenerateRandomInt(0, 3);

        levelLayout[startPositionX, startPositionY] = true;

        for (int x = 0; x < levelSteps; x++)
        {
            //Debug.Log("x, y" + currentPositionX + currentPositionY);

            
            //0 -> North
            //1 -> South
            //2 -> East
            //3 -> West

            if(currentSteps > consecutiveSteps)
            {
                travelDirection = randomGenerator.GenerateRandomInt(0, 3);
                currentSteps = 0;
            }

            if(randomGenerator.GenerateRandomPercentage() > multiStepChance)
            {
                travelDirection = randomGenerator.GenerateRandomInt(0, 3);
            }

            currentSteps++;

            switch (travelDirection)
            {
                case 0:
                    currentPositionY++;
                    break;
                case 1:
                    currentPositionY--;
                    break;
                case 2:
                    currentPositionX++;
                    break;
                case 3:
                    currentPositionX--;
                    break;

            }

            if(currentPositionX < 1 || currentPositionX >= levelSizeX)
            {
                currentPositionX = previousPositionX;
                if (x > 0)
                {
                    x--;
                    continue;
                }
                    
            }

            if (currentPositionY < 1 || currentPositionY >= levelSizeY)
            {
                currentPositionY = previousPositionY;
                if (x > 0)
                {
                    x--;
                    continue;
                }
            }

            levelLayout[currentPositionX, currentPositionY] = true;

            

            treasureChanceAdder = x / levelSteps * treasureAdderMultiplier;

            if (randomGenerator.GenerateRandomPercentage() < (treasureChance + treasureChanceAdder) && treasureCount <= numberOfTreasures)
            {
                theDungeon[currentPositionX, currentPositionY].treasure = true;
                theDungeon[currentPositionX, currentPositionY].treasureType = randomGenerator.GenerateRandomInt(0, 5);
                treasureCount++;
            }

            enemyChanceAdder = x / levelSteps * enemyAdderMultiplier;

            if (randomGenerator.GenerateRandomPercentage() < (enemyChance + enemyChanceAdder) && enemyCount <= numberOfEnemies)
            {
                theDungeon[currentPositionX, currentPositionY].enemy = true;
                theDungeon[currentPositionX, currentPositionY].enemyType = randomGenerator.GenerateRandomInt(0, 5);
                enemyCount++;

            }

            previousPositionX = currentPositionX;
            previousPositionY = currentPositionY;

        }

        endPositionX = currentPositionX;
        endPositionY = currentPositionY;
    }

    void PlaceDungeonTiles()
    {
        int dungeonTileIndex;

        for(int x = 0; x < levelSizeX; x++)
        {
            for(int y = 0; y < levelSizeY; y++)
            {
                if (levelLayout[x, y])
                {
                    dungeonTileIndex = CalculateBitMask(x, y);

                    switch (dungeonTileIndex)
                    {
                        case 0:
                            spawnedTile = Instantiate(dungeonTile00);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 1:
                            spawnedTile = Instantiate(dungeonTile01);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 2:
                            spawnedTile = Instantiate(dungeonTile02);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 3:
                            spawnedTile = Instantiate(dungeonTile03);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;

                        case 4:
                            spawnedTile = Instantiate(dungeonTile04);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 5:
                            spawnedTile = Instantiate(dungeonTile05);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 6:
                            spawnedTile = Instantiate(dungeonTile06);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 7:
                            spawnedTile = Instantiate(dungeonTile07);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 8:
                            spawnedTile = Instantiate(dungeonTile08);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 9:
                            spawnedTile = Instantiate(dungeonTile09);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 10:
                            spawnedTile = Instantiate(dungeonTile10);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 11:
                            spawnedTile = Instantiate(dungeonTile11);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 12:
                            spawnedTile = Instantiate(dungeonTile12);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 13:
                            spawnedTile = Instantiate(dungeonTile13);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 14:
                            spawnedTile = Instantiate(dungeonTile14);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        case 15:
                            spawnedTile = Instantiate(dungeonTile15);
                            spawnedTile.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);
                            break;
                        default:
                            Debug.Log("Tile index out of range");
                            break;
                    }

                    //spawnedTile.SetActive(false);


                }
            }
        }

    }

    int CalculateBitMask(int xPosition, int yPosition)
    {
        int bitmask= 0;

        if(levelLayout[xPosition,yPosition + 1])
        {
            bitmask += 1;
        }

        if(yPosition - 1 >= 0)
        {
            if (levelLayout[xPosition, yPosition - 1])
            {
                bitmask += 8;
            }
        }
        
        if (levelLayout[xPosition + 1, yPosition])
        {
            bitmask += 4;
        }
        
        if(xPosition - 1 >= 0)
        {
            if (levelLayout[xPosition - 1, yPosition])
            {
                bitmask += 2;
            }
        }
        
        
        return bitmask;
    }

    void PlacePlayer()
    {
        int playerStartPositionX;
        int playerStartPositionY;


        playerStartPositionX = startPositionX;
        playerStartPositionY = startPositionY;

        playerInstance = Instantiate(playerGameObject);
        playerInstance.transform.position = new Vector2(playerStartPositionX * spacingInterval, playerStartPositionY * spacingInterval);
        theDungeon[playerStartPositionX, playerStartPositionY].playerStart = true;

        if (isGeneratingTutorial)
        {
            playerInstance.gameObject.GetComponent<PlayerScript>().tutorialMode = true;
        }
            

    }

    void PlaceEnemies()
    {

        int relativePosX;
        int relativePosY;
        float monsterPlayerDistance;
        
        for (int x = 0; x < levelSizeX; x++)
        {
            for (int y = 0; y < levelSizeY; y++)
            {
                if (levelLayout[x, y])
                {
                                       
                    if (theDungeon[x, y].enemy && !theDungeon[x,y].playerStart)
                    {

                        relativePosX = Mathf.Abs(x - startPositionX);
                        relativePosY = Mathf.Abs(y - startPositionY);
                        monsterPlayerDistance = Mathf.Sqrt(Mathf.Pow(relativePosX, 2) + Mathf.Pow(relativePosY, 2));
                        
                        
                        if(monsterPlayerDistance > monsterStartingDistance)
                        {
                            enemyInstance = Instantiate(enemyGameObject);
                            enemyInstance.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);

                            if (isGeneratingTutorial)
                            {
                                enemyInstance.GetComponent<EnemyScript>().canMove = false;
                            }
                        }
                        
                        
                        

                    }
                }
                
                

            }

        }

    }

    void PlaceTreasure()
    {
        
        for (int x = 0; x < levelSizeX; x++)
        {
            for (int y = 0; y < levelSizeY; y++)
            {
                if (levelLayout[x, y])
                {
                    if (theDungeon[x, y].treasure && !theDungeon[x,y].enemy && !theDungeon[x,y].playerStart)
                    {
                        treasureInstance = Instantiate(treasureGameObject);
                        treasureInstance.transform.position = new Vector2(x * spacingInterval, y * spacingInterval);

                    }
                }
                    

            }

        }
    }

    void PlaceExit()
    {
        exitInstance = Instantiate(theExit);
        exitInstance.transform.position = new Vector2(endPositionX * spacingInterval, endPositionY * spacingInterval);
    }
}
