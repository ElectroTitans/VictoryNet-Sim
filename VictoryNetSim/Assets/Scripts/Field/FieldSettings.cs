using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSettings : MonoBehaviour {

    public float FieldWidth = 10;
    public float FieldHeight = 20;

    public float ConvertToFieldX(float inX)
    {
        return inX / FieldWidth;
    }

    public float ConvertToFieldY(float inY)
    {
        return inY / FieldHeight;
    }


    public float ConvertToUnityX(float inX)
    {
        return inX * FieldWidth;
    }

    public float ConvertToUnityY(float inY)
    {
        return inY * FieldHeight;
    }

    // X -> X
    // Y -> none
    // Z -> Y
    public Vector2 ConvertToCoordFromUnity(Vector3 inPos)
    {
        return new Vector2(ConvertToFieldX(inPos.x), ConvertToFieldY(inPos.z));
    }

    public Vector3 ConvertToUnityFromCoord(Vector2 inPos, float y)
    {
        return new Vector3(ConvertToUnityX(inPos.x), y, ConvertToUnityY(inPos.y));
    }



}
