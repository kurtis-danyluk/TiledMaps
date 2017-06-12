﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public Terrain Terr;
    public Generate_Terrain map;

    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    private bool shouldTeleport;

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
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
    private void Teleport()
    {
        // 1
        shouldTeleport = false;
        // 2
        reticle.SetActive(false);
        // 3
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // 4
        difference.y = 0;
        // 5
        cameraRigTransform.position = hitPoint + difference;
    }
    // Update is called once per frame
    void Update () {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            RaycastHit hit;

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 1000))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                reticle.SetActive(true);
                if (hit.collider.gameObject.name == "Terrain")
                {
                    //  Debug.Log("Hit main map");
                }
                else if (hit.collider.gameObject.name == "miniMap")
                {
                    
                    hitPoint = hit.collider.gameObject.transform.InverseTransformPoint(hitPoint);
                    hitPoint = (hitPoint * map.map_width);
                    hitPoint.x = hitPoint.x - (Generate_Terrain.tile_width);
                    hitPoint.z = hitPoint.z  - (Generate_Terrain.tile_height);
                    hitPoint.y = Terr.SampleHeight(hitPoint);
                }
                //Debug.Log(hitPoint.x + " " + hitPoint.y + " " + hitPoint.z);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                shouldTeleport = true;

            }
        }
        else // 3
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip) && shouldTeleport)
        {
            Teleport();
        }
    }
}
