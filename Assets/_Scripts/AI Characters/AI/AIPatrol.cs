using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPatrol : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AITargetDefinition aITargetDefinition;
    [SerializeField] private AIMover aIMover;
    [SerializeField] private AIPatrolType patrolType;
    [SerializeField] private List<Transform> patrolPositions;

    [Header("AI Patrol Variables")]
    [SerializeField] private float homePointWanderRange = 15f;
    [SerializeField] private int maxPointSearchLoops = 20;

    [Header("Debugging Variables")]
    [SerializeField] private bool showGizmos = false;
    [SerializeField] private Color gizmosColor = Color.cyan;

    private Vector3 currentGoalPoint;

    private void Awake() {
        if(patrolPositions.Count == 0){
            Debug.LogError("No patrol points set on AIPatrol, " + gameObject.name + "this is not allowed!");
            return;
        }
        switch (patrolType){
            case AIPatrolType.Homepoint: 
                if(!aIMover.CheckValidPosition( patrolPositions[0].position, out Vector3 validHomePosition)){
                    Debug.LogError( patrolPositions[0].gameObject.name + " is not close enough to a valid Navmesh position!");
                }
                currentGoalPoint = validHomePosition;
                break;
            case AIPatrolType.Patrolling: GenerateGoalPoint();
                break;
        }
    }

    public bool AtGoalPoint(float minRange){
        return Vector3.Distance(transform.position, currentGoalPoint) <= minRange;
    }

    public void SwitchPatrolType(AIPatrolType newPatrolType){
        if(newPatrolType == patrolType) return;

        patrolType = newPatrolType;
        GetNextGoalPoint();
    }

    public Vector3 GetCurrentGoalPoint(){
        return currentGoalPoint;
    }

    public void GenerateGoalPoint(){
        currentGoalPoint = GetNextGoalPoint();
    }

    private Vector3 GetNextGoalPoint(){
        return patrolType switch{
            AIPatrolType.Homepoint => GetHomepointGoalPoint(),
            AIPatrolType.Patrolling => GetPatrollingGoalPoint(),
            _ => currentGoalPoint,
        };
    }

    private Vector3 GetHomepointGoalPoint(){
        Vector2 randomCirclePos = Random.insideUnitCircle * homePointWanderRange;
        Vector3 randomPos = new(patrolPositions[0].position.x + randomCirclePos.x, patrolPositions[0].position.y, patrolPositions[0].position.z + randomCirclePos.y); 
        Ray randomPosGroundRay = new(){
            origin = randomPos,
            direction = Vector3.down
        };

        for (int i = 0; i < maxPointSearchLoops; i++){
            //Translate the Y level to ground level at that random pos
            if(Physics.Raycast(randomPosGroundRay, out RaycastHit hitInfo)){
                randomPos.y = hitInfo.point.y;
            }

            //Check if the random point is valid and is in LOS
            if(aIMover.CheckValidPosition(randomPos, out Vector3 validPosition)){
                if(PointInLOS(patrolPositions[0].position, validPosition)){
                    return validPosition;
                }
            }   
            
            //Select another random point
            randomCirclePos = Random.insideUnitCircle * homePointWanderRange;
            randomPos.Set(patrolPositions[0].position.x + randomCirclePos.x, patrolPositions[0].position.y, patrolPositions[0].position.z + randomCirclePos.y);
        }

        //Return the homepoint position if no valid point was found within the loop
        if(!aIMover.CheckValidPosition( patrolPositions[0].position, out Vector3 validHomePosition)){
            Debug.LogError( patrolPositions[0].gameObject.name + " is not close enough to a valid Navmesh position!");
        }
        return validHomePosition;
    }

    private Vector3 GetPatrollingGoalPoint(){
        //Find which patrol point index we are at
        int currentPatrolIndex = patrolPositions.FindIndex(p => Vector3.Distance(p.position, transform.position) <= 1.5f);

        //If we didn't find the patrol index then find the closest patrol point and set the current goal pos to it.
        if(currentPatrolIndex == -1){
            Transform closestPointTransform = null;
            float closestPoint = float.MaxValue;
            for (int i = 0; i < patrolPositions.Count; i++){
                float distance = Vector3.Distance(transform.position, patrolPositions[i].position);
                if(distance < closestPoint){
                    closestPointTransform = patrolPositions[i];
                    closestPoint = distance;
                }
            }
            
            if(!aIMover.CheckValidPosition(closestPointTransform.position, out Vector3 validPosition)){
                Debug.LogError(closestPointTransform.gameObject.name + " is not close enough to a valid Navmesh position!");
            }

            return validPosition;
        }

        //Get the next patrol index, and wrap around if the patrol index is the last on on the list
        int nextPatrolIndex = currentPatrolIndex + 1;

        if(nextPatrolIndex >= patrolPositions.Count){
            nextPatrolIndex = 0;
        }

        if(!aIMover.CheckValidPosition(patrolPositions[nextPatrolIndex].position, out Vector3 validIndexPosition)){
            Debug.LogError(patrolPositions[nextPatrolIndex].gameObject.name + " is not close enough to a valid Navmesh position!");
        }

        return validIndexPosition;
    }

    private bool PointInLOS(Vector3 checkOrigin, Vector3 point){
        Vector3 direction = (point - checkOrigin).normalized;
        float pointCheckDistance = Vector3.Distance(checkOrigin, point);
        
        Ray pointRay = new(checkOrigin, direction);

        Debug.DrawRay(pointRay.origin, direction * pointCheckDistance, Color.red, 3f);

        if(Physics.Raycast(pointRay, pointCheckDistance, aITargetDefinition.obstructionLayerMask)){
            return false;
        }

        return true;
    }

    private void OnDrawGizmos() {
        if(!showGizmos || currentGoalPoint == null) return;

        Gizmos.color = gizmosColor;
        if(AtGoalPoint(3f)){
            Gizmos.color = Color.green;
        }

        Gizmos.DrawLine(transform.position, currentGoalPoint);

        if(patrolPositions[0].position == null) return;

        if(patrolType != AIPatrolType.Homepoint) return;

        Gizmos.DrawWireSphere(patrolPositions[0].position, homePointWanderRange);
    }
}