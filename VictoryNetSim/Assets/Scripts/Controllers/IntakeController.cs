using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntakeController : MonoBehaviour {
    public GameObject cube;
    public bool isHolding;
	// Use this for initialization
	void Start () {
        ResetIntake();

    }
	
	// Update is called once per frame
	void Update () {
        if (isHolding)
        {
            cube.transform.position = transform.position;

        }
	}

    public void ResetIntake()
    {
        cube.transform.position = transform.position;
        cube.transform.rotation = transform.rotation;
    }
    public void Drop()
    {
        cube.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,0));
        isHolding = false;
    }
}
