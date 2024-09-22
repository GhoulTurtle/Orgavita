using System;
using UnityEngine;
using UnityEngine.Events;

public class DamagableObject : MonoBehaviour, IDamagable{
    [Header("Required References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Mesh destoryedMesh;
    [SerializeField] private MeshFilter meshFilter;

    [Header("Damagable Object Variables")]
    [SerializeField] private float maxHealth = 1f; 
    [SerializeField] private float forceToApplyOnDeath = 10f;

    [Header("Damagable Object Event")]
    public UnityEvent OnObjectDeath;

    private bool isDestoryed = false;

    public Transform GetDamageableTransform(){
        return transform;
    }

    public Action GetDeathAction(){
        return null;
    }

    public void TakeDamage(float damageAmount, IDamagable damageDealer, Vector3 damagePoint){     
        if(isDestoryed) return;

        maxHealth -= damageAmount;

        if(maxHealth <= 0){
            isDestoryed = true;
            OnObjectDeath?.Invoke();
            //Update the object mesh
            if(meshFilter != null){
                meshFilter.mesh = destoryedMesh;
            }

            //Trigger and apply force to the rigidbody
            if(rb != null){
                rb.isKinematic = false;
                Vector3 direction = rb.position - damagePoint;

                direction.Normalize();

                rb.AddForce(direction * forceToApplyOnDeath, ForceMode.Impulse);
            }
        }
    }
}
