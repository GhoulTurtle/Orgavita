using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothFollow : MonoBehaviour{
    [Header("Variables")]
    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplier;

    private float mouseX;
    private float mouseY;

    private Quaternion rotationX;
    private Quaternion rotationY;

    private Quaternion targetRotation;
    
    private readonly float snapDistance = 0.001f;

    private void Update() {
        if(transform.localRotation == targetRotation) return;

        if(Vector3.Distance(transform.localEulerAngles, targetRotation.eulerAngles) < snapDistance){
            transform.localRotation = targetRotation;
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public void MoveCameraInput(InputAction.CallbackContext context){
		var inputVector = context.ReadValue<Vector2>();
		mouseX += inputVector.x * swayMultiplier;
		mouseY -= inputVector.y * swayMultiplier;
		
        rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
        rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        targetRotation = rotationX * rotationY;
    }
}