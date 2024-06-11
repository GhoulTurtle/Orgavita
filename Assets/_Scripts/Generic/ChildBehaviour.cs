using UnityEngine;

public class ChildBehaviour : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private Transform lockPos;
	
	[Header("Location Variables")]
	[SerializeField] private bool keepOffset;
	[SerializeField] private Vector3 offsetPos;
	[SerializeField] private Vector3 offsetRotation;
	[Space(3f)]
	[SerializeField] private bool delayPosUpdate;
	[SerializeField] private float followSpeed;

	private Quaternion offsetQuaternion;

	private Quaternion finalRotation;
	private Vector3 finalPostion;

	private void Start() {
		offsetQuaternion = Quaternion.Euler(offsetRotation);
	}

	private void Update(){
		if(lockPos == null) return;
		
		if(keepOffset){
			finalRotation = lockPos.rotation * offsetQuaternion;
			finalPostion = lockPos.rotation * offsetPos + lockPos.position;

			if(delayPosUpdate){
				transform.SetPositionAndRotation(Vector3.Lerp(transform.position, finalPostion, followSpeed * Time.deltaTime), Quaternion.Slerp(transform.rotation, finalRotation, followSpeed * Time.deltaTime));
			}
			else{
				transform.SetPositionAndRotation(finalPostion, finalRotation);
			}
        }
		else{
			if(delayPosUpdate){
				transform.position = Vector3.Lerp(transform.position, lockPos.position, followSpeed * Time.deltaTime);
			}
			else{
				transform.position = lockPos.position;
			}
		}
	}
}
