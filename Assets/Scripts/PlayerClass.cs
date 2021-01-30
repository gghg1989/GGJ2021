using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClass : MonoBehaviour
{
    ControlSystem controls;

    public int health;
    public float power;
    public float moveSpeed;



    // Start is called before the first frame update
    void Start()
    {
        //if no controls, create new one
        if (controls == null)
        {
            controls = new ControlSystem();

            controls.Gameplay.SetCallbacks((ControlSystem.IGameplayActions)this);
        }

        //enable control input
        controls.Gameplay.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        //disable when player is disabled
        controls.Gameplay.Disable();
    }

    public void OnVerticalMovement(InputAction.CallbackContext context)
    {
        //Enter 'Vertical Movement' code here
    }

    public void OnHorizontalMovement(InputAction.CallbackContext context)
    {
        //Enter 'Horizontal Movement' code here
    }

    public void OnUse(InputAction.CallbackContext context)
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
    }

    public void TakeDamage(float enemyPower)
    {
        //Enter 'Take Damage' code here
    }    
}
