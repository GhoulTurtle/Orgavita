using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Character Data/Basic Character Data", fileName = "NewAICharacterDataSO")]
public class AICharacterDataSO : ScriptableObject{
    [Header("Base Character Variables")]
    public float maxHealth;
    public float movementSpeed;
    public float attackDamage;
    public float attackRange;
    
    [Header("Base Vision Variables")]
    public float visionRange = 10f;
    public float visionAngle = 45f;
    public int maxTargets;

    [Header("Base Timer Variables")]
    [Tooltip("The amount of time that the AI will be idle.")]
    [MinMaxRange(1f, 600f)] public RangedFloat idleTimeInSeconds;
    [Tooltip("The amount of time that the AI will keep track of the player while the player isn't in the LOS while attacking.")]
    [MinMaxRange(1f, 600f)] public RangedFloat looseTimeInSeconds;
    [Tooltip("The total amount of time that the AI will search for a target in a area.")]
    [MinMaxRange(1f, 600f)] public RangedFloat searchTimeInSeconds;
    [Tooltip("The amount of time that the AI will stand at a search vector.")]
    [MinMaxRange(1f, 600f)] public RangedFloat searchIdleTimeInSeconds;
    [Tooltip("The total amount of time that the AI will patrol.")]
    [MinMaxRange(1f, 600f)] public RangedFloat patrolTimeInSeconds;
    [Tooltip("The amount of time that the AI will stand at a patrol vector.")]
    [MinMaxRange(1f, 600f)] public RangedFloat patrolIdleTimeInSeconds;
}