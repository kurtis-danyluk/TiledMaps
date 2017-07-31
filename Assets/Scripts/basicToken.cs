using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicToken : MonoBehaviour {

    public TextMesh counter;
    public static int count;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public bool isGrabbed;
    public bool showBeacon;

    public coinBank bank;
    public float time_grabbed;
    public string coin_id;

    // Use this for initialization
    void Start () {

        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        count = 0;
        isGrabbed = false;
        showBeacon = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (showBeacon)
        {
            ShowLaser(this.transform.position, this.transform.position + new Vector3(0, 1000, 0), 1000);
        }

        if (isGrabbed)
        {
            bank.count++;
            time_grabbed = Time.time;
            this.gameObject.SetActive(false);
        }
            
	}

    private void ShowLaser(Vector3 startPoint, Vector3 hitPoint, float distance)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(startPoint, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            distance);
    }
}
