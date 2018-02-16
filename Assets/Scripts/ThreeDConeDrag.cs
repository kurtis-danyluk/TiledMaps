using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDConeDrag : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private RaycastHit hit;
    private Vector3 grabForward;
    private Vector3 grabLoc;
    private Transform grabTrans;
    private Vector3 grabPath;
    private Vector3 feetPoint;
    private Vector3 grabRot;
    private float lastMovementRatio;
    private float lastRotAngle;
    private Transform localPlane;
    private GameObject planeObject;
    //public Terrain Terr;
    //public Generate_Terrain map;
    OneEuroFilter<Vector3> posFilter;
    float filterFrequency = 120f;



    public Transform cameraRigTransform;
    public Transform headTransform;

    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Vector3 teleportReticleOffset;
    private bool triggerDown;

    //private Vector2 teleTimeType;

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        planeObject = new GameObject();
        localPlane = planeObject.transform;

        lastMovementRatio = 0;

        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        posFilter = new OneEuroFilter<Vector3>(filterFrequency);

        //teleTimeType = new Vector2();
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    private void ShowLaser(Vector3 hitPoint, float distance)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, distance);
    }
    
    // Update is called once per frame


    void Update()
    {
        if (Controller.GetHairTriggerUp())
        {
            triggerDown = false;
            laser.SetActive(false);
            reticle.SetActive(false);
            posFilter = new OneEuroFilter<Vector3>(filterFrequency);
            lastMovementRatio = 0;
            lastRotAngle = 0;
            return;
        }
        if (Controller.GetHairTriggerDown() && !triggerDown)
        {

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 1000))
            {
                //grabTrans.position = trackedObj.transform.position;
                //grabTrans.rotation = trackedObj.transform.rotation;

                triggerDown = true;
                grabForward = trackedObj.transform.forward;
                grabLoc = new Vector3(headTransform.position.x, cameraRigTransform.position.y, headTransform.position.z);
                grabRot = trackedObj.transform.right;
                localPlane.rotation = trackedObj.transform.rotation;

                hitPoint = hit.point;

                grabPath = hitPoint - grabLoc;
                //feetPoint = new Vector3(headTransform.position.x, 0, headTransform.position.z);

            }
        }
        if (triggerDown)
        {
         
            float baseAngle = Vector3.Angle(grabForward, Vector3.up);
            Vector3 contForward = trackedObj.transform.forward;
            //contForward = posFilter.Filter<Vector3>(contForward);
            float contAngle = Vector3.Angle(/*trackedObj.transform.forward*/contForward, Vector3.up);       
            float difAngle = (baseAngle - contAngle) ;

            float movementRatio = 0;
            if(baseAngle != 0)
                movementRatio = (difAngle / baseAngle);

            if (movementRatio < 0)
                movementRatio = 0;

            if(movementRatio > 0.8)
            {
                movementRatio += (0.5f* (1 - movementRatio));
            }

            

            float posRatio = movementRatio - lastMovementRatio;
            if (posRatio > 0.1)
            {
                lastMovementRatio += 0.1f;
                posRatio = 0.1f;

            }
            else if (posRatio < -0.1)
            {
                lastMovementRatio -= 0.1f;
                posRatio = -0.1f;             
            }
            else
                lastMovementRatio = movementRatio;

            //Determine how far the transform should move
            float transDist = posRatio * grabPath.magnitude;

            //Direct that distance in the correct direction
            Vector3 movementPath = grabPath.normalized * transDist;
            
            //Determine the new location of the camera rig as moved by that path 
            Vector3 triPath = cameraRigTransform.position + movementPath;

            //Move the rig to its new position
            //cameraRigTransform.position = triPath;



            //Determine how much the controller has been rotated from its starting position

            contForward = trackedObj.transform.forward;

            contForward = posFilter.Filter<Vector3>(contForward);

            Vector3 pathToTarget =  trackedObj.transform.position - hitPoint ;

            float forAngle = Mathf.Atan(trackedObj.transform.forward.x / trackedObj.transform.forward.z);
            float pathAngle = Mathf.Atan(pathToTarget.x / pathToTarget.z);

            

            float difYAngle = Mathf.Rad2Deg * forAngle - Mathf.Rad2Deg*pathAngle ;//Vector2.Angle(new Vector2(trackedObj.transform.forward.x, trackedObj.transform.forward.z), new Vector2(pathToTarget.x, pathToTarget.z));

            Debug.Log(difYAngle);

            localPlane.transform.position = triPath;

            
            if(difYAngle > 1)
                localPlane.RotateAround(hitPoint, Vector3.up, 1);
            if(difYAngle < 1)
                localPlane.RotateAround(hitPoint, Vector3.up, -1);

            //Return the orginal local rotation
            //cameraRigTransform.eulerAngles = tempRot;

            cameraRigTransform.position = localPlane.position;

            //cameraRigTransform.position = posFilter.Filter<Vector3>(cameraRigTransform.position);
            

            //Show a laser between controller and grab point
            float distance = Vector3.Distance(trackedObj.transform.position, hitPoint);
            ShowLaser(hitPoint, distance);


        }
        else // 3
        {
        }
        
    }
}
