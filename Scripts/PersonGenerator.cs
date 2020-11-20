using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonGenerator : MonoBehaviour
{
    public GameObject aPerson;
    public GameObject aZombie;
    public GameObject toGenerate;
    public GameObject anOfficer;
    public GameObject aSoldier;
    public int personID;

    // Current Position of the Parent Building
    public float xParent;
    public float zParent;

    //Positions to generate
    public float xPos;
    public float zPos;

    public bool infectedBool;
    public bool certainInfected;
    public bool safeSpawn;

    public GameObject terrainParent;
    public GameObject progressBar;
    public GameObject happinessBar;
    public GameObject tierIdentifier;
    public GameObject policeStation;
    public GameObject mainTerrain;
    public GameObject cameraRig;

    //Proabability of an infected person, Range(0,100)
    public float covidProb;
    public int delayTime;
    public int delayTwo;
    public bool triggerLostRespect;

    // Start is called before the first frame update
    void Start()
    {
        personID = 1;
        delayTime = Random.Range(1, 120); //changing the 120 changes the speed at which they spawn in

        //Get Covid Probability from Tier 1
        tierIdentifier = GameObject.Find("TierIdentifier");
        covidProb = tierIdentifier.GetComponent<TierLabel>().infectionProb;

        progressBar = GameObject.Find("Progress Bar");
        happinessBar = GameObject.Find("Happiness Bar");
        mainTerrain = GameObject.Find("Main Terrain");
        policeStation = GameObject.Find("Police Station");
        cameraRig = GameObject.Find("Camera Rig");

        StartCoroutine(DelaySpawn());
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds (delayTime);
        StartCoroutine(PersonDrop());
    }

    IEnumerator PersonDrop()
    {
        
        xParent = transform.position.x;
        zParent = transform.position.z;


        while (Application.isPlaying)
        {
            if (transform.rotation == Quaternion.Euler(0, 0, 0))
            {
                xPos = xParent;
                zPos = zParent + 10; 
            } 
            else if (transform.rotation == Quaternion.Euler(0, 90, 0)) 
            {
                xPos = xParent + 10;
                zPos = zParent; 
            }
            else if (transform.rotation == Quaternion.Euler(0, 180, 0)) 
            {
                xPos = xParent;
                zPos = zParent - 10; 
            }
            else if (transform.rotation == Quaternion.Euler(0, 270, 0)) 
            {
                xPos = xParent - 10;
                zPos = zParent; 
            }

            // Without this, newPerson may have e.g. 0.2 chance of spawning infected but as it's only a chance it is therefore a possibility
            //      to get 20 people spawn with none infected. This ensures that a minimum percentage of the new spawns are infected.
            if (progressBar.GetComponent<ProgressBar>().totalPeople != 0)
            {
                float popInfected = (float)progressBar.GetComponent<ProgressBar>().infectedCount / (float)progressBar.GetComponent<ProgressBar>().totalPeople;
                if ((popInfected * 100) < tierIdentifier.GetComponent<TierLabel>().infectionProb) {
                    certainInfected = true;
                }
            }

            if (!certainInfected)
            {
                var covidChance = Random.Range(0,100);
                if (covidChance <= covidProb) 
                {
                    toGenerate = aZombie;
                    infectedBool = true;
                } else {
                    toGenerate = aPerson;
                    infectedBool = false;
                }
            } else {
                toGenerate = aZombie;
                infectedBool = true;
                certainInfected = false;
            }

            // Check whether it's ok to spawn in
            Vector3 spawnPos = new Vector3(xPos, 1.45f, zPos);
            // float radius = 5f;

            safeSpawn = true;

            int currentAlive = progressBar.GetComponent<ProgressBar>().totalPeople;
            int aliveLimit = tierIdentifier.GetComponent<TierLabel>().popNum;

            if (policeStation.GetComponent<PoliceStation>().createSoldier)
            {
                toGenerate = aSoldier;
                xPos = policeStation.transform.position.x - 3;
                zPos = policeStation.transform.position.z - 8;
                infectedBool = false;
            }
            if (safeSpawn == true && ((currentAlive < aliveLimit) || policeStation.GetComponent<PoliceStation>().createSoldier))// && tierIdentifier.GetComponent<TierLabel>().endGame == false)
            {
                GameObject newPerson = Instantiate(toGenerate, new Vector3(xPos, 1.45f, zPos), Quaternion.identity);
                newPerson.transform.parent = terrainParent.transform;
                newPerson.AddComponent<PersonMovement>();
                newPerson.AddComponent<BoxCollider>();
                newPerson.AddComponent<PersonBehaviour>();
                newPerson.AddComponent<Rigidbody>();
                newPerson.AddComponent<MeshRenderer>();
                
                var personCollider = newPerson.transform.gameObject.GetComponent<BoxCollider>();

                GameObject station = GameObject.Find("Police Station");
                int infectionDistance = station.GetComponent<PoliceStation>().infectionDistance;
                if (!policeStation.GetComponent<PoliceStation>().createSoldier)
                {
                    personCollider.size = new Vector3(infectionDistance, 5, infectionDistance);
                } else {
                    personCollider.size = new Vector3(4, 5, 4);
                }
                personCollider.center = new Vector3(0, 2.5f, 0);
                if (infectedBool)
                {
                    progressBar.GetComponent<ProgressBar>().infectedCount += 1;
                }

                newPerson.GetComponent<PersonBehaviour>().person = newPerson;
                newPerson.GetComponent<PersonBehaviour>().covidBool = infectedBool;
                newPerson.GetComponent<PersonBehaviour>().aZombie = aZombie;
                newPerson.GetComponent<PersonBehaviour>().aHuman = aPerson;
                newPerson.GetComponent<PersonBehaviour>().anOfficer = anOfficer;
                newPerson.GetComponent<PersonMovement>().anOfficer = anOfficer;

                // Set animation state
                Animator animator = newPerson.GetComponent<Animator>();
                animator.SetBool("isMoving", false);

                // Freeze Orientation
                Rigidbody newPersonRB = newPerson.GetComponent<Rigidbody>();
                newPersonRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

                newPerson.name += "GenPerson" + GetInstanceID().ToString() + "-" + personID;
                personID += 1;

                // Update Progress Bar
                if (!policeStation.GetComponent<PoliceStation>().createSoldier)
                {
                    progressBar.GetComponent<ProgressBar>().totalPeople += 1;
                    if ((progressBar.GetComponent<ProgressBar>().totalPeople >= tierIdentifier.GetComponent<TierLabel>().randomProtest) && policeStation.GetComponent<PoliceStation>().protestBeen == false && cameraRig.GetComponent<CameraController>().isTracking == false)
                    {
                        policeStation.GetComponent<PoliceStation>().protestPresent = true;
                    } 
                } else {
                    policeStation.GetComponent<PoliceStation>().createSoldier = false;
                }             
            }

            triggerLostRespect = mainTerrain.GetComponent<PersonPlayerInteraction>().lostRespect;
            if (triggerLostRespect)
            {
                delayTime = 0;
                delayTwo = 0;
                tierIdentifier.GetComponent<TierLabel>().popNum = 20;
            } else {
                delayTime = Random.Range(1, 120);
                delayTwo = Random.Range(30, 100);
            }

            // Range determines speed of generation
            yield return new WaitForSeconds(delayTwo);

        }
    }
}
