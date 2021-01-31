using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadHeroClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnColliderEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerClass player = FindObjectOfType<PlayerClass>();

            if(player.soulCount == player.maxSouls)
            {
                //Revive Animation
                //LoadScene();
            }
        }
    }
}
