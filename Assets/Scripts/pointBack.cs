using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointBack : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public Transform cameraRigTransform;
    private bool isActive = true;
    public coinBank bank;

    // Use this for initialization
    void Start()
    {
        laser = Instantiate(laserPrefab);
        laser.GetComponent<MeshRenderer>().material.color = Color.blue;
        laserTransform = laser.transform;

    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    // Update is called once per frame
    void Update()
    {

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            isActive = !isActive;
        }

        if (isActive && Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 1000))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                Vector3 truePath;
                if (bank.active_coin > 1)
                    truePath = bank.tokens[bank.active_coin - 2].transform.position - cameraRigTransform.position;
                else
                    truePath = new Vector3(0, 0, 0) - cameraRigTransform.position;

                Vector3 beamPath = hitPoint - cameraRigTransform.position;

                truePath.Normalize();
                beamPath.Normalize();

                float angle = Vector3.Angle(truePath, beamPath);
                if(bank.active_coin > 0)
                    bank.tokens[bank.active_coin - 1].GetComponent<basicToken>().point_back_angle = angle;

                //Debug.Log("Angle between paths: " + angle);
                
            }



        }
        else
            laser.SetActive(false);
    }
}
