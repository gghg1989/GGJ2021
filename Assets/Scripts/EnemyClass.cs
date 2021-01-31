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
    
    public enum direction
    {
        north,
        south,
        east,
        west,
        nowhere
    }

    public enemyType thisEnemysType;
    public enemyAttitude thisEnemysAttitude;

    // enemy movement stats
    public float enemySpeed; // speed used for aggressive and very aggressive movement
    public float idleSpeed; // speed used for passive and doesn't care movement

    //enemy stats
    public int enemyPower;
    public float enemyLootDropChance;
    public int enemyHealth;
    public float enemyDetectDistance;

    //random number utility class
    RandNumberManager randGen;

    //movement variables
    public bool isChasingPlayer;
    public float idleMoveRadius; // a misnomer, this is the value we are multiplying a random vector by to get the radius of idle movement
    public bool isIdle;
    [SerializeField] bool isMoving;
    public int numberOfInvalidMoves;
    public int maxInvalidMoves;

    bool canMove;

    public float moveIncrement; //the base move increment the enemy will take, this is the center to center distance of the tiles
    public float unStuckMove; // how much the enemy moves to get un stuck


    //variables for checking if the enemy is hitting the walls in the collision
    public float colliderCheckRadius;

    //collision layer mask, setting in inspector to the stop move layer
    // this lets us check for collisions just on that layer
    public LayerMask collisionLayer;

    Vector3 currentPosition;
    Vector3 previousPosition;

    Vector3 targetPosition;

    //reference to the player game object
    public GameObject playerGameObject;
    public GameObject playerMovePoint;

    //reference to the point that is the enemys position on the map
    Transform movePoint;
    Vector2 movePointOffset;

    public Animator dragonAnimator;

    void Start()
    {
        randGen = new RandNumberManager();
        currentPosition = transform.position;
        previousPosition = currentPosition;

        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        targetPosition = transform.position;
        isMoving = false;
        dragonAnimator = gameObject.GetComponentInChildren<Animator>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {

        currentPosition = transform.position;

        if (canMove)
        {
            MoveTowardPlayer();
        }
        

        previousPosition = currentPosition;
        

    }

    void MoveToPoint(Vector2 targetPoint)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, enemySpeed);
    }

    void MoveTowardPlayer()
    {

        direction moveDirection;

        int randomDirection;

        moveDirection = DeterminePlayerDirection();

        if (IsValidMove(moveDirection) && !isMoving)
        {
                switch (moveDirection)
                {
                    case direction.north:
                        targetPosition = new Vector2(transform.position.x, transform.position.y + moveIncrement);
                        isMoving = true;
                        dragonAnimator.SetFloat("DirX", 0f);
                        dragonAnimator.SetFloat("DirY", 1f);

                    break;
                    case direction.south:
                        targetPosition = new Vector2(transform.position.x, transform.position.y - moveIncrement);
                        isMoving = true;
                        dragonAnimator.SetFloat("DirX", 0f);
                        dragonAnimator.SetFloat("DirY", -1f);
                    break;
                    case direction.east:
                        targetPosition = new Vector2(transform.position.x + moveIncrement, transform.position.y);
                        isMoving = true;
                        dragonAnimator.SetFloat("DirX", 1f);
                        dragonAnimator.SetFloat("DirY", 0f);
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    break;
                    case direction.west:
                        targetPosition = new Vector2(transform.position.x - moveIncrement, transform.position.y);
                        isMoving = true;
                        dragonAnimator.SetFloat("DirX", -1f);
                        dragonAnimator.SetFloat("DirY", 0f);
                    transform.localScale = new Vector3(transform.localScale.x* -1, transform.localScale.y, transform.localScale.z);
                    break;
                    default:
                        targetPosition = transform.position;
                        isMoving = false;
                        break;
                }
            
        }

        if (!IsValidMove(moveDirection))
        {
            numberOfInvalidMoves++;
        }

        if (!isMoving && numberOfInvalidMoves > maxInvalidMoves)
        {
            randomDirection = randGen.GenerateRandomInt(1, 4);

            switch (randomDirection)
            {
                case 1: //north
                    if (IsValidMove(direction.north))
                    {
                        targetPosition = new Vector2(transform.position.x, transform.position.y + moveIncrement);
                        isMoving = true;
                    }
                    break;

                case 2: //south
                    if (IsValidMove(direction.south))
                    {
                        targetPosition = new Vector2(transform.position.x, transform.position.y - moveIncrement);
                        isMoving = true;
                    }
                    break;
                case 3: //east
                    if (IsValidMove(direction.east))
                    {
                        targetPosition = new Vector2(transform.position.x + moveIncrement, transform.position.y);
                        isMoving = true;
                    }
                    break;
                case 4: //west
                    if (IsValidMove(direction.west))
                    {
                        targetPosition = new Vector2(transform.position.x - moveIncrement, transform.position.y);
                        isMoving = true;
                    }
                    break;

                default:
                    targetPosition = transform.position;
                    isMoving = false;
                    break;
            }
            
            numberOfInvalidMoves = 0;
        }

        if (transform.position == targetPosition)
        {
            isMoving = false;
            
        }



        if (isMoving)
        {
            MoveToPoint(targetPosition);
            //dragonAnimator.SetBool("isMovingRight", false);
            //dragonAnimator.SetBool("isMovingUp", false);
            //dragonAnimator.SetBool("isMovingDown", false);
            //dragonAnimator.SetBool("isMovingLeft", false);
        }
            

        
    }

    direction DeterminePlayerDirection()
    {
        direction enemyMoveDirection = direction.nowhere;

        Vector3 playerDirection;

        playerDirection = playerGameObject.transform.position - gameObject.transform.position;


        if (Mathf.Abs(playerDirection.x) >= Mathf.Abs(playerDirection.y))
        {

            if (playerDirection.x >= 0)
            {
                enemyMoveDirection = direction.east;

            }

            if (playerDirection.x < 0)
            {
                enemyMoveDirection = direction.west;
            }

        }




        if (Mathf.Abs(playerDirection.x) < Mathf.Abs(playerDirection.y))
        {

            if (playerDirection.y >= 0)
            {
                enemyMoveDirection = direction.north;
            }

            if (playerDirection.y < 0)
            {
                enemyMoveDirection = direction.south;
            }

        }

        if (playerDirection.magnitude > enemyDetectDistance)
        {
            enemyMoveDirection = direction.nowhere;
        }

        return enemyMoveDirection;
    }

    bool IsValidMove(direction targetDirection)
    {
        bool validMove = false;

        switch (targetDirection)
        {
            case direction.north:
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(0f, moveIncrement), colliderCheckRadius, collisionLayer)) //check the north
                {
                    validMove = true;
                }
                break;

            case direction.south:
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(0f, -moveIncrement), colliderCheckRadius, collisionLayer)) //check the north
                {
                    validMove = true;
                }
                break;
            case direction.east:
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(moveIncrement, 0f), colliderCheckRadius, collisionLayer)) //check the north
                {
                    validMove = true;
                }
                break;

            case direction.west:
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(-moveIncrement, 0f), colliderCheckRadius, collisionLayer)) //check the north
                {
                    validMove = true;
                }
                break;
            default:
                validMove = false;
                break;
        }




            return validMove;
    }

    void EnemyKilled()
    {
        //set the game objec inactive to return to object pool

        dragonAnimator.SetBool("Dead", true);

        gameObject.GetComponent<Collider2D>().enabled = false;

        canMove = false;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        
        PlayerClass thePlayer;
        
        if(collision.gameObject.tag == "Player")
        {
            

            thePlayer = collision.gameObject.GetComponent<PlayerClass>();
            if (thePlayer.attackEnabled)
            {
                enemyHealth -= 1;
                if (enemyHealth <= 0)
                {
                    EnemyKilled();
                }
            }
            else
            {
                thePlayer.TakeDamage();
            }
        }
        
    }


}
