using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerGameEvent : MonoBehaviour{
    [Header("Trigger Game Event Variables")]
    [SerializeField] private UnityEvent OnGameEventTriggered;
    [SerializeField] private bool deactivateOnTrigger = true;

    private Collider triggerCollider;

    private const string PLAYER = "Player";

    private void Awake() {
        TryGetComponent(out triggerCollider);
        triggerCollider.isTrigger = true;
    }

    private void Start() {
        triggerCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag(PLAYER)) return;

        OnGameEventTriggered?.Invoke();

        if(deactivateOnTrigger){
            triggerCollider.enabled = false;
        }
    }
}