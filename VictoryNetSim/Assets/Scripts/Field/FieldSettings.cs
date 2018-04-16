using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSettings : MonoBehaviour {

    public float FieldWidth = 10;
    public float FieldLength = 20;

    public float ConvertToFieldX(float inX)
    {
        return inX / FieldWidth;
    }

    public float ConvertToFieldY(float inY)
    {
        return inY / FieldLength;
    }


    public float ConvertToCoordX(float inX)
    {
        return inX * FieldWidth;
    }

    public float ConvertToCoordY(float inY)
    {
        return inY * FieldLength;
    }


    
}
