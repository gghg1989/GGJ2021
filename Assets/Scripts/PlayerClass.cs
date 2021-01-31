﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerClass : MonoBehaviour, ControlSystem.IGameplayActions
{
    ControlSystem controls;

    public int health;
    public float power;
    public float moveSpeed = 3f;
    public bool attackEnabled = false;
    private int attackCountdown = 20;
    public int soulCount;
    public int attackPower;

    public Transform movePoint;

    public Health healthDisplay;

    public LayerMask CollisionLayer;

    private bool FacingRight = true;

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

        controls.Gameplay.Movement.actionMap.actionTriggered += context => OnActionTriggered(context);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        AttackCountdown();
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
            Debug.Log(moveVector);

            // Flip player side animation
            if (moveVector.x < 0 && FacingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                FacingRight = false;
            }
            else if (moveVector.x > 0 && !FacingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                FacingRight = true;
            }

            if (Mathf.Abs(moveVector.x) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(moveVector.x, 0f, 0f), 0.2f, CollisionLayer))
                {
                    // Update player direction and load relevant animation
                    GetComponentInChildren<Animator>().SetFloat("DirX", 1f);
                    GetComponentInChildren<Animator>().SetFloat("DirY", 0f);
                    GetComponentInChildren<Animator>().SetBool("Idle", false);
                    movePoint.position += new Vector3(moveVector.x, 0f, 0f);
                }
            }
            else if (Mathf.Abs(moveVector.y) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, moveVector.y, 0f), 0.2f, CollisionLayer))
                {
                    // Update player direction and load relevant animation
                    GetComponentInChildren<Animator>().SetFloat("DirX", 0f);
                    GetComponentInChildren<Animator>().SetFloat("DirY", moveVector.y);
                    GetComponentInChildren<Animator>().SetBool("Idle", false);
                    movePoint.position += new Vector3(0f, moveVector.y, 0f);
                }
            }

            if (moveVector.y == 0 && moveVector.x == 0)
            {
                GetComponentInChildren<Animator>().SetBool("Idle", true);
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
        if(collision.gameObject.tag == "Enemy" && !attackEnabled)
        {
            //Take Damage for now is default 1 damage an enemy
            TakeDamage();
        }

        if(collision.gameObject.tag == "ExtraLife")
        {
            //Player gains a life, update display
            health += 1;
            healthDisplay.UpdateHealth(1);
        }

        if(collision.gameObject.tag == "SuperPower")
        {
            //Attack is Enabled, Enemies can check for this when colliding with player.
            //In update, AttackCountdown is called to have a timer for how long this attack state is enabled.
            attackEnabled = true;
        }

        //Add to soul count
        if(collision.gameObject.tag == "Soul")
        {
            soulCount += 1;
            healthDisplay.UpdateSoulCount(soulCount);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage()
    {
        health -= 1;
        healthDisplay.UpdateHealth(-1);
        
        if(health == 0)
        {
            //Disable Player
            //Enter Death Scene
        }
    }    

    private void AttackCountdown()
    {
        if (attackEnabled == true)
        {
            attackCountdown -= 1;

            if (attackCountdown == 0)
            {
                attackEnabled = false;
                attackCountdown = 20;
            }
        }
    }

    void OnActionTriggered(InputAction.CallbackContext context)
    {
        string action = context.action.name;

        switch(action)
        {
            case "Movement":
                OnMovement(context);
                break;
            case "Use/Interact":
                OnUseInteract(context);
                break;
            default:
                break;
        }
    }
}
