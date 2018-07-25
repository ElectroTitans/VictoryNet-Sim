using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoringzone : MonoBehaviour {

    public bool scored;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "block")
        {
            scored = true;
        }
    }


}
