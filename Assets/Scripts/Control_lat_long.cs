﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_lat_long : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private float lat_range = 85f;
    private float long_range = 179f;
    public collect_tiles collector;

    private bool inputLock;
    private float lockTime = 1f;
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
        inputLock = false;
	}

    void unlockInput()
    {
        inputLock = false;
    }

    void lockInput()
    {
        inputLock = true;
        Invoke("unlockInput", lockTime);
    }

	// Update is called once per frame
	void Update () {
        if(!inputLock)
		    if(Controller.GetAxis() != Vector2.zero)
            {
                lockInput();
                float angle = Mathf.Atan2(Controller.GetAxis().y, Controller.GetAxis().x) * 180 / Mathf.PI;

                Debug.Log("Angle: "+ angle);

                if (angle > 45 && angle <= 135)
                {
                    collector.latitude += 90 / (Mathf.Pow(2, collect_tiles.zoom));
                    Debug.Log("Up");
                }
                else if (angle > -135 && angle <= -45)
                {
                    collector.latitude -= 4 * (90 / (Mathf.Pow(2, collect_tiles.zoom)));
                    Debug.Log("Down");
                }
                else if (angle > 135 && angle <= 180 || angle > -180 && angle < -135)
                {
                    collector.longitude -= 180 / (Mathf.Pow(2, collect_tiles.zoom));
                    Debug.Log("Left");
                }
                else if (angle > 0 && angle <= 45 || angle > -45 && angle < 0)
                {
                    collector.longitude += 4 * (180 / (Mathf.Pow(2, collect_tiles.zoom)));
                    Debug.Log("Right");
                }

               // collector.latitude = Controller.GetAxis().y * lat_range;
               // collector.longitude = Controller.GetAxis().x * long_range;
            }
	}
}
