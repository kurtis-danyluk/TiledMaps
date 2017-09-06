using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionController : MonoBehaviour {

    public Generate_Terrain map;

    private static bool enableFlying;
    public GoogleStyleMovement flyingRight;

    private static bool enableToken;
    public static bool enableTokenMove;
    public tracker_guide movementToken;

    private bool enableGrab;
    public ControllerGrabObject controllerGrabLeft;
    public ControllerGrabObject controllerGrabRight;

    private static bool enableTeleport;
    public LaserPointer laserPointerRight;

    public static bool enableMiniMap;
    public GameObject miniMap;

    public static bool enableMap;
    public GameObject mainMap;

    private static bool enableTasks;
    public coinBank taskBank;

    private static bool enableNavigation;
    public Control_lat_long controlLatLonLeft;
    public control_zoom controlZoomRight;

    private static bool enablePointBack;
    public pointBack pointBackLeft;

    public bool boolMovement = false;
    public void toggleMovement(bool state)
    {
        enableFlying = state;
        enableTeleport = state;
        enableGrab = state;

        flyingRight.enabled = state;

        controllerGrabLeft.enabled = state;
        controllerGrabRight.enabled = state;

        laserPointerRight.enabled = state;

    }

    public bool boolGrab = false;
    public void toggleGrab(bool state)
    {
        enableGrab = state;
        controllerGrabLeft.enabled = state;
        controllerGrabRight.enabled = state;

    }


    public bool boolFlying = false;
    public void toggleFlying(bool state)
    {
        enableFlying = state;
        flyingRight.enabled = state;
    }

    public bool boolTokenMove = false;
    public void toggleTokenMove(bool state)
    {
        enableTokenMove = state;

    }

    public bool boolToken = false;
    public void toggleToken(bool state)
    {
        enableToken = state;
        movementToken.enabled = state;
    }


    public bool boolTeleportation = false;
    public void toggleTeleportation(bool state)
    {
        enableTeleport = state;
        laserPointerRight.enabled = state;
    }


    public bool boolNavigation = false;
    public void toggleNavigation(bool state)
    {
        enableNavigation = state;
        controlLatLonLeft.enabled = state;
        controlZoomRight.enabled = state;
    }


    public bool boolPointBack = false;
    public void togglePointBack(bool state)
    {
        enablePointBack = state;
        pointBackLeft.enabled = state;
    }


    public bool boolMap = false;
    public void toggleMap(bool state)
    {
        enableMap = state;
        mainMap.SetActive(state);
    }


    public bool boolMinimap = false;
    public void toggleMiniMap(bool state)
    {
        enableMiniMap = state;
        miniMap.SetActive(state);
    }

    void start()
    {
        mainMap = map.mainMap;
        miniMap = map.miniMap;
    }

    void Update()
    {
        if (boolFlying)
        {
            toggleFlying(!enableFlying);
            boolFlying = false;
        }
        if (boolGrab)
        {
            toggleGrab(!enableGrab);
            boolGrab = false;
        }
        if (boolTeleportation)
        {
            toggleTeleportation(!enableTeleport);
            boolTeleportation = false;
        }
        if(boolMovement)
        {
            toggleMovement(!(enableTeleport || enableGrab || enableFlying));
            boolMovement = false;
        }
        if (boolPointBack)
        {
            togglePointBack(!enablePointBack);
            boolPointBack = false;
        }
        if (boolNavigation)
        {
            toggleNavigation(!enableNavigation);
            boolNavigation = false;
        }
        if (boolToken)
        {
            toggleToken(!enableToken);
            boolToken = false;
        }
        if (boolTokenMove)
        {
            enableTokenMove = !enableTokenMove;
            boolTokenMove = false;
        }
        if (boolMinimap)
        {
            toggleMiniMap(!enableMiniMap);
            boolMinimap = false;
        }
        if (boolMap)
        {
            toggleMap(!enableMap);
            boolMap = false;
        }

    }

}
