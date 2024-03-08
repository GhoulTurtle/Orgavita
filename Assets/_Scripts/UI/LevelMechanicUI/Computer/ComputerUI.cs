using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private Sprite interactCursorSprite;

    private float cursorOffset = 2f;

    private List<GameObject> dragTargets = new List<GameObject>();

    private Vector2 currentMousePosition = new Vector2();

    private DesktopUI currentSelectedApplication;

    private ApplicationUI currentApplication;

    private void Awake() {
        TryGetComponent(out canvasTransform);
        TryGetComponent(out graphicRaycaster);
        computerInteractable.OnSetupComputerApplications += SetupApplicationUI;
    }

    private void Start() {
        selectedImage.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        computerInteractable.OnSetupComputerApplications -= SetupApplicationUI;
        StopAllCoroutines();
    }

    private void SetupApplicationUI(object sender, ComputerInteractable.ComputerApplicationSetupEventArgs e){
        for (int i = 0; i < e.computerApplications.Count; i++){
            Instantiate(desktopApplicationTemplate, desktopIconParent).SetupDesktopUI(this, e.computerApplications[i]);
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

    private void LaunchApplication(ComputerApplication application){
        cursorImage.sprite = loadingCursorSprite;

        desktopIconParent.gameObject.SetActive(false);

        selectedImage.gameObject.SetActive(false);

        if(application.LoadingWindowSprite != null){
            applicationLoadingImage.sprite = application.LoadingWindowSprite;
            applicationLoadingImage.gameObject.SetActive(true);
        }

        StartCoroutine(LoadingApplicationCoroutine(application));
    }

    public void CloseApplication(){
        if(currentApplication == null) return;
        cursorImage.sprite = loadingCursorSprite;
        StartCoroutine(ClosingApplicationCoroutine());
    }

    private IEnumerator ClosingApplicationCoroutine(){
        yield return new WaitForSeconds(currentApplication.ApplicationSO.LoadTime);
        Destroy(currentApplication.gameObject);
        cursorImage.sprite = defaultCursorSprite;
        desktopIconParent.gameObject.SetActive(true);
        selectedImage.gameObject.SetActive(true);        
    }

    private IEnumerator LoadingApplicationCoroutine(ComputerApplication application){
        yield return new WaitForSeconds(application.LoadTime);
        if(application.LoadingWindowSprite != null){
            applicationLoadingImage.gameObject.SetActive(false);
        }
        cursorImage.sprite = defaultCursorSprite;
        currentApplication = Instantiate(application.ApplicationUI, applicationParent);

        currentApplication.SetupApplicationUI(this, application);
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