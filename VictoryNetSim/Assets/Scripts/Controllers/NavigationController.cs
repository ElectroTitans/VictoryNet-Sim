using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour {

    public Transform goal;
    public NavMeshAgent agent;
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        GoToPoint(-0.7f, 0.3f);


    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void GoToPoint (float fieldX, float fieldY)
    {
        float coordX = FindObjectOfType<FieldSettings>().ConvertToCoordX(fieldX);
        float coordY = FindObjectOfType<FieldSettings>().ConvertToFieldY(fieldY);
        Debug.Log("Moving to Field Coord: " + fieldX + "/" + fieldY + " -> " + coordX + "/" + coordY);
        agent.destination = new Vector3(coordY, transform.position.y, coordX);

       
        
    }
}
