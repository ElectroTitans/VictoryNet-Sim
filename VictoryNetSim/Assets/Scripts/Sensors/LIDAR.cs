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


    public float MaxRange = 40;
    public float NoiseAmount = 0.1f;
    public int NumOfLines = 120;
    public float Angle = 220;

    private Transform emitPoint;
    private List<LineRenderer> debugLines = new List<LineRenderer>();

	// Use this for initialization
	void Start () {
        emitPoint = transform;

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
	}
    public void Update()
    {
        if (AutoUpdate)
        {
            GetPoints();
        }
    }
    public List<LidarPoint> GetPoints()
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
