using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandNumberManager
{

    float randomPercentage;
    float randomWithinLimits;
    float highLimit;
    float lowLimit;

    public float GenerateRandomPercentage()
    {
        randomPercentage = Random.Range(0f, 1f);

        return randomPercentage;

    }


    public float GenerateRandomWithinLimits(float lowLimit, float highLimit)
    {
        randomWithinLimits = Random.Range(lowLimit, highLimit);        
        
        return randomWithinLimits;
    }

    public bool GenerateRandomBool()
    {
        if (GenerateRandomPercentage() < .5f)
        {
            return false;
        }

        if (GenerateRandomPercentage() >= .5f)
        {
            return true;
        }

        else return false;

    }

    public int GenerateRandomInt(int lowLimit, int highLimit)
    {
        int generatedInteger;

        generatedInteger = Random.Range(lowLimit, highLimit + 1);

        return generatedInteger;
    }



}
