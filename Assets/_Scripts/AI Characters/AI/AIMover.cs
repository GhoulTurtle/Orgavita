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
        }

        if(nPCHealth != null){
            nPCHealth.OnCharacterDeath += CharacterDeath;
        }
    }

    private void OnDestroy() {
        if(nPCHealth != null){
            nPCHealth.OnCharacterDeath -= CharacterDeath;
        }
    }

    public void SetDestination(Vector3 _goalPosition){
        if(nPCHealth.IsDead()) return;

		navMeshAgent.SetDestination(_goalPosition);
	}

    public bool CheckValidPosition(Vector3 position, out Vector3 validPosition, float rangeCheck = 1f){
        bool result = NavMesh.SamplePosition(position, out NavMeshHit hitInfo, rangeCheck, NavMesh.AllAreas);
        validPosition = hitInfo.position;

		return result;
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
}