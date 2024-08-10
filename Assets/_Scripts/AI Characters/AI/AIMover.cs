using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMover : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private NPCHealth nPCHealth;

    private IEnumerator rotationCoroutine;

    private void Awake() {
        if(aICharacterDataSO != null){
            navMeshAgent.speed = aICharacterDataSO.movementSpeed;
            navMeshAgent.angularSpeed = aICharacterDataSO.turningSpeed;
            navMeshAgent.acceleration = aICharacterDataSO.accelerationSpeed;
            SetNavMeshUpdateRotation(false);
        }

        if(nPCHealth != null){
            nPCHealth.OnCharacterDeath += CharacterDeath;
        }
    }

    private void OnDestroy() {
        if(nPCHealth != null){
            nPCHealth.OnCharacterDeath -= CharacterDeath;
        }

        StopAllCoroutines();
    }

    private void Update() {
        if(navMeshAgent == null || navMeshAgent.enabled == false || rotationCoroutine != null) return;

        Vector3 dir = navMeshAgent.velocity;
        if(dir == Vector3.zero) return;

        SetAIRotationVector(dir);
    }

    public void SetDestination(Vector3 _goalPosition){
        if(nPCHealth.IsDead()) return;

		navMeshAgent.SetDestination(_goalPosition);
	}

    public Vector3 GetAIRotationVector(){
        return transform.rotation.eulerAngles;
    }

    public void SetAIRotationVector(Vector3 dir){
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        Quaternion yRotation =  Quaternion.Slerp(transform.rotation, targetRotation, aICharacterDataSO.rotationSpeed * Time.deltaTime);
            
        transform.rotation = Quaternion.Euler(0, yRotation.eulerAngles.y, 0);
    }

    public void StartAIRotationJob(Vector3 dir){
        StopAIRotationJob();

        rotationCoroutine = RotationJobCoroutine(dir);
        StartCoroutine(rotationCoroutine);
    }

    public void StopAIRotationJob(){
        if(rotationCoroutine != null){
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }

    public bool CheckValidPosition(Vector3 position, out Vector3 validPosition, float rangeCheck = 1f){
        bool result = NavMesh.SamplePosition(position, out NavMeshHit hitInfo, rangeCheck, NavMesh.AllAreas);
        validPosition = hitInfo.position;
        
        if(!result) return result;

        // Check if the navmesh can reach the valid position
        NavMeshPath path = new NavMeshPath();
        result = NavMesh.CalculatePath(position, validPosition, NavMesh.AllAreas, path);
        if (result && path.status != NavMeshPathStatus.PathComplete){
            result = false; // The path is not complete, so it's not a valid position
        }

		return result;
	}

    public bool IsAgentAtMovementTarget(){
        if(navMeshAgent.enabled == false) return false;
        if(navMeshAgent.pathPending) return false;
        if(navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return false;
        if(navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude != 0f) return false;
        
        return true;
    }

    public void SetNavMeshIsStopped(bool state){
        if(navMeshAgent == null) return;
        if(!navMeshAgent.isOnNavMesh) return;
        if(navMeshAgent.isStopped == state) return;

        if(state && navMeshAgent.hasPath){
            navMeshAgent.ResetPath();
        }

        navMeshAgent.isStopped = state;
    }

	public void SetNavMeshUpdateRotation(bool state){
        if(navMeshAgent == null) return;

		navMeshAgent.updateRotation = state;
	}

    public void ApplyForce(Vector3 dir, float forceAmount, float forceSpeed){
        if(navMeshAgent == null || navMeshAgent.enabled == false) return;
        
        dir = dir.normalized;
        
        SetNavMeshAgentSpeed(forceSpeed, forceSpeed * 2);
        Vector3 newDestination = navMeshAgent.transform.position + dir * forceAmount;
        
        navMeshAgent.SetDestination(newDestination);
    }

    public void SetNavMeshAgentSpeed(float speed, float acceleration = -1f){
        navMeshAgent.speed = speed;

        if(acceleration == -1f) return;

        navMeshAgent.acceleration = acceleration;
    }

    public void ResetNavMeshAgentSpeed(){
        navMeshAgent.speed = aICharacterDataSO.movementSpeed;
        navMeshAgent.acceleration = aICharacterDataSO.accelerationSpeed;
    }

    private void CharacterDeath(){
        if(navMeshAgent == null) return;

        navMeshAgent.enabled = false;
    }

    private IEnumerator RotationJobCoroutine(Vector3 dir){
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        float timeElapsed = 0f;
        Quaternion startRotation = transform.rotation;

        while(timeElapsed < 1f){
            timeElapsed += aICharacterDataSO.rotationSpeed * Time.deltaTime;
            Quaternion yRotation =  Quaternion.Slerp(startRotation, targetRotation, timeElapsed);
            
            transform.rotation = Quaternion.Euler(0, yRotation.eulerAngles.y, 0);
            
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        rotationCoroutine = null;
    }
}