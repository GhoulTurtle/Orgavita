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
    [MinMaxRange(1f, 600f)] public RangedFloat idleTime;
    [MinMaxRange(1f, 600f)] public RangedFloat looseTime;
    [MinMaxRange(1f, 600f)] public RangedFloat searchTime;
    [MinMaxRange(1f, 600f)] public RangedFloat patrolTime;
}