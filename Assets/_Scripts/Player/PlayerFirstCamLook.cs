using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFirstCamLook : MonoBehaviour{
	[Header("Cam Variables")]
	[SerializeField] private float camSens;
	[SerializeField] private bool lockCursor;

	[Header("Rquired Reference")]
	[SerializeField] private Transform cameraRoot;
	[SerializeField] private Transform characterOrientation;

	private const float YClamp = 80f;

	private float camX;
	private float camY;

	private void Start() {
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void OnLookInput(InputAction.CallbackContext context){
		var inputVector = context.ReadValue<Vector2>();
		camX += inputVector.x * camSens * Time.deltaTime;
		camY -= inputVector.y * camSens * Time.deltaTime;
		camY = Mathf.Clamp(camY, -YClamp, YClamp);
	}

	public void Update(){
		cameraRoot.localRotation = Quaternion.Euler(camY, camX, 0);

		characterOrientation.transform.localRotation = Quaternion.Euler(0, camX, 0);
	}
}
