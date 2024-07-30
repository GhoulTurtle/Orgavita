using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIAttackDefinitionSO : ScriptableObject{
    [Header("Base Attack Variables")]
    [SerializeField, MinMaxRange(0.5f, 300f)] private RangedFloat attackRange;
    [Tooltip("The amount of time the AI will have to perform dodges in order to be able to attack again.")]
    [SerializeField] private float attackRestPeriodInSeconds;
    [Tooltip("The amount of time that this attack takes to perform.")]
    [SerializeField] private float attackSpeedInSeconds;
    [Tooltip("The amount of damage this attack will do.")]
    [SerializeField] private float attackDamage;

    [Header("Attack Score Evaulation Curves")]
    [Tooltip("The curve that will be used in evaulating the distance from the target.")]
    [SerializeField] private AnimationCurve rangeEvaulationCurve;
    [Tooltip("The curve that will be used in evaulating the rest period based on the current health of the AI.")]
    [SerializeField] private AnimationCurve restPeriodEvaulationCurve;
    [Tooltip("The curve that will be used in evaulating the attack speed based on the current health of the AI.")]
    [SerializeField] private AnimationCurve attackSpeedEvaulationCurve;
    [Tooltip("The curve that will be used in evaulating the attack damage based on the current health of the AI.")]
    [SerializeField] private AnimationCurve attackDamgageEvaulationCurve;

    public virtual void PerformAttack(AIStateMachine aIStateMachine){

    }

    //Invalid attack = -1, otherwise rate the score from 0-100 with 0 being the worse and 100 being the best.
    public float GetAttackScore(float distanceFromTarget, float currentHealth){
        return 0f;
    }
}