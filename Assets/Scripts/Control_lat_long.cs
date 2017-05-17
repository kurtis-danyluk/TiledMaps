using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_lat_long : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private float lat_range = 89f;
    private float long_range = 179f;
    public collect_tiles collector;
    // Use this for initialization
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Controller.GetAxis() != Vector2.zero)
        {
            collector.latitude = Controller.GetAxis().y * lat_range;
            collector.longitude = Controller.GetAxis().x * long_range;
        }
	}
}
