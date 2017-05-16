using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tracker_guide : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject tracker;
    private Transform trackerTransform;

    public Transform cameraRigTransform;

    void Start()
    {
        tracker = this.gameObject;
        trackerTransform = tracker.transform;
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //TO-DO: Change the 128 to be half the terrain size- which 128 currently is
        Vector3 trackPos = cameraRigTransform.position/256;
        trackPos.y = 0.1f;
        trackerTransform.localPosition = trackPos;
    }
}
