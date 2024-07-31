using UnityEngine;

public static class AIHelper{
    public static Vector3 GetRandomCirclePosition(AIMover aIMover, Vector3 centerOfCircle, float radius, int maxSearchLoops){
        Vector2 randomCirclePos = Random.insideUnitCircle * radius;
        Vector3 randomPos = new(centerOfCircle.x + randomCirclePos.x, centerOfCircle.y, centerOfCircle.z + randomCirclePos.y); 
        Ray randomPosGroundRay = new(){
            origin = randomPos,
            direction = Vector3.down
        };

        for (int i = 0; i < maxSearchLoops; i++){
            //Translate the Y level to ground level at that random pos
            if(Physics.Raycast(randomPosGroundRay, out RaycastHit hitInfo)){
                randomPos.y = hitInfo.point.y;
            }

            //Check if the random point is valid
            if(aIMover.CheckValidPosition(randomPos, out Vector3 validPosition)){
                return validPosition;
            }   
            
            //Select another random point
            randomCirclePos = Random.insideUnitCircle * radius;
            randomPos.Set(centerOfCircle.x + randomCirclePos.x, centerOfCircle.y, centerOfCircle.z + randomCirclePos.y);
        }

        //Return the last known position if no valid point was found within the loop
        if(!aIMover.CheckValidPosition(centerOfCircle, out Vector3 validDefaultPosition)){
            Debug.LogError(centerOfCircle + " is not close enough to a valid Navmesh position!");
        }
        return validDefaultPosition;
    }
}
