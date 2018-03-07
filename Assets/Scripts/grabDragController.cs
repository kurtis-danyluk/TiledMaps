using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class grabDragController : MonoBehaviour {

    public FunctionController funcController;
    public ThreeDConeDrag coneDrag;
    public ControllerGrabObject grabObject;
    public Transform contTransform;
    public Transform trackTransform;
    public GameObject grabIcon;

	// Use this for initialization
	void Start () {
        if (!FunctionController.tokenMoveGEnabled)
        {
            grabIcon.SetActive(false);
        }
        trackTransform = funcController.miniMap.transform.FindChild("Tracker(Clone)").transform;


        if (!FunctionController.coneDragGEnabled || !FunctionController.tokenMoveGEnabled)
            this.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (!FunctionController.coneDragGEnabled || !FunctionController.tokenMoveGEnabled)
            return;


            if (!coneDrag.triggerDown && !grabObject.iGrabbed) {
            if (funcController.miniMap.transform != null && FunctionController.enableMiniMap == true)
            {
                //if (Vector3.Distance(contTransform.position, funcController.miniMap.transform.position + new Vector3(0.5f, 0, 0.5f)) > 0.55)
                if (Vector3.Distance(contTransform.position, trackTransform.position) > 0.2)
                {
                    funcController.toggleConeDrag(true);
                    funcController.toggleGrab(false);
                    grabIcon.SetActive(false);
                }
                else
                {
                    grabIcon.SetActive(true);
                    funcController.toggleConeDrag(false);
                    funcController.toggleGrab(true);
                }
            }
        }
        

	}
}
