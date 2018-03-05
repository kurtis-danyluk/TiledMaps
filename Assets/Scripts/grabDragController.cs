using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class grabDragController : MonoBehaviour {

    public FunctionController funcController;
    public ThreeDConeDrag coneDrag;
    public ControllerGrabObject grabObject;
    public Transform contTransform;
    public GameObject grabIcon;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(funcController.miniMap.transform != null && FunctionController.enableMiniMap == true)
        {
            if (Vector3.Distance(contTransform.position, funcController.miniMap.transform.position) > 1)
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
