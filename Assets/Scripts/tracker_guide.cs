using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tracker_guide : MonoBehaviour {

    public bool isGrabbed;
    private SteamVR_TrackedObject trackedObj;
    public Terrain Terr;
    public Terrain mTerr;
    private GameObject tracker;
    private Transform trackerTransform;
    private MeshRenderer mesh;

    public Transform cameraRigTransform;
    public Transform headTransform;

    void Start()
    {
        tracker = this.gameObject;
        trackerTransform = tracker.transform;
    //    Terr = Terrain.activeTerrains[1];
        isGrabbed = false;
        mesh = this.GetComponent<MeshRenderer>();
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
        if (!isGrabbed)
        {
            Vector3 trackPos = cameraRigTransform.position / Terr.terrainData.heightmapWidth;
              trackPos.y = (cameraRigTransform.position/ Terr.terrainData.heightmapHeight).y;
            // trackPos.y = 0.1f;
            //Calculate based off the terrain height!
            trackPos.y +=0.6f;


            if (trackPos.x > 1)
                trackPos.x = 1;
            if (trackPos.x < -1)
                trackPos.x = -1;
            if (trackPos.y > 1)
                trackPos.y = 1;
            if (trackPos.y < -1)
                trackPos.y = -1;
            if (trackPos.z > 1)
                trackPos.z = 1;
            if (trackPos.z < -1)
                trackPos.z = -1;



            trackerTransform.localPosition = trackPos;

            mesh.material.color = Color.red;
        }
        else
        {
            Vector3 temp;
            temp = trackerTransform.localPosition * Terr.terrainData.heightmapWidth;
            temp.y = Terr.SampleHeight(temp);
            Vector3 difference = cameraRigTransform.position - headTransform.position;
            // Keep tracker on the ground
       //     difference.y = Terr.transform.position.y;

            //Let tracker roam0
            difference.y = Terr.transform.position.y + (trackerTransform.localPosition.y - 0.1f) * Terr.terrainData.heightmapHeight;

            // 5
            cameraRigTransform.position = temp + difference;

            mesh.material.color = Color.green;
        }
        
    }
}
