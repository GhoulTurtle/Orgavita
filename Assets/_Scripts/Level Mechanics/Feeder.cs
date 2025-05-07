using System;
using UnityEngine;

public class Feeder : MonoBehaviour, IDamagable{
    public Action OnFeederDeath;

    public Transform GetDamageableTransform(){
        return transform;
    }

    public Action GetDeathAction(){
        return OnFeederDeath;
    }

    public void TakeDamage(float damageAmount, IDamagable damageDealer, Vector3 damagePoint){

    }
}
