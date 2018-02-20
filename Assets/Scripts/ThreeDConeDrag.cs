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
    OneEuroFilter angleFilter;
    float filterFrequency = 60f;



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
        angleFilter = new OneEuroFilter(filterFrequency);
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
            angleFilter = new OneEuroFilter(filterFrequency);
            
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
         
            //Part 1: Setup needed constants
            float baseAngle = Vector3.Angle(grabForward, Vector3.up);
            Vector3 contForward = trackedObj.transform.forward;

            float contAngle = Vector3.Angle(contForward, Vector3.up);       
            float difAngle = (baseAngle - contAngle) ;

            float movementRatio = 0;
            //Prevent divide by 0 case
            if(baseAngle != 0)
                movementRatio = -(difAngle / baseAngle);

            //If we should move backwards- stop instead
            //if (movementRatio < 0)
            //    movementRatio = 0;
            
            

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
            Vector3 pathToTarget =  (trackedObj.transform.position - hitPoint).normalized ;
            contForward = trackedObj.transform.forward.normalized;

            Vector2 cont2DForward = new Vector2(contForward.x, contForward.z);
            Vector2 pathToTarget2D = new Vector2(pathToTarget.x, pathToTarget.z); 

            /*
            if (Vector2.Dot(new Vector2(contForward.z, contForward.x), new Vector2(pathToTarget.z, pathToTarget.x)) > 0)
            {
                Debug.Log("I'm backwards!");    
            }
            else
            {
                Debug.Log("I'm proper");
            }
            */
            //3D math
            //float forAngle = Mathf.Asin(contForward.z / contForward.x);
            //float pathAngle = Mathf.Atan(pathToTarget.z / pathToTarget.x);
            //2D Math
            float forAngle = Mathf.Asin(cont2DForward.x / cont2DForward.magnitude);
            float pathAngle = Mathf.Asin(pathToTarget2D.x / pathToTarget2D.magnitude);
            
            float difYAngle =   (Mathf.Rad2Deg*pathAngle) - (Mathf.Rad2Deg * forAngle);//Vector2.Angle(new Vector2(trackedObj.transform.forward.x, trackedObj.transform.forward.z), new Vector2(pathToTarget.x, pathToTarget.z));
            

            


            localPlane.transform.position = triPath;
            localPlane.RotateAround(hitPoint, Vector3.up, difYAngle);
           
            //Return the orginal local rotation
            cameraRigTransform.position = localPlane.position;

           
            //Show a laser between controller and grab point
            float distance = Vector3.Distance(trackedObj.transform.position, hitPoint);
            ShowLaser(hitPoint, distance);


        }
        else // 3
        {
        }
        
    }
}
