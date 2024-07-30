using UnityEngine;
using Random = UnityEngine.Random;

public class AISearch : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AIMover aIMover;

    [Header("Search Variables")]
    [SerializeField] private float searchRadius;
    [SerializeField] private AISearchType aISearchType;
    [SerializeField] private float maxSearchPointLoops = 20;

    [Header("Debugging")]
    [SerializeField] private bool showGizmos = false;
    [SerializeField] private Color gizmosColor = Color.white;

    private Transform currentTargetTransform;
    private Vector3 lastKnownTargetPosition;

    public void SetCurrentTargetTransform(Transform targetTransform){
        currentTargetTransform = targetTransform;
    }

    public Transform GetCurrentTargetTransform(){
        return currentTargetTransform;
    }

    public void SetLastKnownTargetPosition(Vector3 position){
        lastKnownTargetPosition = position;
    }

    public Vector3 GetLastKnownTargetPosition(){
        return lastKnownTargetPosition;
    }

    public Vector3 GetNextSearchVector(Vector3 currentAIPos, float currentSearchProgress){
        return aISearchType switch{
            AISearchType.Random => GetRandomSearchVector(),
            AISearchType.Funnel => GetFunnelSearchVector(currentAIPos, currentSearchProgress),
            _ => lastKnownTargetPosition,
        };
    }

    private Vector3 GetFunnelSearchVector(Vector3 currentAIPos, float currentSearchProgress){
        Vector3 currentTargetPos = currentTargetTransform.position;

        float distanceToTarget = Vector3.Distance(currentTargetPos, currentAIPos);

        //Target is within the search radius, narrow down the search area to be toward the target depending on the search time
        if(distanceToTarget <= searchRadius){
            Vector3 directionToTarget = (currentTargetPos - currentAIPos).normalized;
            float searchDistance = distanceToTarget * currentSearchProgress;
             //Check if the point is valid
            if(aIMover.CheckValidPosition(currentAIPos + directionToTarget * searchDistance, out Vector3 validPosition)){
                return validPosition;
            }
            return currentAIPos;
        }

        float angle = currentSearchProgress * 360f;
        float radius = currentSearchProgress * searchRadius;

        Vector3 offset = new(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        //Check if the point is valid
        if(aIMover.CheckValidPosition(lastKnownTargetPosition + offset, out Vector3 validCirclePosition)){
                return validCirclePosition;
        }
        return currentAIPos;
    }   

    private Vector3 GetRandomSearchVector(){
        Vector2 randomCirclePos = Random.insideUnitCircle * searchRadius;
        Vector3 randomPos = new(lastKnownTargetPosition.x + randomCirclePos.x, lastKnownTargetPosition.y, lastKnownTargetPosition.z + randomCirclePos.y); 
        Ray randomPosGroundRay = new(){
            origin = randomPos,
            direction = Vector3.down
        };

        for (int i = 0; i < maxSearchPointLoops; i++){
            //Translate the Y level to ground level at that random pos
            if(Physics.Raycast(randomPosGroundRay, out RaycastHit hitInfo)){
                randomPos.y = hitInfo.point.y;
            }

            //Check if the random point is valid
            if(aIMover.CheckValidPosition(randomPos, out Vector3 validPosition)){
                return validPosition;
            }   
            
            //Select another random point
            randomCirclePos = Random.insideUnitCircle * searchRadius;
            randomPos.Set(lastKnownTargetPosition.x + randomCirclePos.x, lastKnownTargetPosition.y, lastKnownTargetPosition.z + randomCirclePos.y);
        }

        //Return the last known position if no valid search point was found within the loop
        if(!aIMover.CheckValidPosition(lastKnownTargetPosition, out Vector3 validHomePosition)){
            Debug.LogError(lastKnownTargetPosition + " is not close enough to a valid Navmesh position!");
        }
        return validHomePosition;
    }

    private void OnDrawGizmos() {
        if(!showGizmos) return;
        if(lastKnownTargetPosition == Vector3.zero) return;

        Gizmos.color = gizmosColor;

        switch (aISearchType){
            case AISearchType.Random: 
                Gizmos.DrawWireSphere(lastKnownTargetPosition, searchRadius);
                break;
            case AISearchType.Funnel: DrawFunnelSearchGizmos();
                break;
        }
    }

    private void DrawFunnelSearchGizmos(){
        // Draw the search radius
        Gizmos.DrawWireSphere(lastKnownTargetPosition, searchRadius);

        // Draw the expanding circular search pattern
        Gizmos.color = Color.red;
        for (float angle = 0; angle < 360f; angle += 10f){
            float radians = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * searchRadius;
            Gizmos.DrawLine(lastKnownTargetPosition, lastKnownTargetPosition + offset);
        }
    }
}