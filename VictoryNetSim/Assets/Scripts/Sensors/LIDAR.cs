using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIDAR : MonoBehaviour {
    public struct LidarPoint
    {
        public float index;
        public float angle;
        public float distance;
    }

    public bool DebugLines;
    public bool AutoUpdate;
    public Material lineMaterial;
    public Material lineMaterialActive;
    public Material lineMaterialDead;
    public float MaxRange = 40;
    public float NoiseAmount = 0f;
    public int NumOfLines = 120;
    public float Angle = 220;
    public float rotateRate = 1;
    public float dropOutPercentage = 0.05f;
    public float positionError = 0.01f;
  

    private Transform emitPoint;
    private List<LineRenderer> debugLines = new List<LineRenderer>();
    private List<float> currentLidar = new List<float>();
    private LIDARTrainingData trainingData;
	// Use this for initialization
	void Start () {
        emitPoint = transform;
        trainingData = FindObjectOfType<LIDARTrainingData>();
       
        if (DebugLines)
        {
            for (int i = 0; i < NumOfLines; i++)
            {
                LineRenderer line = new GameObject("Line " + i.ToString()).AddComponent<LineRenderer>();
                line.transform.parent = transform;
                line.material = lineMaterial;
                line.SetWidth(0.01F, 0.01F);
                line.startColor = Color.red;
                line.endColor = Color.red;
                line.SetVertexCount(2);
                debugLines.Add(line);
            }
        }

        

        for(int i = 0; i < NumOfLines; i++)
        {
            currentLidar.Add(0);
        }
        if (AutoUpdate)
        {
            StartCoroutine(LidarTick());
        }
    }
    public void Update()
    {
        
    }

    private int index = 0;
    private IEnumerator LidarTick()
    {
        while (true)
        {
            currentLidar.Clear();
            float deegresPerLine = Angle / NumOfLines;

            for (int i = 0; i < NumOfLines; i++)
            {   

                float angle = (i * deegresPerLine) - (Angle / 2);
                RaycastHit currentHit;
                // Lidar.rotation = Quaternion.Euler(Lidar.rotation.eulerAngles.x, Lidar.rotation.eulerAngles.y, i * 2);
                if (Physics.Raycast(emitPoint.position, Quaternion.Euler(0, angle, 0) * emitPoint.right, out currentHit))
                {
                    Debug.Log("currentHit.distance " + currentHit.distance);
                    if(currentHit.distance > MaxRange)
                    {
                        if (DebugLines)
                        {
                            debugLines[i].enabled = false;
                        }

                        currentHit.distance = 0;

                        Debug.Log("maxrange");
                    }

                    if (DebugLines)
                    {
                        Debug.DrawLine(emitPoint.position, currentHit.point);
                        debugLines[i].enabled = true;
                        debugLines[i].SetPosition(0, emitPoint.position);
                        debugLines[i].SetPosition(1, currentHit.point);
                        debugLines[i].material = lineMaterial;
                    }

                }
                else
                {
                    if (DebugLines)
                    {
                        debugLines[i].enabled = false;
                    }

                }


                float distance = Mathf.Clamp(currentHit.distance / MaxRange, 0, 1) + Random.Range(-NoiseAmount / 2, NoiseAmount / 2);
                float odds = (100f / (dropOutPercentage * 100f));
                
                if (Random.Range(0, (int)odds) ==0){
                    distance = 0;
                    Debug.Log("drop out");
                    if (DebugLines)
                    {
                        debugLines[i].material = lineMaterialDead;
                    }
                }
                   
                currentLidar.Add(distance);
            }
            yield return new WaitForSeconds(1.0f/rotateRate);
        }
    }

    /*
      public List<LidarPoint> GetPoints()
    {
        if (timeLoopMode)
        {
            return null;
        }
        else
        {
            List<LidarPoint> points = new List<LidarPoint>();


            float deegresPerLine = Angle / NumOfLines;

            for (int i = 0; i < NumOfLines; i++)
            {

                float angle = (i * deegresPerLine) - (Angle / 2);
                RaycastHit currentHit;
                // Lidar.rotation = Quaternion.Euler(Lidar.rotation.eulerAngles.x, Lidar.rotation.eulerAngles.y, i * 2);
                if (Physics.Raycast(emitPoint.position, Quaternion.Euler(0, angle, 0) * emitPoint.right, out currentHit))
                {
                    if (DebugLines)
                    {
                        Debug.DrawLine(emitPoint.position, currentHit.point);
                        debugLines[i].enabled = true;
                        debugLines[i].SetPosition(0, emitPoint.position);
                        debugLines[i].SetPosition(1, currentHit.point);
                    }

                }
                else
                {
                    if (DebugLines)
                    {
                        debugLines[i].enabled = false;
                    }

                }


                float distance = Mathf.Clamp(currentHit.distance / MaxRange, 0, 1) + Random.Range(-NoiseAmount / 2, NoiseAmount / 2);

                LidarPoint newPoint = new LidarPoint();
                newPoint.index = i;
                newPoint.angle = angle;
                newPoint.distance = distance;
                points.Add(newPoint);


            }

            return points;
        }
       
    }
    */

    public List<float> GetPointsRaw()
    {
        if (AutoUpdate)
        {
            return currentLidar;
        }
        else
        {
            List<float> points = new List<float>();


            float deegresPerLine = Angle / NumOfLines;

            for (int i = 0; i < NumOfLines; i++)
            {
                float angle = (i * deegresPerLine) - (Angle / 2);
                RaycastHit currentHit;
                // Lidar.rotation = Quaternion.Euler(Lidar.rotation.eulerAngles.x, Lidar.rotation.eulerAngles.y, i * 2);
                if (Physics.Raycast(emitPoint.position, Quaternion.Euler(0, angle, 0) * emitPoint.right, out currentHit))
                {
                    if (DebugLines)
                    {
                        Debug.DrawLine(emitPoint.position, currentHit.point);
                        debugLines[i].enabled = true;
                        debugLines[i].SetPosition(0, emitPoint.position);
                        debugLines[i].SetPosition(1, currentHit.point);
                        debugLines[i].material = lineMaterial;
                    }
                    if (currentHit.distance > MaxRange)
                    {
                        if (DebugLines)
                        {
                            debugLines[i].enabled = false;
                        }

                        currentHit.distance = 0;

                        
                    }
                }
                else
                {
                    if (DebugLines)
                    {
                        debugLines[i].enabled = false;
                    }

                }


                float distance = Mathf.Clamp(currentHit.distance / MaxRange, 0, 1) + Random.Range(-NoiseAmount / 2, NoiseAmount / 2);
                float odds = (100f / (dropOutPercentage * 100f));

                if (Random.Range(0, (int)odds) == 0)
                {
                    distance = 0;
  
                    if (DebugLines)
                    {
                        debugLines[i].material = lineMaterialDead;
                    }
                }
                points.Add(distance);
            }

            return points;
        }
    }
}
