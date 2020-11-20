using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class currency : MonoBehaviour
{

    public int wealth;

    // Start is called before the first frame update
    void Start()
    {
        wealth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Text toDisplay = GetComponent<Text>();
        toDisplay.text = wealth.ToString();
    }
}
