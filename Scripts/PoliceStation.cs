using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoliceStation : MonoBehaviour
{

    public GameObject shopCanvas;
    public GameObject closeButton;
    public GameObject gymButton;
    public GameObject visorsButton;
    public GameObject armyButton;
    public GameObject regenButton;
    public GameObject currencyObj;
    public GameObject errorText;
    public GameObject happinessBar;
    public GameObject tierIdentifier;
    public GameObject policeStation;

    public GameObject[] theBarricade;
    public GameObject[] protestors;
    public GameObject protestor;

    public GameObject itemPanel;
    public GameObject itemHeader;
    public GameObject itemCancel;
    public GameObject mainPanel;
    public GameObject itemBody;
    public GameObject itemAttrib;
    public GameObject itemCost;
    public GameObject itemBuyButton;
    public GameObject itemAdditional;

    public Button itemBuyBtn;

    public bool shopActive;
    public bool createSoldier;

    public bool protestPresent;
    public bool protestBeen;

    public float officerSpeed;
    public int infectionDistance;
    public int soldierNum;

    public Vector3 initStationSize;

    public AudioSource buttonPop;
    public AudioSource buyNoise;
    public AudioSource failedAttempt;
    public AudioSource[] audioSources;

    public int currentCost;

    // Start is called before the first frame update
    void Start()
    {
        officerSpeed = 5;
        infectionDistance = 5;
        soldierNum = 0;

        protestPresent = false;
        protestBeen = false;

        shopCanvas = GameObject.Find("Police Shop");
        shopActive = false;

        currencyObj = GameObject.Find("Currency");

        errorText = GameObject.Find("ErrorText");

        policeStation = GameObject.Find("Police Station");
        initStationSize = policeStation.transform.localScale;

        happinessBar = GameObject.Find("Happiness Bar");
        tierIdentifier = GameObject.Find("TierIdentifier");

        mainPanel = GameObject.Find("MainPanel");
        mainPanel.GetComponent<Canvas>().enabled = true;

        audioSources = GetComponents<AudioSource>();
        buyNoise = audioSources[0];
        buttonPop= audioSources[1];
        failedAttempt = audioSources[2];

        itemPanel = GameObject.Find("ItemPanel");
        itemPanel.GetComponent<Canvas>().enabled = false;
        itemHeader = GameObject.Find("TitleText");
        itemBody = GameObject.Find("BodyText");
        itemAttrib = GameObject.Find("AttribText");
        itemCost = GameObject.Find("CostText");
        itemCancel = GameObject.Find("CancelButton");
        itemBuyButton = GameObject.Find("BuyButton");
        itemAdditional = GameObject.Find("Additional");
        itemBuyBtn = itemBuyButton.GetComponent<Button>();

        Button closeBtn = closeButton.GetComponent<Button>();
		closeBtn.onClick.AddListener(closeUI);

        Button gymBtn = gymButton.GetComponent<Button>();
        gymBtn.onClick.AddListener(gymUpgrade);

        Button regenBtn = regenButton.GetComponent<Button>();
        regenBtn.onClick.AddListener(inspiriationalSpeech);

        Button visorsBtn = visorsButton.GetComponent<Button>();
        visorsBtn.onClick.AddListener(equipVisor);

        Button armyBtn = armyButton.GetComponent<Button>();
        armyBtn.onClick.AddListener(callArmy);

        Button cancelBtn = itemCancel.GetComponent<Button>();
        cancelBtn.onClick.AddListener(closeItem);

        // Make Protest event invisible initially
        theBarricade = GameObject.FindGameObjectsWithTag("barricade");
        foreach (GameObject fence in theBarricade) fence.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!protestPresent) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // Detects when hovering over the police station
            if(Physics.Raycast (ray, out hit))
            {
                if(hit.transform.name == "Police Station" && shopActive == false)
                {
                    GameObject station = GameObject.Find(hit.transform.name);
                    float enlargeRange = Vector3.Distance(station.transform.localScale, initStationSize);
                    if (enlargeRange < 0.008)
                    {
                        station.transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
                    }
                    detectOpen();
                } else {
                    policeStation.transform.localScale = initStationSize;
                }
            }
        } else {
            startProtest();
        }
    }

    void startProtest()
    {
        if (protestBeen == false)
        {
            foreach (GameObject fence in theBarricade) fence.SetActive(true);
            float xPos = transform.position.x-5;
            float zPos = transform.position.z-11;

            for (int i = 0; i < tierIdentifier.GetComponent<TierLabel>().numberOfProtestors; i++)
            {
                GameObject newPerson = Instantiate(protestor, new Vector3(xPos, 0, zPos), Quaternion.identity);
                xPos += 5;
            }

            protestors = GameObject.FindGameObjectsWithTag("protestor");
            protestBeen = true;
            StartCoroutine(protestDelay());
        }
    }

    private IEnumerator protestDelay()
    {
        yield return new WaitForSeconds (tierIdentifier.GetComponent<TierLabel>().numberOfProtestors * 5);
        protestPresent = false;
        foreach (GameObject fence in theBarricade) fence.SetActive(false);
        foreach (GameObject person in protestors) GameObject.Destroy(person);
    }

    void closeUI()
    {
        buttonPop.Play();
        shopCanvas.GetComponent<Canvas>().enabled = false;
        errorText.GetComponent<Text>().text = "";
        shopActive = false;
	}

    void closeItem()
    {
        buttonPop.Play();
        mainPanel.GetComponent<Canvas>().enabled = true;
        itemPanel.GetComponent<Canvas>().enabled = false;
        errorText.GetComponent<Text>().text = "";
        itemAdditional.GetComponent<Text>().text = "";
    }

    void gymUpgrade()
    {
        buttonPop.Play();
        currentCost = tierIdentifier.GetComponent<TierLabel>().gymMembership;
        mainPanel.GetComponent<Canvas>().enabled = false;
        itemPanel.GetComponent<Canvas>().enabled = true;
        itemHeader.GetComponent<Text>().text = "Officer Gym Membership";
        itemAttrib.GetComponent<Text>().text = "Upgrade: +1 Officer Movement Speed";
        itemCost.GetComponent<Text>().text = "Cost: " + currentCost + " masks";
        itemBody.GetComponent<Text>().text = "Invest in speeding up your officers. Training on the track? Roller blades for shoes? Extra strong coffee? Who knows which of these will work, but one of them should help you catch the infected just a little bit faster.";
        itemAdditional.GetComponent<Text>().text = "Current Speed Level: " + ((officerSpeed-3)/2) + "/5";

        itemBuyBtn.onClick.RemoveAllListeners();
        itemBuyBtn.onClick.AddListener(addSpeed);
    }

    void addSpeed()
    {
        // Need to replace below with money sound
        buttonPop.Play();
        if (currencyObj.GetComponent<currency>().wealth >= currentCost)
        {
            buyNoise.Play();
            officerSpeed += 2;
            if (officerSpeed > 12) {
                Button gymBtn = gymButton.GetComponent<Button>();
                gymBtn.interactable = false;
                gymButton.GetComponentInChildren<Text>().text = "Max. Speed Reached";
                closeItem();
            }
            currencyObj.GetComponent<currency>().wealth -= currentCost;
            itemAdditional.GetComponent<Text>().text = "Current Speed Level: " + ((officerSpeed-3)/2) + "/5";
        }
        else
        {
            failedAttempt.Play();
            errorText.GetComponent<Text>().text = "Insufficient Funds";
        }
    }

    // Regenerating Public Satisfaction
    void inspiriationalSpeech()
    {
        buttonPop.Play();
        currentCost = tierIdentifier.GetComponent<TierLabel>().priceOfHappiness;
        mainPanel.GetComponent<Canvas>().enabled = false;
        itemPanel.GetComponent<Canvas>().enabled = true;
        itemHeader.GetComponent<Text>().text = "Inspirational Speech";
        itemAttrib.GetComponent<Text>().text = "Upgrade: +1 Public Satisfaction";
        itemCost.GetComponent<Text>().text = "Cost: " + currentCost + " masks";
        itemBody.GetComponent<Text>().text = "Everyone needs a good speech now and then. Get your speech writer to draft up something truly special to calm everyone down and help keep this virus under control.";
        itemAdditional.GetComponent<Text>().text = "Current Satisfaction Level: " + happinessBar.GetComponent<HappinessBar>().happinessScore;

        itemBuyBtn.onClick.RemoveAllListeners();
        itemBuyBtn.onClick.AddListener(addHappiness);
    }

    void addHappiness()
    {
        Button regenBtn = regenButton.GetComponent<Button>();
        if (happinessBar.GetComponent<HappinessBar>().happinessScore != 5)
        {
            if (currencyObj.GetComponent<currency>().wealth >= currentCost)
            {
                buyNoise.Play();
                happinessBar.GetComponent<HappinessBar>().happinessScore += 1;
                currencyObj.GetComponent<currency>().wealth -= currentCost;
                if (happinessBar.GetComponent<HappinessBar>().happinessScore == 5)
                {
                    regenBtn.interactable = false;
                    regenButton.GetComponentInChildren<Text>().text = "Max. Public Satisfaction";
                    closeItem();
                }
                itemAdditional.GetComponent<Text>().text = "Current Satisfaction Level: " + happinessBar.GetComponent<HappinessBar>().happinessScore;
            } else {
                failedAttempt.Play();
                errorText.GetComponent<Text>().text = "Insufficient Funds";
            }
        }
    }

    void equipVisor()
    {
        buttonPop.Play();
        currentCost = tierIdentifier.GetComponent<TierLabel>().visorUpgrade;
        mainPanel.GetComponent<Canvas>().enabled = false;
        itemPanel.GetComponent<Canvas>().enabled = true;
        itemHeader.GetComponent<Text>().text = "Visors and Sanitiser";
        itemAttrib.GetComponent<Text>().text = "Upgrade: Reduce infection distance";
        itemCost.GetComponent<Text>().text = "Cost: " + currentCost + " masks";
        itemBody.GetComponent<Text>().text = "You can't stop the spread, but you can certainly slow it down. You revolutionise the police force with genius ideas such as: double (or even triple!) layering masks on top of each other, reusing riot gear as covid visors, and forcing your officers to bathe in large vats of hand sanitiser.";
        itemAdditional.GetComponent<Text>().text = "Current Protection Level: " + (6-infectionDistance) + "/3";

        itemBuyBtn.onClick.RemoveAllListeners();
        itemBuyBtn.onClick.AddListener(moreProtection);
    }

    void moreProtection()
    {
        Button visorsBtn = visorsButton.GetComponent<Button>();
        if (6-infectionDistance < 3)
        {
            if (currencyObj.GetComponent<currency>().wealth >= currentCost)
            {
                buyNoise.Play();
                infectionDistance -= 1;

                GameObject[] people = GameObject.FindGameObjectsWithTag("human");
                foreach (GameObject human in people) {
                    var personCollider = human.transform.gameObject.GetComponent<BoxCollider>();
                    personCollider.size = new Vector3(infectionDistance, 5, infectionDistance);
                }

                currencyObj.GetComponent<currency>().wealth -= currentCost;
                if (6-infectionDistance == 3)
                {
                    visorsBtn.interactable = false;
                    visorsButton.GetComponentInChildren<Text>().text = "Max. Distancing Protection";
                    closeItem();
                }
                itemAdditional.GetComponent<Text>().text = "Current Protection Level: " + (6-infectionDistance) + "/3";
            } else {
                failedAttempt.Play();
                errorText.GetComponent<Text>().text = "Insufficient Funds";
            }
        }
    }

    void callArmy()
    {
        buttonPop.Play();
        currentCost = tierIdentifier.GetComponent<TierLabel>().soldierRecruitment;
        mainPanel.GetComponent<Canvas>().enabled = false;
        itemPanel.GetComponent<Canvas>().enabled = true;
        itemHeader.GetComponent<Text>().text = "Army Buddy";
        itemAttrib.GetComponent<Text>().text = "Upgrade: Syringe Wielding Soldier";
        itemCost.GetComponent<Text>().text = "Cost: " + currentCost + " masks";
        itemBody.GetComponent<Text>().text = "Apparently there's an experimental vaccine that the army has managed to get it's hands on, rumour has it that it can immediately cure the infected! It sure is lucky that you happened to stay in touch with your old military pals.";
        itemAdditional.GetComponent<Text>().text = "Current Number of Soldiers: " + soldierNum + "/3";

        itemBuyBtn.onClick.RemoveAllListeners();
        itemBuyBtn.onClick.AddListener(gimmeVaccine);
    }

    void gimmeVaccine()
    {
        Button armyBtn = armyButton.GetComponent<Button>();
        if (soldierNum < 3)
        {
            if (currencyObj.GetComponent<currency>().wealth >= currentCost)
            {
                buyNoise.Play();
                createSoldier = true;
                soldierNum += 1;
                currencyObj.GetComponent<currency>().wealth -= currentCost;
                if (soldierNum == 3)
                {
                    armyBtn.interactable = false;
                    armyButton.GetComponentInChildren<Text>().text = "All Soldiers Active";
                    closeItem();
                }
                itemAdditional.GetComponent<Text>().text = "Current Number of Soldiers: " + soldierNum + "/3";
            } else {
                failedAttempt.Play();
                errorText.GetComponent<Text>().text = "Insufficient Funds";
            }
        }
    }

    void detectOpen()
    {
        Button regenBtn = regenButton.GetComponent<Button>();
        Button gymBtn = gymButton.GetComponent<Button>();

        if (Input.GetMouseButtonDown (0)) {
            shopCanvas.GetComponent<Canvas>().enabled = true;
            mainPanel.GetComponent<Canvas>().enabled = true;
            itemPanel.GetComponent<Canvas>().enabled = false;
            buttonPop.Play();
            shopActive = true;
        }

        if (happinessBar.GetComponent<HappinessBar>().happinessScore != 5)
        {
            regenBtn.interactable = true;
            regenButton.GetComponentInChildren<Text>().text = "Inspirational Speech";
        } else {
            regenBtn.interactable = false;
            regenButton.GetComponentInChildren<Text>().text = "Max. Public Satisfaction";
        }

        if (officerSpeed <= 12)
        {
            gymBtn.interactable = true;
        } else {
            gymBtn.interactable = false;
            gymButton.GetComponentInChildren<Text>().text = "Max. Speed Reached";
        }
    }
}
