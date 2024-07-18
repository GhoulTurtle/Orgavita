using UnityEngine;

[CreateAssetMenu(menuName = "AI Character Data/Base AI Character Data", fileName = "NewAICharacterDataSO")]
public class AICharacterDataSO : ScriptableObject{
    [Header("Base Enemy Data")]
    public float maxHealth;
    public float movementSpeed;
    public float attackDamage;
    public float attackRange;
    public float visionRange = 10f;
    public float visionAngle = 45f;
    public int maxTargets;
}
