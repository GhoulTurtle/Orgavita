using UnityEngine;

public class AILineOfSight : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private AITargetDefinition aITargetDefinition;

    [Header("Debugging")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color detectionSphereLOSGizmosColor = Color.green;
    [SerializeField] private Color detectionAngleLOSGizmosColor = Color.red;

    private Transform currentTarget;
    private Collider[] targetsInView;

    private void Awake() {
        if(aICharacterDataSO != null){
            targetsInView = new Collider[aICharacterDataSO.maxTargets];
        }
    }

    public Transform GetCurrentTarget(){
        if(currentTarget == null){
            return null;
        }

        return currentTarget;
    }

    public bool IsCurrentTargetInLOS(){
        if(currentTarget == null) return false;
        return IsTargetValid(currentTarget);
    }

    public void ChooseRandomTarget(){
        if (GetTargets(out Collider[] validTargets)){
            int randomTargetIndex = Random.Range(0, validTargets.Length);
            currentTarget = validTargets[randomTargetIndex].transform;
        }
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

    private bool IsTargetValid(Transform target){
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) > aICharacterDataSO.visionAngle / 2) return false;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, aITargetDefinition.GetObstructionLayerMask())) return false;

        return true;
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
