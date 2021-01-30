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

    public enum enemyAttitude
    {
        veryAggressive, // always will chase the player from any distance
        aggressive, //only chases the player when within the radius set
        passive, //moves toward the player slowly
        doesntCare, //ignores the player, and just idly moves around
    }
    
    public enemyType thisEnemysType;
    public enemyAttitude thisEnemysAttitude;

    // enemy movement stats
    public float enemySpeed; // speed used for aggressive and very aggressive movement
    public float idleSpeed; // speed used for passive and doesn't care movement

    //enemy stats
    public float enemyPower;
    public float enemyLootDropChance;

    //random number utility class
    RandNumberManager randGen;

    //movement variables
    public bool isChasingPlayer;
    public float idleMoveRadius; // a misnomer, this is the value we are multiplying a random vector by to get the radius of idle movement
    public bool isIdle;
    public bool isMoving;

    public float moveIncrement; //the base move increment the enemy will take. 
    public float unStuckMove; // how much the enemy moves to get un stuck


    //variables for checking if the enemy is hitting the walls in the collision
    public float colliderCheckRadius;

    //collision layer mask, setting in inspector to the stop move layer
    // this lets us check for collisions just on that layer
    public LayerMask collisionLayer;

    Vector3 currentPosition;
    Vector3 previousPosition;

    //reference to the player game object
    public GameObject playerGameObject;
    public GameObject playerMovePoint;

    //reference to the point that is the enemys position on the map
    Transform movePoint;
    Vector2 movePointOffset;

    void Start()
    {
        randGen = new RandNumberManager();
        currentPosition = transform.position;
        previousPosition = currentPosition;

        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {

        MoveTowardPlayer();

    }

    void MoveToPoint(Vector2 targetPoint)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, enemySpeed);
    }

    void MoveTowardPlayer()
    {
        Vector2 playerDirection;
        currentPosition = transform.position;
        playerDirection = transform.position - playerGameObject.transform.position;

        //are we at the player's location?
        if(transform.position != playerMovePoint.transform.position)
        {
            if (!Physics2D.OverlapCircle(transform.position + new Vector3(moveIncrement, 0f), colliderCheckRadius, collisionLayer)) // if the collider doesn't hit anything, then move toward the player along the x axis
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerGameObject.transform.position.x, transform.position.y), enemySpeed);
            }

            //move along the y axis
            if (!Physics2D.OverlapCircle(transform.position + new Vector3(0f, moveIncrement), colliderCheckRadius, collisionLayer)) // move in y axis as above
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, playerGameObject.transform.position.y), enemySpeed);
            }
        }

        previousPosition = currentPosition;
        
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

    void IdleMovement()
    {

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


}
