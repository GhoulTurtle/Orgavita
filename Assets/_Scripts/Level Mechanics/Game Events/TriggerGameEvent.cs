using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerGameEvent : MonoBehaviour{
    [Header("Trigger Game Event Setting")]
    [SerializeField] private UnityEvent OnGameEventTriggered;
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool deactivateOnTrigger = true;

    private Collider triggerCollider;

    private const string PLAYER = "Player";

    private void Awake() {
        TryGetComponent(out triggerCollider);
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag(PLAYER) || !isActive) return;

        OnGameEventTriggered?.Invoke();

        if(deactivateOnTrigger){
            Destroy(this);
        }
    }

    public void SetIsActive(bool state){
        isActive = state;
    }
}