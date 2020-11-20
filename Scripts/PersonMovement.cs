using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMovement : MonoBehaviour
{

    public float timer;
    public float delay;

    public GameObject anOfficer;
    public GameObject attachedOfficer;
    public GameObject policeStation;
    public GameObject tierIdentifier;
    public GameObject happinessBar;

    public bool newSpawn;
    // State of being escorted by officer
    public bool isEscorted;
    public bool attached;

    public float moveSpeed;
    public float officerSpeed;

    public Vector3 officerOffset;

    void Start()
    {
        delay = 2;
        timer = 0;
        newSpawn = false;
        attached = false;
        officerOffset = new Vector3 (1, 0, 1);

        happinessBar = GameObject.Find("Happiness Bar");

        policeStation = GameObject.Find("Police Station");

        //Get Covid Probability from Tier 1
        tierIdentifier = GameObject.Find("TierIdentifier");

        InvokeRepeating("ChangeDirection", 1.0f, 3.0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
 
        if (timer > delay) {
            if (isEscorted == true)
            {
                if (!attached)
                {
                     attachPO();
                }
                controlMov();
            }
            else
            {
                moveSpeed = tierIdentifier.GetComponent<TierLabel>().pMobility + (5 - happinessBar.GetComponent<HappinessBar>().happinessScore);
                randomMov();
            }
        }
    }

    void randomMov()
    {

        // Set animation state
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isMoving", true);

        newSpawn = true;

        transform.position += transform.forward * Time.deltaTime * moveSpeed;

    }

    void attachPO()
    {
        officerSpeed = policeStation.GetComponent<PoliceStation>().officerSpeed;
        Vector3 genPos = transform.position + officerOffset;

        attachedOfficer = Instantiate(anOfficer, genPos, Quaternion.identity);
        attached = true;
    }

    void controlMov()
    {
        Vector3 pManipulation = transform.position;
        var rManipulation = transform.eulerAngles;
        transform.position = transform.position;

        float transformAngle = rManipulation.y;

        bool dirChosen = false;

        if(Input.GetKey(KeyCode.W))
        {
            rManipulation.y = 0;
            dirChosen = true;
        }
        else if(Input.GetKey(KeyCode.A))
        {
            rManipulation.y = 270;
            dirChosen = true;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            rManipulation.y = 180;
            dirChosen = true;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            rManipulation.y = 90;
            dirChosen = true;
        } else {
            // Set animation state
            Animator animator = GetComponent<Animator>();
            animator.SetBool("isMoving", false);

            // Set animation state
            Animator animatorPO = attachedOfficer.GetComponent<Animator>();
            animatorPO.SetBool("isMoving", false);
        }
        if (dirChosen)
        {
            pManipulation += transform.forward * Time.deltaTime * officerSpeed;
            transform.position = pManipulation;
            transform.eulerAngles = rManipulation;

            attachedOfficer.transform.eulerAngles = transform.eulerAngles;

            attachedOfficer.transform.position = transform.position + officerOffset;

            // Set animation state
            Animator animator = GetComponent<Animator>();
            animator.SetBool("isMoving", true);

            // Set animation state
            Animator animatorPO = attachedOfficer.GetComponent<Animator>();
            animatorPO.SetBool("isMoving", true);
        }
    }
    
    void ChangeDirection()
    {
        var euler = transform.eulerAngles;
        euler.y = Random.Range(0.0f, 360.0f);
        transform.eulerAngles = euler;
    }

}
