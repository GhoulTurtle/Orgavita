using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ObjectUIButton : MonoBehaviour{
    [Header("Button Options")]
    [SerializeField] private bool isEnabled = true;
    
    [Header("Interaction Events")]
    [Tooltip("Fires when the mouse hovers over the element.")]
    public UnityEvent HoverEvent;
    [Tooltip("Fires when the mouse stops hovers over the element.")]
    public UnityEvent HoverExitEvent;
    [Tooltip("Fires when the mouse clicks on the element.")]
    public UnityEvent ClickEvent;

    private Collider objectCollider;

    private void Awake() {
        TryGetComponent(out objectCollider);
    }

    private void Start() {
        objectCollider.isTrigger = true;
    }

    private void OnMouseEnter() {
        if(!isEnabled) return;
        HoverEvent?.Invoke();
    }

    private void OnMouseExit() {
        if(!isEnabled) return;
        HoverExitEvent?.Invoke();
    }

    private void OnMouseDown() {
        if(!isEnabled) return;
        ClickEvent?.Invoke();
    }

    public void SetIsEnabled(bool state){
        isEnabled = state;
    }
}
