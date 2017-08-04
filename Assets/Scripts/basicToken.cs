using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicToken : MonoBehaviour {

    public TextMesh counter;
    public static int count;
    public float offset;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public bool isGrabbed;
    public bool showBeacon;

    public coinBank bank;
    public float time_grabbed;
    public float time_entered;
    public bool beaconEntered = false;
    public string coin_id;
    public bool hasChanged = false;

    public Transform lightHouseTransform;

    // Use this for initialization
    void Start () {

        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        laser.name = "beacon" + coin_id;

        this.gameObject.GetComponent<Rigidbody>().maxDepenetrationVelocity = 0;

        count = 0;
        isGrabbed = false;
        showBeacon = true;

        if (showBeacon)
        {
            Vector2 disp = Random.insideUnitCircle;
            disp = new Vector2(disp.x * 25, disp.y * 25);
            Vector3 start = new Vector3(this.transform.position.x + disp.x, this.transform.position.y, this.transform.position.z + disp.y);
            Vector3 end = start + new Vector3(0, 1000, 0);
            ShowLaser(start, end, 1000);
        }

    }

    // Update is called once per frame
    void Update() {

        if (showBeacon && hasChanged)
        {
            Vector2 disp = Random.insideUnitCircle;
            disp = new Vector2(disp.x * 25, disp.y * 25);
            Vector3 start = new Vector3(this.transform.position.x + disp.x, this.transform.position.y, this.transform.position.z + disp.y);
            Vector3 end = start + new Vector3(0, 1000, 0);
            ShowLaser(start, end, 1000);
        }

        if (showBeacon && !beaconEntered) {
            Vector2 beaconHeart = new Vector2(laser.transform.position.x, laser.transform.position.z);
            Vector2 lhHeart = new Vector2(lightHouseTransform.position.x, lightHouseTransform.position.z);
            if (Vector2.Distance(beaconHeart, lhHeart) < (laser.transform.localScale.x)/2)
            {
                beaconEntered = true;
                time_entered = Time.time;
            }           
        }

        if (hasChanged)
        {
            moveOffGround();
            hasChanged = false;

        }


        if (isGrabbed)
        {
            bank.count++;
            time_grabbed = Time.time;
            this.gameObject.SetActive(false);
            laser.SetActive(false);
        }
            
	}

    private void ShowLaser(Vector3 startPoint, Vector3 hitPoint, float distance)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(startPoint, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.Rotate(new Vector3(90,0,0));
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, distance,
            laserTransform.localScale.z);
    }

    private void moveOffGround()
    {
        RaycastHit hit;
        string[] layers = { "Ignore Raycast" };
        int mask = LayerMask.GetMask(layers);
        mask = ~mask;
        Physics.Raycast(this.transform.position + new Vector3(0, 1000, 0), Vector3.down, out hit, 3000f, mask);

        this.transform.position = new Vector3(this.transform.position.x, hit.point.y + offset, this.transform.position.z);

    }
}
