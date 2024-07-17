using UnityEngine;

[CreateAssetMenu(menuName = "Character Data/Base Character Data", fileName = "NewCharacterDataSO")]
public class CharacterDataSO : ScriptableObject{
    [Header("Base Enemy Data")]
    public float maxHealth;
    public float movementSpeed;
    public float enemyAttackDamage;
}
