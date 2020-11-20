using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeController : MonoBehaviour
{

    public GameObject music;
    public float sliderVal;

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.Find("Music");
    }

    // Update is called once per frame
    void Update()
    {
        sliderVal = GetComponent<Slider>().value;
        music.GetComponent<AudioSource>().volume = sliderVal;
    }
}
