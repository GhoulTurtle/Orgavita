using System;
using UnityEngine;

[Serializable]
public class ObjectData{
    public string objectID = "";
    public string objectCustomState;
    public bool objectState;
    public int objectStateInt;
    public float objectStateFloat;
    public bool isEnabled;
    public Vector3 objectPos;
    public Quaternion objectRotation;

    public string GetObjectID(){
        if(objectID == ""){
            GenerateNewObjectGUID();
        }

        return objectID;
    }

    public string GenerateNewObjectGUID(){
        objectID = Guid.NewGuid().ToString();
        return objectID;
    }
}
