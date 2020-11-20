using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float moveSpeed;
    public float moveTime;
    public float rotationAmount;

    public Vector3 newPos;
    public Quaternion newRotation;
    public Quaternion initialRotation;

    public GameObject trackingObj;
    public GameObject policeStation;
    
    public bool isTracking;

    public bool recentRevert;

    // Start is called before the first frame update
    void Start()
    {
        recentRevert = false;
        isTracking = false;
        newPos = transform.position;
        newRotation = transform.rotation;

        policeStation = GameObject.Find("Police Station");        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTracking)
        {
            if (recentRevert)
            {
                InvokeRepeating("newPause", 0.5f, 10f);
            } else {
                bool protestPresent = policeStation.GetComponent<PoliceStation>().protestPresent;
                if (!protestPresent)
                {
                    HandleInput();
                } else {
                    viewStation();
                }
            } 
        } else {
            // Track the Pollice Officer
            followPO();
        }
    }

    void newPause()
    {
        recentRevert = false;
        newPos = transform.position;
        CancelInvoke();
    }

    void viewStation()
    {
        Vector3 closeView = transform.position;
        int offset = 20;
        trackingObj = policeStation;
        
        closeView.y = 10;
        closeView.x = trackingObj.transform.position.x;
        closeView.z = trackingObj.transform.position.z - offset;
        transform.position = closeView;
        transform.rotation = initialRotation;
    }

    void followPO()
    {
        Vector3 closeView = transform.position;
        int offset = 10;
        
        closeView.y = 5;
        closeView.x = trackingObj.transform.position.x;
        closeView.z = trackingObj.transform.position.z - offset;
        transform.position = closeView;
        transform.rotation = initialRotation;
    }

    public bool validMove()
    {
        Vector3 testPos = newPos + (transform.forward * moveSpeed);
        if (testPos.x >= -5 && testPos.x <= 100 && testPos.z >= -5 && testPos.z <= 100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void fixPos()
    {
        if (newPos.x <= 0) { newPos.x = 0; }
        if (newPos.x >= 95) { newPos.x = 95; }
        if (newPos.z <= 0) { newPos.z = 0; }
        if (newPos.z >= 95) { newPos.z = 95; } 
    }

    void HandleInput()
    {
        // Keyboard Camera Movement
        if(Input.GetKey(KeyCode.W))
        {
            if (validMove())
            {
                newPos += (transform.forward * moveSpeed);
            }
            else 
            {
                fixPos();
            }
        }
        if(Input.GetKey(KeyCode.A))
        {
            if (validMove())
            {
                newPos += (transform.right * -moveSpeed);
            }
            else 
            {
                fixPos();
            }
        }
        if(Input.GetKey(KeyCode.S))
        {           
            if (validMove())
            {
                newPos += (transform.forward * -moveSpeed);
            }
            else 
            {
                fixPos();
            }
        }
        if(Input.GetKey(KeyCode.D))
        {
            if (validMove())
            {
                newPos += (transform.right * moveSpeed);
            }
            else 
            {
                fixPos();
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveTime);
        cameraRotation();
    }

    void cameraRotation() {
        // Keyboard Camera Rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * moveTime);
    }

}
