using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class LIDARTrainingData : MonoBehaviour {


    public class EnvSettings
    {
        public int lineNum;
        public float noise;
        public float dropout;
        public float maxRange;
        public float spinRate;
        public bool instantMode;
        
    }

    public struct LidarTrainingEntry
    {
        public float X;
        public float Y;
        public float heading;

        public List<float> points;
    };

    public int DataLength = 64;

    public int FileID = 0;

    public float AttemptCount = 0;

    public int UpdateRate = 1;
    public float SaveEvery = 5000;
    public string BaseFileName = "LIDAR-Points#";

    public LIDAR LIDARToTrack;
    public EncoderSensor EncoderL;
    public EncoderSensor EncoderR;
    private List<LidarTrainingEntry> CurrentDataSet = new List<LidarTrainingEntry>();
    private FieldSettings field;

    private Vector3 lastCoord;
	// Use this for initialization
	void Start () {
        field = FindObjectOfType<FieldSettings>();
        DataLength = LIDARToTrack.NumOfLines +3 ; // Points + x/y + heading

        string JSONpath = Application.dataPath + "/LIDAR/_settings.json";

        EnvSettings settings = new EnvSettings();
        settings.dropout = LIDARToTrack.dropOutPercentage;
        settings.noise = LIDARToTrack.NoiseAmount;
        settings.lineNum = LIDARToTrack.NumOfLines;
        settings.maxRange = LIDARToTrack.MaxRange;
        settings.spinRate = LIDARToTrack.rotateRate;
        settings.instantMode = LIDARToTrack.AutoUpdate;
        string json = JsonUtility.ToJson(settings);

        StreamWriter jsonoutStream = System.IO.File.CreateText(JSONpath);
        jsonoutStream.Write(json);
        jsonoutStream.Close();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (AttemptCount < UpdateRate)
        {
            AttemptCount++;
        }
        else
        {
            AttemptCount = 0;

            Vector2 currentFieldPos = field.ConvertToCoordFromUnity(transform.position);
            float heading = transform.rotation.eulerAngles.y;
            heading /= 360;
            heading += Random.Range(-0.001f, 0.001f);
            LidarTrainingEntry entry;
            entry.X = currentFieldPos.x;
            entry.Y = currentFieldPos.y;
            entry.heading = heading;
            entry.points = LIDARToTrack.GetPointsRaw();
            AddData(entry);
            
        }
    }


    public void AddData(LidarTrainingEntry entry)
    {

        if(CurrentDataSet.Count < SaveEvery)
        {
            CurrentDataSet.Add(entry);
        }
        else
        {
            CurrentDataSet.Add(entry);
             
            ExportDataset();
          
        }
    }
   
    private void ExportDataset()
    {
        List<string[]> rowData = new List<string[]>();
        // Debug.Log("Export Lidar data: " + CurrentDataSet.Count);
       

       
        string[] rowDataTemp = new string[DataLength];

        rowDataTemp[0] = "X-Coord";
        rowDataTemp[1] = "Y-Coord";
        rowDataTemp[2] = "Heading";
        for(int i = 3; i < DataLength; i++)
        {
            rowDataTemp[i] = "L-" + (i - 3);
        }

        rowData.Add(rowDataTemp);


     
        for (int i = 0; i < CurrentDataSet.Count; i++)
        {
            rowDataTemp = new string[DataLength];
            rowDataTemp[0] = CurrentDataSet[i].X.ToString();
            rowDataTemp[1] = CurrentDataSet[i].Y.ToString();
            rowDataTemp[2] = CurrentDataSet[i].heading.ToString();
            for (int j = 3; j < DataLength; j++)
            {
                rowDataTemp[j] = CurrentDataSet[i].points[j - 3].ToString();
            }

            rowData.Add(rowDataTemp);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
        CurrentDataSet.Clear();

      
    }
    private string getPath()
    {
        FileID++;
        return Application.dataPath + "/LIDAR/" + BaseFileName + FileID;

    }
}
