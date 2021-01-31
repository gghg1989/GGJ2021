using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadHeroClass : MonoBehaviour
{
    public ParticleSystem revivingHeroParticles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerClass player = FindObjectOfType<PlayerClass>();

            if(player.soulCount == player.maxSouls)
            {
                GetComponentInChildren<Animator>().SetBool("Revived", true);
                revivingHeroParticles.Play();
            }
        }
    }

    public void DeadHeroRevived()
    {
        SceneManager.LoadScene(2);
    }
}
