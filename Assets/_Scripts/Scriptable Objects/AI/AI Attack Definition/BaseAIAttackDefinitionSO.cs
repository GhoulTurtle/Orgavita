using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Base AI Attack Definition", fileName = "NewBaseAIAttackDefinitionSO")]
public class BaseAIAttackDefinitionSO : ScriptableObject{
    [Header("Base Attack Variables")]
    [Tooltip("The amount of time that this attack takes to perform.")]
    [SerializeField] private float attackSpeedInSeconds;
    public float AttackSpeedInSeconds{
        get { return attackSpeedInSeconds;}
        set{
            attackSpeedInSeconds = value;
            OnBaseAttackSpeedValueChanged();
        }
    }
    [Tooltip("The amount of time the AI will have to perform dodges in order to be able to attack again.")]
    [MinMaxRange(0.5f, 10f)] public RangedFloat attackDodgePeriodInSeconds;
    [Tooltip("The amount of damage this attack will do.")]
    public float attackDamage;
    [MinMaxRange(0f, 300f)] public RangedFloat attackRange;

    [Header("Attack Score Evaulation Curves")]
    [Tooltip("The curve that will be used in evaulating the distance from the target.")]
    public AnimationCurve rangeEvaulationCurve = new(rangeEvaulationDefaultKeyframes);
    [Tooltip("The curve that will be used in evaulating the rest period based on the current health of the AI.")]
    public AnimationCurve restPeriodEvaulationCurve = new(restPeriodEvaulationDefaultKeyframes);
    [Tooltip("The curve that will be used in evaulating the attack speed based on the current health of the AI.")]
    public AnimationCurve attackSpeedEvaulationCurve = new(attackSpeedEvaulationDefaultKeyframes);
    [Tooltip("The curve that will be used in evaulating the attack damage based on the current health of the AI.")]
    public AnimationCurve attackDamageEvaulationCurve = new(attackDamageEvaulationDefaultKeyframes);

    public Action OnAttackStart;
    public Action OnAttackPerformed;
    public Action OnAttackRest;
    public Action OnAttackEnd;

    private static readonly float evaulationTotal = 4f;

    private static readonly Keyframe[] rangeEvaulationDefaultKeyframes = new Keyframe[]{
        new Keyframe(0f, 0f),
        new Keyframe(0.5f, 1f),
        new Keyframe(1f, 0f)
    };

    private static readonly Keyframe[] restPeriodEvaulationDefaultKeyframes = new Keyframe[]{
        new Keyframe(0f, 0f),
        new Keyframe(1f, 1f),
    };

    private static readonly Keyframe[] attackSpeedEvaulationDefaultKeyframes = new Keyframe[]{
        new Keyframe(0f, 1f),
        new Keyframe(0.75f, 0.5f),
        new Keyframe(1f, 0f),
    };

    private static readonly Keyframe[] attackDamageEvaulationDefaultKeyframes = new Keyframe[]{
        new Keyframe(0f, 0f),
        new Keyframe(1f, 1f)
    };

    //Must position the aI in a valid position to be able to damage the player, then trigger the attack, and call the OnAttackStart and OnAttackEnd actions.
    public virtual void PerformAttack(AIStateMachine aIStateMachine, Transform targetTransform){

    }

    //Invalid attack = -1, otherwise rate the score from 0-100 with 0 being the worse and 100 being the best.
    public float GetAttackScore(float distanceFromTarget, float healthScore){
        if (distanceFromTarget > attackRange.maxValue || distanceFromTarget < attackRange.minValue) return -1f;

        float distanceScore = 1f - ((distanceFromTarget - attackRange.minValue) / (attackRange.maxValue - attackRange.minValue));
        distanceScore = Mathf.Clamp01(distanceScore);

        healthScore = Mathf.Clamp01(healthScore);

        float currentAttackScore = (rangeEvaulationCurve.Evaluate(distanceScore) + restPeriodEvaulationCurve.Evaluate(healthScore)
                     + attackSpeedEvaulationCurve.Evaluate(healthScore) + attackDamageEvaulationCurve.Evaluate(healthScore)) / evaulationTotal;

        currentAttackScore *= 100f;

        return currentAttackScore;
    }

    public bool IsDistanceInAttackRange(float distance){
        if (distance > attackRange.maxValue || distance < attackRange.minValue) return false;
        return true;
    }

    private void OnValidate() {
        OnBaseAttackSpeedValueChanged();    
    }

    protected virtual void OnBaseAttackSpeedValueChanged(){
        // Base implementation (if any) or leave empty
    }
}