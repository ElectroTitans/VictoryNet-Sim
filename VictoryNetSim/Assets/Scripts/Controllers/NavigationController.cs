using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour {

    public Transform goal;
    private NavMeshAgent agent;
    // Use this for initialization
    void Start () {
        
        
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void GoToPoint (float fieldX, float fieldY)
    {
        float coordX = FindObjectOfType<FieldSettings>().ConvertToCoordX(fieldX);
        float coordY = FindObjectOfType<FieldSettings>().ConvertToFieldY(fieldY);
        agent.destination = new Vector3(coordX, coordY, transform.position.z);

        agent.stoppingDistance = 10;
        
    }
}
