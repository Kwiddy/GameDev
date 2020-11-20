using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TierLabel : MonoBehaviour
{
    public int tierNum;
    public GameObject progressBar;
    public GameObject progressionText;
    public GameObject endScreen;
    public GameObject policeStation;

    public AudioSource levelUpNoise;
    public AudioSource winningNoise;
    public AudioSource[] audioSources;

    // Tier variables
    public float infectionProb;
    public int popNum;
    public int pMobility;
    public int scoreTarget;
    public int currentScore;
    public bool increaseTier;
    public bool movingTitle;

    public bool endGame;

    public int priceOfHappiness;
    public int gymMembership;
    public int visorUpgrade;
    public int soldierRecruitment;
    public int numberOfProtestors;

    public int randomProtest;

    public Vector3 initPos;
    
    void Start()
    {
        progressBar = GameObject.Find("Progress Bar");
        progressionText = GameObject.Find("progressionText");
        endScreen = GameObject.Find("EndScreen");
        policeStation = GameObject.Find("Police Station");

        audioSources = progressionText.GetComponents<AudioSource>();
        levelUpNoise = audioSources[0];
        winningNoise = audioSources[1];

        movingTitle = false;
        increaseTier = true;
        endGame = false;

        tierNum = 0;

        progToOne();
        Text toDisplay = GetComponent<Text>();
        toDisplay.text = "Tier " + tierNum.ToString();

        initPos = progressionText.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (increaseTier == true)
        {
            if (tierNum == 0) {
                progToOne();
            } else if (tierNum == 1) {
                progToTwo();
            } else if (tierNum == 2) {
                progToThree();
            } else {
                progToGameEnd();
            }
        
            progressionText.GetComponent<Text>().text = "Tier " + tierNum;
            Text toDisplay = GetComponent<Text>();
            toDisplay.text = "Tier " + tierNum.ToString();
        }
        if (movingTitle)
        {
            moveTitle();
        }
    }

    void progToOne()
    {
        tierNum = 1;
        increaseTier = false;
        movingTitle = true;
        currentScore = 0;
        
        // Initialise Tier 1 difficulties
        infectionProb = 20;
        popNum = 10;
        pMobility = 5;
        priceOfHappiness = 5;
        visorUpgrade = 5;
        gymMembership = 2;
        soldierRecruitment = 10;
        scoreTarget = 5;
        numberOfProtestors = 1;

        // Reset the protest event
        policeStation.GetComponent<PoliceStation>().protestPresent = false;
        policeStation.GetComponent<PoliceStation>().protestBeen = false;
        randomProtest = Random.Range(1, scoreTarget/2);

        levelUpNoise.Play();
    }

    // Progression to Tier Two
    void progToTwo()
    {
        tierNum = 2;
        increaseTier = false;
        movingTitle = true;
        currentScore = 0;
        
        // Initialise Tier 2 difficulties
        infectionProb = 30;
        popNum = 10;
        pMobility = 7;
        priceOfHappiness = 7;
        visorUpgrade = 10;
        soldierRecruitment = 15;
        gymMembership = 4;
        scoreTarget = 10;
        numberOfProtestors = 2;

        // Reset the protest event
        policeStation.GetComponent<PoliceStation>().protestPresent = false;
        policeStation.GetComponent<PoliceStation>().protestBeen = false;
        randomProtest = Random.Range(1, scoreTarget/2);
        levelUpNoise.Play();
        resetTerrain();
    }

    // Progression to Tier Three
    void progToThree()
    {
        tierNum = 3;
        increaseTier = false;
        movingTitle = true;
        currentScore = 0;
        
        // Initialise Tier 3 difficulties
        infectionProb = 40;
        popNum = 15;
        pMobility = 9;
        priceOfHappiness = 9;
        visorUpgrade = 15;
        soldierRecruitment = 20;
        gymMembership = 6;
        scoreTarget = 15;
        numberOfProtestors = 3;

        // Reset the protest event
        policeStation.GetComponent<PoliceStation>().protestPresent = false;
        policeStation.GetComponent<PoliceStation>().protestBeen = false;
        randomProtest = Random.Range(1, scoreTarget/2);
        levelUpNoise.Play();
        resetTerrain();
    }

    void progToGameEnd()
    {
        increaseTier = false;
        winningNoise.Play();
        resetTerrain();
        activateEndScene(true);
    }

    void resetTerrain()
    {
        GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("human");
        foreach (GameObject human in toDestroy) GameObject.Destroy(human);

        progressBar.GetComponent<ProgressBar>().totalPeople = 0;
        progressBar.GetComponent<ProgressBar>().infectedCount = 0;
    }

    void moveTitle()
    {
        Vector3 titlePos = progressionText.transform.position;
        if (titlePos.y >= -400 ) {
            titlePos.y -= 4;
        } else {
            movingTitle = false;
            titlePos = initPos;
        }
        progressionText.transform.position = titlePos;
    }

    // Pass in True for a winning scenario, false for a losing scenario
    public void activateEndScene(bool win)
    {
        endGame = true;
        endScreen.GetComponent<Canvas>().enabled = true;
        GameObject winLose = GameObject.Find("resultText");
        if (win)
        {
            winLose.GetComponent<Text>().text = "You Win!";
        } else {
            winLose.GetComponent<Text>().text = "You Lose!";
        }
    }

    public void returnToMain()
    {
        SceneManager.LoadScene(0);
    }

}
