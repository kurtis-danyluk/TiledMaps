using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tracker_guide : MonoBehaviour {

    public bool isGrabbed;
    private SteamVR_TrackedObject trackedObj;
    public Generate_Terrain map;
    private GameObject tracker;
    private Transform trackerTransform;
    private MeshRenderer mesh;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public Transform cameraRigTransform;
    public Transform headTransform;

    void Start()
    {
        tracker = this.gameObject;
        trackerTransform = tracker.transform;
    //    Terr = Terrain.activeTerrains[1];
        isGrabbed = false;
        mesh = this.GetComponent<MeshRenderer>();

        laser = Instantiate(laserPrefab);
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
        laserTransform.position = Vector3.Lerp(trackerTransform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }


    // Update is called once per frame
    void Update()
    {


        if (!isGrabbed)
        {


            Vector3 trackPos = new Vector3();
            trackPos.x =  cameraRigTransform.position.x + Generate_Terrain.tile_width;
            trackPos.z = cameraRigTransform.position.z + Generate_Terrain.tile_height;
            trackPos = trackPos / map.map_width;
            trackPos.y = (cameraRigTransform.position.y / Generate_Terrain.tile_height);

            trackerTransform.localPosition = trackPos;
            
            RaycastHit hit;
            if (Physics.Raycast(trackerTransform.position, new Vector3(0, -1, 0), out hit, 1000))
            {
                    hitPoint = hit.point;
                    ShowLaser(hit);               
            }
            mesh.material.color = Color.red;
        }
        else
        {
            Vector3 temp = new Vector3();
            temp.x = (trackerTransform.localPosition.x * map.map_width ) - Generate_Terrain.tile_width;
            temp.z = (trackerTransform.localPosition.z * map.map_height ) - Generate_Terrain.tile_height;
            
            Vector3 difference = cameraRigTransform.position - headTransform.position;
            // Keep tracker on the ground
            //     difference.y = Terr.transform.position.y;

            //Let tracker roam
            temp.y = trackerTransform.localPosition.y * Generate_Terrain.tile_height;
            //difference.y = map.center.Terr.transform.position.y + map.center.Terr.terrainData.heightmapHeight + (trackerTransform.localPosition.y - 0.1f);

            
            cameraRigTransform.position = temp + difference;

            mesh.material.color = Color.green;
        }
        
    }
}
