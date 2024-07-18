using UnityEngine;

public class AIStateMachine : StateMachine<AIStateType>{
    [Header("Required References")]
    [SerializeField] private AIStateDataList aIStateDataList;
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private AILineOfSight aILineOfSight;
    [SerializeField] private AIMover aIMover;
    [SerializeField] private NPCHealth nPCHealth;

    [Header("AI Debugging")]
    [SerializeField] private bool showGizmos;
    [SerializeField] private Color attackRangeGizmosColor = Color.red;

    private void Awake() {
        if(aIStateDataList != null){
            aIStateDataList.GetAIStateDictionary(States);
        }
    }

    public AILineOfSight GetAILineOfSight(){
        if(aIStateDataList == null) return null;
        return aILineOfSight;
    }

    public AIMover GetAIMover(){
        if(aIMover == null) return null;
        return aIMover;
    }

    public NPCHealth GetNPCHealth(){
        if(nPCHealth == null) return null;
        return nPCHealth;
    }

    private void OnDrawGizmosSelected() {
        if(!showGizmos || aICharacterDataSO == null) return;

        Gizmos.color = attackRangeGizmosColor;

        Gizmos.DrawWireSphere(transform.position, aICharacterDataSO.attackRange);
    }
}