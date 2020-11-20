using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehaviour : MonoBehaviour
{

    // Covid Boolean
    public bool covidBool = false;
    public GameObject person;
    public GameObject aHuman;
    public GameObject aZombie;
    public GameObject anOfficer;

    public Vector3 impactPos;

    private GameObject rig;
    public GameObject currencyText;
    public GameObject progressBar;
    public GameObject tierIdentifier;
    public GameObject station;

    public bool isEscorted;

    public bool changingCharacter;
    public bool infectingCharacter;

    // Start is called before the first frame update
    void Start()
    {
        rig = GameObject.Find("Camera Rig");
        progressBar = GameObject.Find("Progress Bar");
        currencyText = GameObject.Find("Currency");
        tierIdentifier = GameObject.Find("TierIdentifier");
        station = GameObject.Find("Police Station");
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        isEscorted = GetComponent<PersonMovement>().isEscorted;

        if (collision.gameObject.name.Contains("GenPerson"))
        {

            //Transmit Covid to the uninfected
            if ((covidBool == true) && (collision.gameObject.GetComponent<PersonBehaviour>().covidBool == false) && (collision.gameObject.name.Contains("Soldiers") == false))
            {
                changingCharacter = true;
                infectingCharacter = true;
            // return infected to normal
            } else if (gameObject.name.Contains("Soldiers") == true && collision.gameObject.GetComponent<PersonBehaviour>().covidBool == true){
                if (collision.gameObject.GetComponent<PersonMovement>().isEscorted) {
                    collision.gameObject.GetComponent<PersonMovement>().isEscorted = false;
                    Destroy(collision.gameObject.GetComponent<PersonMovement>().attachedOfficer);
                    rig.GetComponent<CameraController>().isTracking = false;
                    rig.GetComponent<CameraController>().trackingObj = null;
                    rig.GetComponent<CameraController>().recentRevert = true;
                    rig.transform.position = new Vector3 (50, 20, 0);
                }
                changingCharacter = true;
                infectingCharacter = false;
            }
        } 
        else if (collision.gameObject.name.Contains("House") && isEscorted == true) {
            progressBar.GetComponent<ProgressBar>().infectedCount -= 1;
            progressBar.GetComponent<ProgressBar>().totalPeople -= 1;
            Destroy(gameObject);
            Destroy(gameObject.GetComponent<PersonMovement>().attachedOfficer);

            rig.GetComponent<CameraController>().isTracking = false;
            rig.GetComponent<CameraController>().trackingObj = null;
            rig.GetComponent<CameraController>().recentRevert = true;
            rig.transform.position = new Vector3 (50, 20, 0);
            currencyText.GetComponent<currency>().wealth += 1;
            
            tierIdentifier.GetComponent<TierLabel>().currentScore += 1;
            if (tierIdentifier.GetComponent<TierLabel>().currentScore == tierIdentifier.GetComponent<TierLabel>().scoreTarget)
            {
                tierIdentifier.GetComponent<TierLabel>().increaseTier = true;
            }
            AudioSource houseAudio = collision.gameObject.GetComponent<AudioSource>();
            houseAudio.Play();
        }       
        else {
            var euler = transform.eulerAngles;
            euler.y += 90.0f;
            transform.eulerAngles = euler;
        }

        // replacing colliding person with either infected prefab or non-infected prefab
        if (changingCharacter)
        {
            impactPos = collision.gameObject.transform.position;

            Destroy (collision.gameObject);
            GameObject newPerson;

            if (infectingCharacter) {
                newPerson = Instantiate(aZombie, impactPos, Quaternion.identity); 
                progressBar.GetComponent<ProgressBar>().infectedCount += 1;
            } else {
                newPerson = Instantiate(aHuman, impactPos, Quaternion.identity);
                progressBar.GetComponent<ProgressBar>().infectedCount -= 1;
            }
            newPerson.transform.parent = person.gameObject.transform.parent;
            newPerson.AddComponent<PersonMovement>();
            newPerson.AddComponent<BoxCollider>();
            newPerson.AddComponent<PersonBehaviour>();
            newPerson.AddComponent<Rigidbody>();
            newPerson.AddComponent<MeshRenderer>();
            
            var infectedCollider = newPerson.transform.gameObject.GetComponent<BoxCollider>();
            int infectionDistance = station.GetComponent<PoliceStation>().infectionDistance;
            infectedCollider.size = new Vector3(infectionDistance, 5, infectionDistance);
            infectedCollider.center = new Vector3(0, 2.5f, 0);

            newPerson.GetComponent<PersonBehaviour>().person = newPerson;
            if (infectingCharacter) {
                newPerson.GetComponent<PersonBehaviour>().covidBool = true;
            } else {
                newPerson.GetComponent<PersonBehaviour>().covidBool = false;
            }
            newPerson.GetComponent<PersonBehaviour>().aZombie = aZombie;
            newPerson.GetComponent<PersonBehaviour>().anOfficer = anOfficer;
            newPerson.GetComponent<PersonMovement>().anOfficer = anOfficer;

            // Set animation state
            Animator animator = newPerson.GetComponent<Animator>();
            animator.SetBool("isMoving", false);

            // Freeze Orientation
            Rigidbody newPersonRB = newPerson.GetComponent<Rigidbody>();
            newPersonRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            newPerson.name += "GenPerson" + GetInstanceID().ToString();
        }

        changingCharacter = false;
        infectingCharacter = false;
    }
}
