using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractableMovingObject : MonoBehaviour, IInteractable{
    [Header("Moving Object Variables")]
    [SerializeField] private float moveTimeInSeconds = 0.5f;
    [SerializeField] private AnimationCurve movingCurve;
    [SerializeField] private Vector3 goalPos;
    [SerializeField] private string startToGoalPrompt;
    [SerializeField] private string goalToStartPrompt;
    [SerializeField] private bool atGoalPos = false;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isStoppable = true;
    [SerializeField] private bool useLocalSpace =  false;
    [SerializeField] private bool oneShot = false;

    [Header("Moving Object Events")]
    [SerializeField] private UnityEvent OnStartToGoalEvent;
    [SerializeField] private UnityEvent OnGoalToStartEvent;

    private Vector3 startPos;
    private Vector3 offsetPos;

    private IEnumerator currentMovingCoroutine;

    private string interactionPrompt;

    public string InteractionPrompt => interactionPrompt;

    public bool canMove = true;

    private void Awake() {
        if(useLocalSpace){
            startPos = transform.TransformPoint(transform.localPosition);
            offsetPos = transform.TransformPoint(goalPos);
        }
        else{
            startPos = transform.position;
            offsetPos = startPos + goalPos;
        }

        if(atGoalPos){
            if(useLocalSpace){
                transform.localPosition = offsetPos;
            }
            else{
                transform.position = offsetPos;
            }
        }

        UpdateInteractionPrompt();
    }

    public bool Interact(PlayerInteract player){
        if (!canMove || !isInteractable) return false;
        
        MoveObject();

        return true;
    }

    public void MoveObject(){
        if (currentMovingCoroutine != null) return;
        
        currentMovingCoroutine = MoveToPosition();
        StartCoroutine(currentMovingCoroutine);

        if(oneShot){
            isInteractable = false;
        }
    }

    private void UpdateInteractionPrompt(){
        if(!isInteractable){
            interactionPrompt = "";
            return;
        } 
        
        interactionPrompt = !atGoalPos ? startToGoalPrompt : goalToStartPrompt;
    }

    private IEnumerator MoveToPosition(){
        float t = 0f;
        
        Vector3 startingPoint = transform.position;
        Vector3 endPoint = !atGoalPos ? offsetPos : startPos; 

        if(endPoint == offsetPos){
            OnStartToGoalEvent?.Invoke();
        }
        else{
            OnGoalToStartEvent?.Invoke();
        }

        while (t < 1){
            t += Time.deltaTime / moveTimeInSeconds;
            transform.position = Vector3.Lerp(startingPoint, endPoint, movingCurve.Evaluate(t));
            yield return null;
        }

        transform.position = endPoint;
        atGoalPos = endPoint == offsetPos;
        currentMovingCoroutine = null;
        UpdateInteractionPrompt();
    }

    private void StopMovingCoroutine(){
        if(currentMovingCoroutine == null) return;

        atGoalPos = !atGoalPos;
        StopCoroutine(currentMovingCoroutine);
        currentMovingCoroutine = null;
        UpdateInteractionPrompt();
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player") || !isStoppable) return;

        Vector3 startingPoint = transform.position;
        Vector3 endPoint = !atGoalPos ? offsetPos : startPos; 
        
        Vector3 moveDirection = (endPoint - startingPoint).normalized;
        
        Vector3 playerDir = (other.transform.position - transform.position).normalized;

        float dotProduct = Vector3.Dot(moveDirection, playerDir);

        if (dotProduct > 0) {
            StopMovingCoroutine();
            canMove = false;
        } 
        else {
            canMove = true;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(!other.CompareTag("Player") || !isStoppable) return;

        Vector3 startingPoint = transform.position;
        Vector3 endPoint = !atGoalPos ? offsetPos : startPos; 
        
        Vector3 moveDirection = (endPoint - startingPoint).normalized;
        
        Vector3 playerDir = (other.transform.position - transform.position).normalized;

        float dotProduct = Vector3.Dot(moveDirection, playerDir);

        if (dotProduct > 0) {
            StopMovingCoroutine();
            canMove = false;
        } 
        else {
            canMove = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(!other.CompareTag("Player") || !isStoppable) return;

        canMove = true;
    }
}