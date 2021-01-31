using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int hearts;

    PlayerClass player;

    public Text text;

    public GameObject[] heartDisplay;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerClass>();

        hearts = player.health;
        UpdateHealthDisplay();

        UpdateSoulCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateHealthDisplay()
    {
        for (int i = 0; i < heartDisplay.Length; i++)
        {
            if (i < hearts)
            {
                heartDisplay[i].SetActive(true);
            }
            else
            {
                heartDisplay[i].SetActive(false);
            }
        }
    }

    public void UpdateHealth(int addToHeartCount)
    {
        hearts += addToHeartCount;
        UpdateHealthDisplay();
    }

    public void UpdateSoulCount(int soulCount)
    {
        text.text = soulCount.ToString();
    }


}
