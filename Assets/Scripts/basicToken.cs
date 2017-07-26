using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicToken : MonoBehaviour {

    public TextMesh counter;
    public static int count;

    public bool isGrabbed;

    public coinBank bank;
    public float time_grabbed;
    public int coin_id;

    // Use this for initialization
    void Start () {
        count = 0;
        isGrabbed = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (isGrabbed)
        {
            bank.count++;
            time_grabbed = Time.time;
            this.gameObject.SetActive(false);
        }
            
	}
}
