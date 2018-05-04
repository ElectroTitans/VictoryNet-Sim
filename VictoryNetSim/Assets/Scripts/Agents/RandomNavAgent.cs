using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RandomNavAgent : MonoBehaviour {
    public NavMeshAgent agent;
    public FieldSettings field;
    public Vector3 target;
    public GameObject targetMesh;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        field = FindObjectOfType<FieldSettings>();
        InvokeRepeating("NewTarget", 0, 5.0f);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
       
	}

    void NewTarget()
    {
        float fieldX = Random.Range(-0.95f, 0.95f);
        float fieldY = Random.Range(-0.95f, 0.95f);
        

        float randomX = field.ConvertToCoordX(fieldX);
        float randomY = field.ConvertToCoordY(fieldY);

        target = new Vector3(randomY, 0.1f, randomX);
        targetMesh.transform.parent = null;
        targetMesh.transform.position = target;
        agent.SetDestination(target);

        Debug.Log("Moving Agent (" + gameObject.name + ") to -> " + fieldX + "/" + fieldY);
    }
}
