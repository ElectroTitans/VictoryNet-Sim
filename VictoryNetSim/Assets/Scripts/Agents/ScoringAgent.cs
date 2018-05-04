using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringAgent : Agent {

    public int[] fmsData = new int[3]; // Take the L aka 0
    public int target;

    public Transform switchClosePos;
    public Transform scalePos;
    public Transform switchFarPos;

    private NavigationController navController;
    private IntakeController intakeController;
    private LiftController liftController;
	// Use this for initialization
	void Start () {
       
    }

    public override void AgentReset()
    {
        List<Spawnpoint> spawnPoints = new List<Spawnpoint>();
        spawnPoints.AddRange(FindObjectsOfType<Spawnpoint>());
        Spawnpoint spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];

        transform.position = spawn.transform.position;

        for (int i = 0; i < 3; i++)
        {
            fmsData[i] = Random.Range(0, 2);
        }


        target = Random.Range(-1, 1);

    }
    public override void CollectObservations()
    {
        float heading = transform.rotation.eulerAngles.y;
        heading /= 360;

        AddVectorObs(target);
        AddVectorObs(fmsData[target + 1]);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Transform targetPos = switchClosePos;

        switch (target)
        {
            case -1:
                targetPos = switchClosePos;
                break;

            case 0:
                targetPos = scalePos;
                break;

            case 1:
                targetPos = switchFarPos;
                break;
        }

        float distanceToTarget = navController.agent.remainingDistance;
        if (distanceToTarget < 0.2)
        {
            AddReward(0.8f);
            Done();
        }

        //Time Punishment
        AddReward(-0.03f);

        // Action
        // Sections
       // float xnav   = Mathf.Clamp(vectorAction[0], -1, 1);
        //float ynav   = Mathf.Clamp(vectorAction[1], -1, 1);
        //float intake = Mathf.Clamp(vectorAction[2], 0, 1);
        //float lift   = Mathf.Clamp(vectorAction[3], 0, 1);
        FindObjectOfType<DiscordStatus>().SubmitStep(GetCumulativeReward());
        //navController.GoToPoint(xnav, ynav);
        
    }


}
