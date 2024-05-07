using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelector : MonoBehaviour{
    [Header("Selector Variables")]
    [SerializeField] private float selectorMoveTime = 0.5f;

    [Header("Required Referecnes")]
    [SerializeField] private Transform selectorImageTransform;
    private RectTransform selectorRectTransform;

    private Transform currentSelectedTransform;
    private RectTransform currentSelectedRectTransform;

    private EventSystem CurrentEventSystem;

    private IEnumerator currentMoveAnimation;

    private const float stopMovingDistanceFromTarget = 0.01f;

    private bool isActive;
    private bool isFirstActivation = true;

    private void Awake() {
        CurrentEventSystem = EventSystem.current;
        
        selectorImageTransform.TryGetComponent(out selectorRectTransform);
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    private void Update(){
        if(!isActive) return;

        if(isFirstActivation){
            SetTarget(currentSelectedTransform);
            isFirstActivation = false;  
        }

        DetectNewObjectSelected();
    }

    private void DetectNewObjectSelected(){
        if(currentSelectedTransform == null) return;

        if(CurrentEventSystem.currentSelectedGameObject != currentSelectedTransform.gameObject){
            SetTarget(CurrentEventSystem.currentSelectedGameObject.transform);
        }
    }

    public void SetTarget(Transform target){
        if(CurrentEventSystem.currentSelectedGameObject != target.gameObject){
            CurrentEventSystem.SetSelectedGameObject(target.gameObject);
        }

        currentSelectedTransform = target;
        target.TryGetComponent(out currentSelectedRectTransform);
        
        if(currentMoveAnimation != null){
            StopCoroutine(currentMoveAnimation);
            currentMoveAnimation = null;
        }

        currentMoveAnimation = CursorMoveAnimationCoroutine();

        StartCoroutine(currentMoveAnimation);
    }

    private IEnumerator CursorMoveAnimationCoroutine(){
        float current = 0;
        while(Vector3.Distance(selectorImageTransform.position, currentSelectedTransform.position) > stopMovingDistanceFromTarget){
            MoveSelector(current);
            current += Time.deltaTime;
            yield return null;
        }

        SetSelectorPostionAndScaleToCurrentSelected();
    }

    private void SetSelectorPostionAndScaleToCurrentSelected(){
        selectorRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSelectedRectTransform.rect.size.x);
        selectorRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentSelectedRectTransform.rect.size.y);

        selectorImageTransform.position = currentSelectedTransform.transform.position;
    }

    private void MoveSelector(float current){
        selectorImageTransform.position = Vector3.Lerp(selectorImageTransform.position, currentSelectedTransform.transform.position, current / selectorMoveTime);

        if(currentSelectedRectTransform == null) return;

        // scale the cursor smoothly to the scale of the selected rect transform
        var horizontalLerp = Mathf.Lerp(selectorRectTransform.rect.size.x, currentSelectedRectTransform.rect.size.x, current / selectorMoveTime);
        var verticalLerp = Mathf.Lerp(selectorRectTransform.rect.size.y, currentSelectedRectTransform.rect.size.y, current / selectorMoveTime);

        // apply the lerp calculation to the rect transform
        selectorRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalLerp);
        selectorRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalLerp);
    }

    public void EnableSelector(){
        isActive = true;
        
        if(currentSelectedTransform != null){
            CurrentEventSystem.SetSelectedGameObject(currentSelectedTransform.gameObject);
            Invoke(nameof(SetSelectorPostionAndScaleToCurrentSelected), 0.1f); //TEMP Needs refactoring
        }

        ShowSelectorVisual();
    }

    public void DisableSelector(){
        if(currentMoveAnimation != null){
            StopCoroutine(currentMoveAnimation);
            currentMoveAnimation = null;
        }
        isActive = false;
        CurrentEventSystem.SetSelectedGameObject(null);
        HideSelectorVisual();
    }

    private void HideSelectorVisual(){
        selectorImageTransform.gameObject.SetActive(false);
    }

    private void ShowSelectorVisual(){
        selectorImageTransform.gameObject.SetActive(true);
    }

    public void SetCursorStartingSelection(Transform _startingSelectedObject){
        if(_startingSelectedObject == null) return;

        currentSelectedTransform = _startingSelectedObject;
        currentSelectedTransform.TryGetComponent(out currentSelectedRectTransform);
    }
}