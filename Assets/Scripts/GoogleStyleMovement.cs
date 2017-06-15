using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleStyleMovement : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;


    public Transform cameraRigTransform;
    public Transform headTransform;

    // Use this for initialization
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    // Use this for initialization
    void Start () {
        		
	}

    // Update is called once per frame
    void Update()
    {

        if (Controller.GetAxis() != Vector2.zero)
        {
            float angle = Mathf.Atan2(Controller.GetAxis().y, Controller.GetAxis().x) * 180 / Mathf.PI;

            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //Vector3 difference = cameraRigTransform.position - headTransform.position;
                
                //difference.y = 0;
                
                if (angle > 0)
                {
                    cameraRigTransform.Translate(headTransform.forward);
                }
                else if (angle < 0)
                {
                    cameraRigTransform.Translate(-headTransform.forward);
                }
            }



        }
    }
}
