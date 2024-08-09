using UnityEngine;

public class AILineOfSight : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private AITargetDefinition aITargetDefinition;

    [Header("Debugging")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color detectionSphereLOSGizmosColor = Color.green;
    [SerializeField] private Color detectionAngleLOSGizmosColor = Color.red;

    private Collider[] targetsInView;

    private void Awake() {
        if(aICharacterDataSO != null){
            targetsInView = new Collider[aICharacterDataSO.maxTargets];
        }
    }

    public bool IsValidTargetInLOS(){
        return GetTargets(out Collider[] validTargets);
    }

    public IDamagable ChooseRandomDamagableTargetInLOS(){
        if (GetTargets(out Collider[] validTargets)){
            int initalRandomTargetIndex = Random.Range(0, validTargets.Length);

            for (int i = 0; i < validTargets.Length; i++){
                int currentIndex = (initalRandomTargetIndex + i) % validTargets.Length;   
                if(validTargets[currentIndex].TryGetComponent(out IDamagable damagable)){
                    return damagable;
                }
            }
        }

        return null;
    }

    public bool GetTargets(out Collider[] validTargets){
        int targetsFound = Physics.OverlapSphereNonAlloc(transform.position, aICharacterDataSO.visionRange, targetsInView, aITargetDefinition.GetTargetLayerMask());

        if(targetsFound == 0){
            validTargets = null;
            return false;
        }
        else{
            int maxTargetNumber = targetsFound < aICharacterDataSO.maxTargets ? targetsFound : aICharacterDataSO.maxTargets;

            validTargets = new Collider[maxTargetNumber];

            bool foundValidTarget = false;

            for(int i = 0; i < targetsFound; i++){
                Collider target = targetsInView[i];
                if(!IsTargetValid(target.transform)) continue;

                validTargets[i] = target;

                foundValidTarget = true;
            }

            return foundValidTarget;
        }
    }

    public bool IsTargetValid(Transform target, Vector3 position){
        Vector3 directionToTarget = (target.position - position).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) > aICharacterDataSO.visionAngle / 2) return false;

        float distanceToTarget = Vector3.Distance(position, target.position);

        if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, aITargetDefinition.GetObstructionLayerMask())) return false;

        return true;
    }

    public bool IsTargetValid(Transform target){
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) > aICharacterDataSO.visionAngle / 2) return false;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, aITargetDefinition.GetObstructionLayerMask())) return false;

        return true;
    }

    public Vector3 FindValidLOSPosition(Vector3 currentPosition, Transform target, AIMover aIMover){
        // Define the maximum search radius and the number of search angles
        float searchRadius = 5f;
        int numberOfRays = 36;
        
        // Calculate the angle increment based on the number of rays
        float angleIncrement = 360f / numberOfRays;

        // Loop through the potential positions around the current position
        for (int i = 0; i < numberOfRays; i++){
            // Calculate the direction of the ray based on the current angle
            float angle = i * angleIncrement;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
            // Calculate the potential new position
            Vector3 potentialPosition = currentPosition + direction * searchRadius;

            if(IsTargetValid(target, potentialPosition) && aIMover.CheckValidPosition(potentialPosition, out Vector3 finalPos)){
                return finalPos;
            }
        }

        return currentPosition;
    }

    private void OnDrawGizmosSelected(){
        if(drawGizmos){
            if(aICharacterDataSO == null) return;

            Gizmos.color = detectionSphereLOSGizmosColor;
            Gizmos.DrawWireSphere(transform.position, aICharacterDataSO.visionRange);

            Vector3 forward = transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, aICharacterDataSO.visionAngle / 2, 0) * forward * aICharacterDataSO.visionRange;
            Vector3 leftBoundary = Quaternion.Euler(0, -aICharacterDataSO.visionAngle / 2, 0) * forward * aICharacterDataSO.visionRange;

            Gizmos.color = detectionAngleLOSGizmosColor;
            Gizmos.DrawRay(transform.position, rightBoundary);
            Gizmos.DrawRay(transform.position, leftBoundary);
        }
    }
}