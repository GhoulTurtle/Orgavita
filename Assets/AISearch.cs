using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISearch : MonoBehaviour{
    private Vector3 lastKnownTargetPosition;

    public void SetLastKnownTargetPosition(Vector3 position){
        lastKnownTargetPosition = position;
    }

    public void GetLastKnownTargetPosition(Vector3 position){

    }
}