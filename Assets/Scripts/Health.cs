﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int hearts;
    public PlayerClass player;

    public Image[] heartDisplay;

    // Start is called before the first frame update
    void Start()
    {
        hearts = player.health;
        UpdateHealthDisplay();
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
                heartDisplay[i].enabled = true;
            }
            else
            {
                heartDisplay[i].enabled = false;
            }
        }
    }

    public void UpdateHealth(int addToHeartCount)
    {
        hearts += addToHeartCount;
        UpdateHealthDisplay();
    }


}