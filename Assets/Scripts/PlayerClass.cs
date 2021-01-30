using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClass : MonoBehaviour, ControlSystem.IGameplayActions
{
    ControlSystem controls;

    public int health;
    public float power;
    public float moveSpeed = 5f;
    public Transform movePoint;

    public Health healthDisplay;

    public LayerMask CollisionLayer;

    // Start is called before the first frame update
    void Start()
    {
        //if no controls, create new one
        if (controls == null)
        {
            controls = new ControlSystem();

            controls.Gameplay.SetCallbacks(this);
        }

        //enable control input
        controls.Gameplay.Enable();

        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        //disable when player is disabled
        controls.Gameplay.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (Vector2.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            Vector2 moveVector = context.ReadValue<Vector2>();

            if (Mathf.Abs(moveVector.x) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(moveVector.x, 0f, 0f), 0.2f, CollisionLayer))
                {
                    // Update player direction and load relevant animation
                    GetComponent<Animator>().SetFloat("DirX", moveVector.x);
                    GetComponent<Animator>().SetFloat("DirY", 0f);
                    movePoint.position += new Vector3(moveVector.x, 0f, 0f);
                }
            }
            else if (Mathf.Abs(moveVector.y) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, moveVector.y, 0f), 0.2f, CollisionLayer))
                {
                    // Update player direction and load relevant animation
                    GetComponent<Animator>().SetFloat("DirX", 0f);
                    GetComponent<Animator>().SetFloat("DirY", moveVector.y);
                    movePoint.position += new Vector3(0f, moveVector.y, 0f);
                }
            }
        }
    }

    public void OnUseInteract(InputAction.CallbackContext context)
    {
        //To be used for potion and shop idea later if applicable
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //To be changed if we have a different damage mechanic in mind
        if(collision.gameObject.GetComponent<EnemyClass>() != null)
        {
            EnemyClass enemy = collision.gameObject.GetComponent<EnemyClass>();

            TakeDamage(enemy.enemyPower);
        }
        if(collision.gameObject.tag == "ExtraLife")
        {
            health += 1;
            healthDisplay.UpdateHealth(1);
        }
    }

    public void TakeDamage(float enemyPower)
    {
        health -= 1;
        healthDisplay.UpdateHealth(-1);
    }    
}
