using UnityEngine;

public class DamageTest : MonoBehaviour{
    [Header("Damage Test Variables")]
    [SerializeField] private bool healPlayer;
    [SerializeField] private float damageAmount;
    [SerializeField] private float healAmount;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.TryGetComponent(out Health health)){
            if(healPlayer){
                health.HealHealth(healAmount);
                return;
            }

            health.TakeDamage(damageAmount, null, transform.position);
        }
    }
}
