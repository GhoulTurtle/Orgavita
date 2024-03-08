using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComputerUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private ComputerInteractable computerInteractable;

    [Header("UI References")]
    [SerializeField] private Image cursorImage;
    private RectTransform canvasTransform;
    private GraphicRaycaster graphicRaycaster;

    [Header("UI Variables")]
    private float cursorOffset = 2f;

    private List<GameObject> dragTargets = new List<GameObject>();

    private Vector2 currentMousePosition = new Vector2();

    private void Awake() {
        TryGetComponent(out canvasTransform);
        TryGetComponent(out graphicRaycaster);
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
