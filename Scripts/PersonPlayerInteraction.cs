using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonPlayerInteraction : MonoBehaviour
{
    public GameObject progressBar;
    private bool mouseActive;
    private GameObject target;
    private GameObject rig;
    public GameObject happinessBar;
    public GameObject tierIdentifier;
    public GameObject policeStation;
    public bool lostRespect;
    public Vector3 screenSpace;
    public Vector3 offset;
    public Vector3 mousePos;

    void Start()
    {
        rig = GameObject.Find("Camera Rig");
        happinessBar = GameObject.Find("Happiness Bar");
        tierIdentifier = GameObject.Find("TierIdentifier");
        policeStation = GameObject.Find("Police Station");
        lostRespect = false;
    }

    void Update()
    {
        bool unavailable = rig.GetComponent<CameraController>().isTracking;

        if (Input.GetMouseButtonDown (0))
        {
            RaycastHit hitInfo;
            target = detectClickedObj (out hitInfo);
            var tName = target.name;
            if (target != null) 
            {
                if (tName.Contains("GenPerson") || tName.Contains("zombie"))
                {
                    bool protestPresent = policeStation.GetComponent<PoliceStation>().protestPresent;
                    if (tName.Contains("zombie") && unavailable == false)
                    {
                        if (!protestPresent)
                        {
                            target.GetComponent<PersonMovement>().isEscorted = true;
                            rig.GetComponent<CameraController>().trackingObj = target;
                            rig.GetComponent<CameraController>().isTracking = true;
                        }
                    }
                    else if(tName.Contains("zombie"))
                    {
                        Debug.Log("You can't escort us together!");
                    }
                    else 
                    {
                        if (tName.Contains("Soldiers") == false)
                        {
                            happinessBar.GetComponent<HappinessBar>().happinessScore -= 1;
                            if (happinessBar.GetComponent<HappinessBar>().happinessScore == 0)
                            {
                                lostRespect = true;
                                // Add loss of respect dialogue
                                tierIdentifier.GetComponent<TierLabel>().activateEndScene(false);
                            }
                        }
                    }
                }
            }
        }
    }

    GameObject detectClickedObj (out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        if (Physics.Raycast (ray.origin, ray.direction * 10, out hit)) 
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
 
    }
}
