﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    protected SteamVR_TrackedObject trackedObj;
    protected GameObject collidingObject;
    protected GameObject objectInHand;
    public tracker_guide target;
    public Transform trackerTransform;
    public bool iGrabbed;
    public Generate_Terrain map;

    Vector2 grabtimes;

    protected SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        target = map.posTracker.GetComponent<tracker_guide>();
        trackerTransform = map.posTracker.transform;
        grabtimes = new Vector2();
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    protected void SetCollidingObject(Collider col)
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
    protected void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;

        if (FunctionController.enableTokenMove)
        {
            if (objectInHand.GetComponent<tracker_guide>() != null)
            {
                target = objectInHand.GetComponent<tracker_guide>();
                target.isGrabbed = true;
                iGrabbed = true;
                grabtimes.x = Time.time;
            }
        }
        if(objectInHand.GetComponent<basicToken>() != null)
        {
            objectInHand.GetComponent<basicToken>().isGrabbed = true;
        }
         
    }

    // 3
    protected FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
    protected void ReleaseObject()
    {
        objectInHand = null;
        if (target.isGrabbed)
        {
            grabtimes.y = Time.time;
            Logger.grabs.Add(grabtimes);
            target.isGrabbed = false;
        }
        iGrabbed = false;
    }
    // Update is called once per frame
    public void Update () {

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
