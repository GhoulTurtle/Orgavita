using UnityEngine;

public class AISearch : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AIMover aIMover;

    [Header("Search Variables")]
    [SerializeField] private float searchRadius;
    [SerializeField] private AISearchType aISearchType;
    [SerializeField] private int maxSearchPointLoops = 20;

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

        Vector3 searchPosCenter = Vector3.Lerp(currentAIPos, currentTargetPos, currentSearchProgress);

        Vector3 finalPos = AIHelper.GetRandomCirclePosition(aIMover, searchPosCenter, searchRadius, maxSearchPointLoops);
        return finalPos;

    }   

    private Vector3 GetRandomSearchVector(){
        return AIHelper.GetRandomCirclePosition(aIMover, lastKnownTargetPosition, searchRadius, maxSearchPointLoops);
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