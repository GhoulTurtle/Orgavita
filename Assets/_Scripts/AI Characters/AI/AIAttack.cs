using System.Collections.Generic;
using UnityEngine;

public class AIAttack : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AIMover aIMover;
    [SerializeField] private AILineOfSight aILineOfSight;
    [SerializeField] private AIAttackDodgeType aIAttackDodgeType;
    [SerializeField] private List<BaseAIAttackDefinitionSO> aIAttackDefinitionList = new();
    [SerializeField] private int maxDodgePointSearchLoops = 20;

    private IDamagable currentTarget;
    private Transform currentTargetTransform;

    private bool isAttacking = false;

    public void SetCurrentDamagableTarget(IDamagable target){
        currentTarget = target;
        currentTargetTransform = currentTarget.GetDamageableTransform();
    }

    public Vector3 GetNextAttackDodgeVector(Vector3 currentPosition, float dodgeDistance){
        return aIAttackDodgeType switch{
            AIAttackDodgeType.Random => GetRandomDodgePosition(dodgeDistance),
            AIAttackDodgeType.Backpedal => GetBackpedalDodgePosition(currentPosition, dodgeDistance),
            AIAttackDodgeType.Switch_Angles => GetSwitchAnglesDodgePosition(currentPosition, dodgeDistance),
            _ => currentPosition,
        };
    }

    private Vector3 GetRandomDodgePosition(float dodgeDistance){
        return AIHelper.GetRandomCirclePosition(aIMover, currentTargetTransform.position, dodgeDistance, maxDodgePointSearchLoops);
    }

    private Vector3 GetBackpedalDodgePosition(Vector3 currentPosition, float dodgeDistance){
        float currentAngle = 0f;

        Ray potentialPosRay = new(){
            direction = Vector3.down
        };

        while(currentAngle <= 360f){
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * -Vector3.forward;
            Vector3 potentialPos = currentTargetTransform.position + (direction.normalized * dodgeDistance);

            potentialPos += currentPosition - currentTargetTransform.position;

            potentialPosRay.origin = potentialPos;

            //Translate the Y level to ground level at potentialPos
            if(Physics.Raycast(potentialPosRay, out RaycastHit hitInfo)){
                potentialPos.y = hitInfo.point.y;
            }

            //Check if the point is valid
            if(aIMover.CheckValidPosition(potentialPos, out Vector3 validPosition)){
                return validPosition;
            }

            currentAngle += 15f;   
        }

        return currentPosition;
    }

    private Vector3 GetSwitchAnglesDodgePosition(Vector3 currentPosition, float dodgeDistance){
        Vector2 randomCircle = Random.insideUnitCircle;
        Vector3 randomDirPoint = new(randomCircle.x * dodgeDistance, currentPosition.y, randomCircle.y * dodgeDistance);
        
        for (int i = 0; i < maxDodgePointSearchLoops; i++){
            Vector3 randomPoint = AIHelper.GetRandomCirclePosition(aIMover, randomDirPoint, dodgeDistance, maxDodgePointSearchLoops);
            if(aILineOfSight.IsTargetValid(currentTargetTransform, randomPoint)){
                return randomPoint;
            }   
        }

        return currentPosition;
    }

    public IDamagable GetCurrentDamagableTarget(){
        return currentTarget;
    }

    public Transform GetCurrentTargetTransform(){
        return currentTargetTransform;
    }

    public void SetIsAttacking(bool _isAttacking){
        isAttacking = _isAttacking;
    }

    public bool GetIsAttacking(){
        return isAttacking;
    }

    public bool IsInAttackRange(float distanceFromTarget){
        for (int i = 0; i < aIAttackDefinitionList.Count; i++){
            if(aIAttackDefinitionList[i].IsDistanceInAttackRange(distanceFromTarget)){
                return true;
            }
        }

        return false;
    }

    public BaseAIAttackDefinitionSO GetBestAttack(float distanceFromTarget, float healthFraction){
        if(aIAttackDefinitionList.Count == 0){
            Debug.LogError("No valid attacks are put in the AIAttackDefinitionList!");
            return null;
        }

        BaseAIAttackDefinitionSO bestAIAttack = null;
        float bestAttackScore = 0f;
        
        for (int i = 0; i < aIAttackDefinitionList.Count; i++){
            float attackScore = aIAttackDefinitionList[i].GetAttackScore(distanceFromTarget, healthFraction);
            if(attackScore == -1) continue;
            if(attackScore > bestAttackScore){
                bestAIAttack = aIAttackDefinitionList[i];
                bestAttackScore = attackScore;
            }
        }

        if(bestAIAttack == null){
            return null;
        }

        Debug.Log(bestAIAttack.name + " is the best attack with a score of: " + bestAttackScore);

        return bestAIAttack;
    }
}