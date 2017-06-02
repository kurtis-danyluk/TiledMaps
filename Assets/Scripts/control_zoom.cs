using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control_zoom : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;
   

    private bool inputLock;
    private float lockTime = 1f;
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

    void Start()
    {
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
    void Update()
    {
        if (!inputLock)
            if (Controller.GetAxis() != Vector2.zero)
            {
                lockInput();
                float angle = Mathf.Atan2(Controller.GetAxis().y, Controller.GetAxis().x) * 180 / Mathf.PI;

                Debug.Log("Angle: " + angle);

                int zoom = collect_tiles.zoom; ;
                
                if (angle > 0)
                {
                    collector.watch_zoom(zoom + 1);
                    //Debug.Log("Zoom In to "+ collect_tiles.zoom);
                }
                else if (angle < 0)
                {
                    collector.watch_zoom(zoom - 1);
                    //Debug.Log("Zoom Out to " + collect_tiles.zoom);
                }
                

                // collector.latitude = Controller.GetAxis().y * lat_range;
                // collector.longitude = Controller.GetAxis().x * long_range;
            }
    }
}
