using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour {
    public float timeScale = 10;
	// Use this for initialization
	void Start () {
        Time.timeScale = timeScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
