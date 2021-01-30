using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    public enum enemyType
    {
        redEnemy,
        pinkEnemy,
        cyanEnemy,
        yellowEnemy,
    }
    
    public enemyType thisEnemyType;
    
    // enemy stats
    public float enemySpeed;
    public float idleSpeed;

    public float enemyPower;
    public float enemyLootDropChance;

    //random number utility class
    RandNumberManager randGen;


    //movement variables
    public bool isChasingPlayer;
    public float idleMoveRadius; // a misnomer, this is the value we are multiplying a random vector by to get the radius of idle movement
    public bool isIdle;
    public bool isMoving;
    
    

    Vector3 currentPosition;
    Vector3 previousPosition;

    //temporary or testing variables
    public GameObject testingPlayerGameObject;


    void Start()
    {
        randGen = new RandNumberManager();
        currentPosition = transform.position;
        previousPosition = currentPosition;

        testingPlayerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        MoveToPoint(testingPlayerGameObject.transform.position);

    }

    void MoveToPoint(Vector2 targetPoint)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, enemySpeed);
    }

    void GetPlayerData()
    {
        //receives player data

    }

    void EnemyKilled()
    {
        //set the game objec inactive to return to object pool

        gameObject.SetActive(false);
    }

    void IdleMovement(Vector3 targetPosition)
    {
        bool isAtTarget;

        transform.Translate(DetermineIdleMovementTarget());
        


    }

    Vector3 DetermineIdleMovementTarget()
    {
        Vector3 targetOffset;
        Vector3 worldTarget;

        //randomly generate a direction to move toward
        targetOffset.x = randGen.GenerateRandomWithinLimits(-1, 1);
        targetOffset.y = randGen.GenerateRandomWithinLimits(-1, 1);
        targetOffset.z = 0f;

        //multiply the direction (since it's normalized) by how far you want the idle move to happen
        targetOffset = targetOffset * idleMoveRadius;
        Debug.Log("target offset" + targetOffset);


        //take the current position and add the direction.
       // worldTarget = transform.position + targetOffset;
       // Debug.Log("world target" + worldTarget);

        return targetOffset;

    }

    bool IsTheEnemyMoving()
    {
        bool isMoving;

        currentPosition = transform.position;

        if (currentPosition == previousPosition)
        {
            isMoving = false;
        }
        else isMoving = true;

        previousPosition = currentPosition;

        return isMoving;

    }

    void ChasePlayer(Vector2 playerLocation)
    {

    }

    void MoveAfterHittingWall(Vector3 locationOfWall)
    {
        Vector3 travelDirection;

        travelDirection = transform.position - locationOfWall;

        transform.Translate(travelDirection.normalized * enemySpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            MoveAfterHittingWall(collision.transform.position);
        }
    }



}
