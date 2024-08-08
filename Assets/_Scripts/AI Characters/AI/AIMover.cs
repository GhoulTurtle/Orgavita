using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMover : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private NPCHealth nPCHealth;

    private void Awake() {
        if(aICharacterDataSO != null){
            navMeshAgent.speed = aICharacterDataSO.movementSpeed;
            navMeshAgent.angularSpeed = aICharacterDataSO.turningSpeed;
            navMeshAgent.acceleration = aICharacterDataSO.accelerationSpeed;
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

    public void SetDestination(Vector3 _goalPosition){
        if(nPCHealth.IsDead()) return;

		navMeshAgent.SetDestination(_goalPosition);
	}

    public bool CheckValidPosition(Vector3 position, out Vector3 validPosition, float rangeCheck = 1f){
        bool result = NavMesh.SamplePosition(position, out NavMeshHit hitInfo, rangeCheck, NavMesh.AllAreas);
        validPosition = hitInfo.position;

        //Need to update to check if this navmesh can reach that result position

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


        navMeshAgent.isStopped = state;
    }

	public void SetNavMeshUpdateRotation(bool state){
        if(navMeshAgent == null) return;

		navMeshAgent.updateRotation = state;
	}

    private void CharacterDeath(){
        if(navMeshAgent == null) return;

        navMeshAgent.enabled = false;
    }

    public void ApplyForce(Vector3 force){
        if(navMeshAgent == null) return;
        
        navMeshAgent.velocity = force;
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
}