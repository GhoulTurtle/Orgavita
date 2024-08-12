using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombineMenuUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("UI References")]
    [SerializeField] private Transform combineMenuParent;
    [SerializeField] private Button combineButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button increaseCombineAmountButton;
    [SerializeField] private Button decreaseCombineAmountButton;
    [SerializeField] private CombineUI firstCombineUI;
    [SerializeField] private CombineUI secondCombineUI;
    [SerializeField] private CombineUI resultCombineUI;

    [Header("Combine Menu Animation Variables")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Vector2 animationPopoutOffset;

    private Vector2 combineMenuOriginalPosition;
    private Vector2 combineMenuPopupGoalPosition;

    private IEnumerator combineMenuPopoutAnimationCoroutine;

    private void Awake() {
        DisableCombineMenuInteractivity();

        combineMenuOriginalPosition = combineMenuParent.localPosition;
        combineMenuPopupGoalPosition = combineMenuOriginalPosition + animationPopoutOffset;
    }

    private void Start() {
        SetupCombineUI();
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void ShowCombineUI(){
        EnableCombineMenuInteractivity();
        
        if(combineMenuPopoutAnimationCoroutine != null){
            StopCoroutine(combineMenuPopoutAnimationCoroutine);
            combineMenuPopoutAnimationCoroutine = null;
        }
        
        combineMenuPopoutAnimationCoroutine = UIAnimator.LerpingAnimationCoroutine(combineMenuParent, combineMenuPopupGoalPosition, animationDuration, false);
        StartCoroutine(combineMenuPopoutAnimationCoroutine);

        inventoryUI.GetInventoryMenuSelector().SetTarget(combineButton.transform);
    }

    public void HideCombineUI(){        
        DisableCombineMenuInteractivity();

        if(combineMenuPopoutAnimationCoroutine != null){
            StopCoroutine(combineMenuPopoutAnimationCoroutine);
            combineMenuPopoutAnimationCoroutine = null;
        }

        combineMenuPopoutAnimationCoroutine = UIAnimator.LerpingAnimationCoroutine(combineMenuParent, combineMenuOriginalPosition, animationDuration, false);
        StartCoroutine(combineMenuPopoutAnimationCoroutine);
    }

    public void UpdateCombineMenuUI(InventoryItem firstInventoryItem, InventoryItem secondInventoryItem, InventoryItem resultingInventoryItem){
        
    }

    private void EnableCombineMenuInteractivity(){
        combineButton.interactable = true;
        cancelButton.interactable = true;
        increaseCombineAmountButton.interactable = true;
        decreaseCombineAmountButton.interactable = true;
    }

    private void DisableCombineMenuInteractivity(){
        combineButton.interactable = false;
        cancelButton.interactable = false;
        increaseCombineAmountButton.interactable = false;
        decreaseCombineAmountButton.interactable = false;
    }

    private void SetupCombineUI(){
        firstCombineUI.SetupCombineUI(this);
        secondCombineUI.SetupCombineUI(this);
        resultCombineUI.SetupCombineUI(this);
    }
}
