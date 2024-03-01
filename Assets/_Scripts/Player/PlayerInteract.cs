using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private LayerMask interactLayer; 
	[SerializeField] private Transform playerCamera;

	[Header("Interact Variables")]
	[SerializeField] private float interactionRadius;
	[SerializeField] private float interactionDistance;
	[SerializeField] private float interactionCooldown = 0.25f;
	
	public EventHandler<InteractableEventArgs> OnInteractHover;
	public EventHandler<InteractableEventArgs> OnInteractSuccess;
	public EventHandler<InteractableEventArgs> OnFailedInteract;

	private IInteractable currentInteractable;

	public class InteractableEventArgs{
		public IInteractable Interactable;

		public InteractableEventArgs(IInteractable interactable){
			Interactable = interactable;
		}
	}

	private bool canInteract = true;
	private WaitForSecondsRealtime interactCooldownTimer;

	private void Awake() {
		interactCooldownTimer = new WaitForSecondsRealtime(interactionCooldown);
	}

	private void OnDestroy() {
		StopAllCoroutines();
	}

	private void Update() {
		if(!GetInteractable(out IInteractable interactable) && currentInteractable != null){
			currentInteractable = null;
			OnInteractHover?.Invoke(this, new InteractableEventArgs(null));
			return;
		}

		else if(interactable != null){
			currentInteractable = interactable;
			OnInteractHover?.Invoke(this, new InteractableEventArgs(currentInteractable));
		}
	}

	public void Interact(InputAction.CallbackContext context){
		if(!canInteract || playerCamera == null || context.phase != InputActionPhase.Performed) return;

		if(GetInteractable(out IInteractable interactable)){
			currentInteractable = interactable;
			
			if(interactable.Interact(this)){
				OnInteractSuccess?.Invoke(this, new InteractableEventArgs(currentInteractable));
			}
			else{
				OnFailedInteract?.Invoke(this, new InteractableEventArgs(currentInteractable));
			}

			StartCoroutine(InteractCoroutine());
		}
	}

	private bool GetInteractable(out IInteractable interactable){
		if(Physics.SphereCast(playerCamera.position, interactionRadius, playerCamera.forward, out RaycastHit hitInfo, interactionDistance, interactLayer)){
			if(!hitInfo.collider.TryGetComponent(out IInteractable _interatable)){
				interactable = null;
				return false;
			}

			interactable = _interatable;
			return true;
		}
		
		interactable = null;
		return false;
	}

	private IEnumerator InteractCoroutine(){
		canInteract = false;
		yield return interactCooldownTimer;
		canInteract = true;
	}
}
