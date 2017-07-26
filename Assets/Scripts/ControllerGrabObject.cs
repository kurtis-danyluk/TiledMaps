﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;
    public tracker_guide target;
    public Transform trackerTransform;
    private bool iGrabbed;
    public Generate_Terrain map;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        target = map.posTracker.GetComponent<tracker_guide>();
        trackerTransform = map.posTracker.transform;
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }
    // 1
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }
    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        if (objectInHand.GetComponent<tracker_guide>() != null)
        {
            target = objectInHand.GetComponent<tracker_guide>();
            target.isGrabbed = true;       
            iGrabbed = true;
        }

        if(objectInHand.GetComponent<basicToken>() != null)
        {
            objectInHand.GetComponent<basicToken>().isGrabbed = true;
        }
         
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
    private void ReleaseObject()
    {
        objectInHand = null;
        target.isGrabbed = false;
        iGrabbed = false;
    }
    // Update is called once per frame
    void Update () {

        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // 2
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
        if (target.isGrabbed && iGrabbed)
            trackerTransform.position = this.transform.position + this.transform.forward.normalized * 0.05f;
    }
}
