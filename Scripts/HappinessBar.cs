using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappinessBar : MonoBehaviour
{

    public int happinessScore;
    public Image mask;

    void Start()
    {
        happinessScore = 5;
    }

    void Update()
    {
        GetFill();
    }

    void GetFill()
    {
        float fillAmount = (float)happinessScore / 5;
        mask.fillAmount = fillAmount;

        Text progressText = this.GetComponentInChildren<Text>();

        progressText.text = "Public Satisfaction: " + happinessScore; 
    }
}
