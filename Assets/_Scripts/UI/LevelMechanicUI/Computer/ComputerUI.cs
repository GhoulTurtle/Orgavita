using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComputerUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private ComputerInteractable computerInteractable;
    [SerializeField] private DesktopUI desktopApplicationTemplate;

    [Header("UI References")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private Image selectedImage;
    [SerializeField] private Image applicationLoadingImage;
    [SerializeField] private Transform desktopIconParent;
    [SerializeField] private Transform applicationParent;
    private RectTransform canvasTransform;
    private GraphicRaycaster graphicRaycaster;

    [Header("UI Options")]
    public Color SelectedIconTint = Color.magenta;
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private Sprite loadingCursorSprite;
    [SerializeField] private Sprite clickableCursorSprite;

    public Action OnEnableUI;
    public Action OnDisableUI;
    public UnityEvent OnApplicationLaunched;
    public UnityEvent OnMouseClick;
    public Action<ComputerApplication, ApplicationUI> OnApplicationSetup;
    public Action<ComputerApplication> OnApplicationClosed;

    private float cursorOffset = 2f;

    private List<GameObject> dragTargets = new List<GameObject>();

    private Vector2 currentMousePosition = new Vector2();

    private DesktopUI currentSelectedApplication;
    private ApplicationUI currentApplicationUI;

    private ComputerCursorState currentCursorState = ComputerCursorState.Default;

    protected bool isActive = false;

    private void Awake() {
        TryGetComponent(out canvasTransform);
        TryGetComponent(out graphicRaycaster);

        computerInteractable.OnSetupComputerApplications += SetupDesktopUI;

        computerInteractable.OnEnterComputerState += EnterComputerState;
        computerInteractable.OnExitComputerState += ExitComputerState; 
    }

    private void EnterComputerState(object sender, EventArgs e){
        isActive = true;
        EnableComputerUIInteractivity();
    }

    private void ExitComputerState(object sender, EventArgs e){
        isActive = false;
        DisableComputerUIInteractivity();
    }

    private void Start() {
        selectedImage.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        computerInteractable.OnSetupComputerApplications -= SetupDesktopUI;
        computerInteractable.OnEnterComputerState -= EnableComputerUI;
        computerInteractable.OnExitComputerState -= DisableComputerUI; 
        StopAllCoroutines();
    }

    private void EnableComputerUI(object sender, EventArgs e){
        EnableComputerUIInteractivity();
        OnEnableUI?.Invoke();
    }

    private void DisableComputerUI(object sender, EventArgs e){
        DisableComputerUIInteractivity();
        OnDisableUI?.Invoke();
    }

    private void SetupDesktopUI(object sender, ComputerInteractable.ComputerApplicationSetupEventArgs e){
        for (int i = 0; i < e.computerApplications.Count; i++){
            Instantiate(desktopApplicationTemplate, desktopIconParent.transform).SetupDesktopUI(this, e.computerApplications[i]);
        }

        DisableComputerUIInteractivity();
    }

    private void EnableComputerUIInteractivity(){
        if(!isActive) return;
        if(currentApplicationUI != null){
            currentApplicationUI.EnableApplicationUIInteractivity();
            return;
        }
        
        Selectable[] selectableUI = transform.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectableUI.Length; i++){
            selectableUI[i].interactable = true;
        }
    }

    private void DisableComputerUIInteractivity(){
        if(currentApplicationUI != null){
            currentApplicationUI.DisableApplicationUIInteractivity();
            return;
        }

        Selectable[] selectableUI = transform.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectableUI.Length; i++){
            selectableUI[i].interactable = false;
        }
    }

    public bool SelectDesktopApplication(DesktopUI application){
        if(currentSelectedApplication == application){
            LaunchApplication(currentSelectedApplication.ApplicationSO);
            return true;
        }
        else{
            selectedImage.gameObject.SetActive(true);
            selectedImage.transform.position = application.transform.position;

            if(currentSelectedApplication != null) currentSelectedApplication.DeselectApplication();
            
            currentSelectedApplication = application;
            return false;
        }
    }

    public void CloseApplication(){
        if(currentApplicationUI == null) return;
        
        UpdateCursor(ComputerCursorState.Loading);
        
        StartCoroutine(ClosingApplicationCoroutine());
    }

    public void UpdateCursor(ComputerCursorState state){
        if(currentCursorState == state) return;

        currentCursorState = state;
        switch (currentCursorState){
            case ComputerCursorState.Default: cursorImage.sprite = defaultCursorSprite;
                break;
            case ComputerCursorState.Clickable: cursorImage.sprite = clickableCursorSprite;
                break;
            case ComputerCursorState.Loading: cursorImage.sprite = loadingCursorSprite;
                break;
        }
    }

    private void LaunchApplication(ComputerApplication application){
        UpdateCursor(ComputerCursorState.Loading);

        desktopIconParent.gameObject.SetActive(false);

        selectedImage.gameObject.SetActive(false);

        if(application.LoadingWindowSprite != null){
            applicationLoadingImage.sprite = application.LoadingWindowSprite;
            applicationLoadingImage.gameObject.SetActive(true);
        }

        OnApplicationLaunched?.Invoke();
        StartCoroutine(LoadingApplicationCoroutine(application));
    }


    private IEnumerator ClosingApplicationCoroutine(){
        yield return new WaitForSeconds(currentApplicationUI.ApplicationSO.LoadTime);
        OnApplicationClosed?.Invoke(currentApplicationUI.ApplicationSO);
        Destroy(currentApplicationUI.gameObject);
        
        UpdateCursor(ComputerCursorState.Default);

        desktopIconParent.gameObject.SetActive(true);
        selectedImage.gameObject.SetActive(true);        
    }

    private IEnumerator LoadingApplicationCoroutine(ComputerApplication application){
        yield return new WaitForSeconds(application.LoadTime);
        if(application.LoadingWindowSprite != null){
            applicationLoadingImage.gameObject.SetActive(false);
        }
        
        UpdateCursor(ComputerCursorState.Default);
        
        currentApplicationUI = Instantiate(application.ApplicationUI, applicationParent);

        currentApplicationUI.SetupApplicationUI(this, application);

        OnApplicationSetup?.Invoke(application, currentApplicationUI);

        if(!computerInteractable.GetIsSelected()){
            DisableComputerUIInteractivity();
        }
    }

    public void CursorMove(Vector2 cursorPosition){
        currentMousePosition = new Vector3(canvasTransform.sizeDelta.x * cursorPosition.x, canvasTransform.sizeDelta.y * cursorPosition.y, 0);

        if(cursorImage != null) cursorImage.transform.localPosition = currentMousePosition - canvasTransform.sizeDelta / cursorOffset;

        PointerEventData mouseEvent = new PointerEventData(EventSystem.current){
            position = currentMousePosition
        };

        List<RaycastResult> graphicRaycastResultsList = new List<RaycastResult>();
        graphicRaycaster.Raycast(mouseEvent, graphicRaycastResultsList);

        bool sendMouseDown = Input.GetMouseButtonDown(0);
        bool sendMouseUp = Input.GetMouseButtonUp(0);
        bool isMouseDown = Input.GetMouseButton(0);

        bool hasTriggedMouseEvent = false;

        if(sendMouseUp){
            foreach(var target in dragTargets){
                if(ExecuteEvents.Execute(target, mouseEvent, ExecuteEvents.endDragHandler)){
                    break;
                }
            }
            dragTargets.Clear();
        }

        foreach (var raycastResult in graphicRaycastResultsList){
            PointerEventData eventData = new PointerEventData(EventSystem.current){
                position = currentMousePosition
            };
            eventData.pointerCurrentRaycast = eventData.pointerPressRaycast = raycastResult;

            if(isMouseDown){
                eventData.button = PointerEventData.InputButton.Left;
                if(sendMouseDown){
                    if(!hasTriggedMouseEvent){
                        OnMouseClick?.Invoke(); 
                        hasTriggedMouseEvent = true;
                    }

                    if(ExecuteEvents.Execute(raycastResult.gameObject, eventData, ExecuteEvents.beginDragHandler)){
                        dragTargets.Add(raycastResult.gameObject);
                    }
                }
                else if (dragTargets.Contains(raycastResult.gameObject)){
                    eventData.dragging = true;
                    ExecuteEvents.Execute(raycastResult.gameObject, eventData, ExecuteEvents.dragHandler);
                }
            }

            if (sendMouseDown){
                if(ExecuteEvents.Execute(raycastResult.gameObject, eventData, ExecuteEvents.pointerDownHandler)){
                    break;
                }
            }

            else if(sendMouseUp){
                bool didRun = ExecuteEvents.Execute(raycastResult.gameObject, eventData, ExecuteEvents.pointerUpHandler);
                didRun |= ExecuteEvents.Execute(raycastResult.gameObject, eventData, ExecuteEvents.pointerClickHandler);

                if(didRun) break;
            } 
        }
    }
}