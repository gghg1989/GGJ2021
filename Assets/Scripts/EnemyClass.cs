using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    // Start is called before the first frame update
    public enum enemyType
    {
        redEnemy,
        pinkEnemy,
        cyanEnemy,
        yellowEnemy,
    }
    
    public enemyType thisEnemyType;
    public float enemySpeed;
    public float enemyPower;
    public float enemyLootDropChance;




    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
