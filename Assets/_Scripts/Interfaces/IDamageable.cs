using System;
using UnityEngine;

public interface IDamagable{
    public abstract void TakeDamage(float damageAmount, IDamagable damageDealer, Vector3 damagePoint);
    public abstract Transform GetDamageableTransform();
    public abstract Action GetDeathAction();
}