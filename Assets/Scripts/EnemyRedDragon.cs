using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRedDragon : MonoBehaviour
{

    // enemy movement stats
    public float speed = 0.3f; // speed used for aggressive and very aggressive movement

    //enemy stats
    public int power;
    public int health;
    public Transform movePoint;

    public GameObject player;
    public LayerMask CollisionLayer;
    
    private bool canMove = true;
    private bool facingRight = true;

    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) 
        {
            MoveTorwardPlayer();
            transform.position = Vector2.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
        }
        
    }

    private void MoveTorwardPlayer()
    {
        // Get moveVector
        Vector2 moveVector = GetMoveDirection();

        if (Vector2.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            // Flip side animation
            if (moveVector.x < 0 && facingRight && moveVector.y == 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                facingRight = false;
            }
            else if (moveVector.x > 0 && !facingRight && moveVector.y == 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                facingRight = true;
            }

            if (Mathf.Abs(moveVector.x) == 1f)
            {
                // Update direction and load relevant animation
                GetComponentInChildren<Animator>().SetFloat("DirX", 1f);
                GetComponentInChildren<Animator>().SetFloat("DirY", 0f);
                movePoint.position += new Vector3(moveVector.x, 0f, 0f);
            }
            else if (Mathf.Abs(moveVector.y) == 1f)
            {
                // Update player direction and load relevant animation
                GetComponentInChildren<Animator>().SetFloat("DirX", 0f);
                GetComponentInChildren<Animator>().SetFloat("DirY", moveVector.y);
                movePoint.position += new Vector3(0f, moveVector.y, 0f);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerClass thePlayer;
        
        if(collision.gameObject.tag == "Player")
        {
            thePlayer = collision.gameObject.GetComponent<PlayerClass>();
            if (thePlayer.attackEnabled)
            {
                health -= 1;
                if (health <= 0)
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

    private void EnemyKilled()
    {
        //set the game objec inactive to return to object pool

        GetComponentInChildren<Animator>().SetBool("Dead", true);

        gameObject.GetComponent<Collider2D>().enabled = false;

        canMove = false;

    }

    private Vector2 GetMoveDirection()
    {
        Vector2[] compass = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};
        int[] availableMovementList = {0, 0, 0, 0};
        int[] targetDirList = {0, 0, 0, 0};
        int[] movementRankList = new int[] {0, 0, 0, 0};
        
        for (int i=0; i < compass.Length; i++)
        {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(compass[i].x, compass[i].y, 0f), 0.2f, CollisionLayer))
            {
                availableMovementList[i] = 1;
            }
        }

        Vector2 dirTowardPlayer = (Vector2) (player.transform.position - transform.position);

        if (dirTowardPlayer.x > 0) {
            targetDirList[1] += 1;
        }
        else
        {
            targetDirList[3] += 1;
        }

        if (dirTowardPlayer.y > 0) {
            targetDirList[0] += 1;
        }
        else
        {
            targetDirList[2] += 1;
        }

        if ( Mathf.Abs(dirTowardPlayer.x) > Mathf.Abs(dirTowardPlayer.y) )
        {
            if (dirTowardPlayer.x > 0)
            {
                targetDirList[1] += 1;
            }
            else{
                targetDirList[3] += 1;
            }
        }
        else{
            if (dirTowardPlayer.y > 0)
            {
                targetDirList[0] += 1;
            }
            else{
                targetDirList[2] += 1;
            }
        }

        for (int i=0; i < compass.Length; i++)
        {
            movementRankList[i] =targetDirList[i] * availableMovementList[i];
        }
        
        int maxValue = Mathf.Max(movementRankList);
        for (int i=0; i < compass.Length; i++)
        {
            if (movementRankList[i] == maxValue)
            {
                return compass[i];
            }
        }
        return Vector2.zero;
    }
}
