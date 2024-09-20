using UnityEngine;

public class RigidbodyObject : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Rigidbody rb;

    [Header("Rigidbody Object Variables")]
    [SerializeField] private bool isOneShot = true;
    [SerializeField] private Vector3 forceDirectionNormalized;
    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float force;

    private bool hasAppliedForce = false;

    public void ApplyForce(){
        if(rb == null) return;
        if(hasAppliedForce && isOneShot) return;

        rb.AddForce(forceDirectionNormalized.normalized * force, forceMode);

        hasAppliedForce = true;
    }
}