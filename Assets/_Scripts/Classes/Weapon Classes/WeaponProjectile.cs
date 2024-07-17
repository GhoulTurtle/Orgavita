using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponProjectile : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Rigidbody myRigidbody;

    private float damageAmount;
    private float explosionForce;
    private float explosionRadius;
    private LayerMask damageLayerMask;

    public void SetupProjectile(Vector3 shootVector, CrossbowWeaponDataSO crossbowWeaponDataSO, LayerMask _damageLayerMask){
        transform.rotation = Quaternion.LookRotation(shootVector);
        myRigidbody.AddForce(shootVector * crossbowWeaponDataSO.arrowSpeed, ForceMode.Impulse);
        
        damageAmount = crossbowWeaponDataSO.weaponAttackDamage;
        explosionForce = crossbowWeaponDataSO.explosionForce;
        explosionRadius = crossbowWeaponDataSO.explosionRadius;

        damageLayerMask = _damageLayerMask;
    }

    public void Explode(){
        myRigidbody.isKinematic = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayerMask);

        foreach (Collider hit in colliders){

            if(hit.TryGetComponent(out IDamagable hitDamagable)){
                hitDamagable.TakeDamage(damageAmount, transform.position);
            }

            if (hit.TryGetComponent(out Rigidbody hitRigidbody)){
                hitRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }
}
