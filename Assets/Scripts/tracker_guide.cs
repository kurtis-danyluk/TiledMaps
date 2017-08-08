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


    public static void translateMaptoMMap(out Vector3 localPos, Vector3 GlobalPose, Generate_Terrain map)
    {
        localPos = new Vector3();
        localPos.x = GlobalPose.x + Generate_Terrain.tile_width;
        localPos.z = GlobalPose.z + Generate_Terrain.tile_height;
        localPos = localPos / map.map_width;

        localPos.y = (GlobalPose.y / map.map_width);
       
    }


    // Update is called once per frame
    void LateUpdate()
    {
        RaycastHit hit;

        if (!isGrabbed)
        {
            

            Vector3 trackPos = new Vector3();
            trackPos.x =  cameraRigTransform.position.x + Generate_Terrain.tile_width;
            trackPos.z = cameraRigTransform.position.z + Generate_Terrain.tile_height;
            trackPos = trackPos / map.map_width;

            Physics.Raycast(cameraRigTransform.position, Vector3.down, out hit);

            //trackPos.y =0.05f + (hit.distance / Generate_Terrain.tile_height) + map.miniMap.GetComponent<Terrain>().terrainData.GetInterpolatedHeight(trackPos.x, trackPos.z);
            trackPos.y = 0.05f + (cameraRigTransform.position.y / map.map_width);
            trackerTransform.localPosition = trackPos;
            
            
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
            temp.y = ((trackerTransform.localPosition.y - 0.05f) * map.map_width);// - Generate_Terrain.tile_width;

            Vector3 difference = cameraRigTransform.position - headTransform.position;
            
            cameraRigTransform.position = temp + difference;

            mesh.material.color = Color.green;
        }
        
    }
}
