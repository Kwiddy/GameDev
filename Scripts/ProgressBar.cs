using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    public int totalPeople;
    public int infectedCount;
    public Image mask;

    void Start()
    {
        totalPeople = 0;
        infectedCount = 0;
    }

    void Update()
    {
        GetFill();
    }

    void GetFill()
    {
        float fillAmount = (float)infectedCount / (float)totalPeople;
        mask.fillAmount = fillAmount;
        float percentInt;

        Text progressText = this.GetComponentInChildren<Text>();

        if (totalPeople != 0)
        {
            percentInt = (infectedCount * 100 ) / totalPeople;
        } 
        else 
        {
            percentInt = 0;
        }

        progressText.text = percentInt + "% Infected"; 
    }
}
