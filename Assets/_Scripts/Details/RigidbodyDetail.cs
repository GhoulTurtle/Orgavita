using UnityEngine;
using UnityEngine.Events;

public class RigidbodyDetail : MonoBehaviour{
    [Header("Rigidbody Detail References")]
    [SerializeField] private bool triggerEventOnFirstCollision = true;
    [SerializeField] private UnityEvent OnFirstCollision;

    private bool hasCollided = false;

    private Rigidbody detailRigidbody;

    private void Awake() {
        if(!TryGetComponent(out detailRigidbody)){
            Debug.LogWarning("RigidbodyDetail " + gameObject.name + "doesn't have a rigidbody attached, animations will not be played properly.");
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(hasCollided || !triggerEventOnFirstCollision) return;

        OnFirstCollision?.Invoke();
        hasCollided = true;
    }

    public void ApplyImpulseForce(Vector3 forceDirection, float forceStrength){
        if(detailRigidbody != null){
            Vector3 dir = forceDirection.normalized;

            dir *= forceStrength;

            detailRigidbody.AddForce(dir, ForceMode.Impulse);
        }
    }
}
