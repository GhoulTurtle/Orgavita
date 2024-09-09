using UnityEngine;

public class ChildBehaviour : MonoBehaviour{	
	[Header("Child Behaviour Data")]
	[SerializeField] private ChildBehaviourData defaultChildBehaviourData = new ChildBehaviourData();

	private Quaternion offsetQuaternion;

	private Quaternion finalRotation;
	private Vector3 finalPostion;

	private ChildBehaviourData currentChildBehaviourData;

	private void Start() {
		currentChildBehaviourData = defaultChildBehaviourData;
	}

	private void Update(){
		if(currentChildBehaviourData.lockPos == null || Time.timeScale == 0) return;
		
		if(currentChildBehaviourData.keepOffset){
			offsetQuaternion = Quaternion.Euler(currentChildBehaviourData.offsetRotation);

			finalRotation = currentChildBehaviourData.lockPos.rotation * offsetQuaternion;
			finalPostion = currentChildBehaviourData.lockPos.rotation * currentChildBehaviourData.offsetPos + currentChildBehaviourData.lockPos.position;

			if(currentChildBehaviourData.delayPosUpdate){
				transform.SetPositionAndRotation(Vector3.Lerp(transform.position, finalPostion, currentChildBehaviourData.followSpeed * Time.deltaTime), Quaternion.Slerp(transform.rotation, finalRotation, currentChildBehaviourData.followSpeed * Time.deltaTime));
			}
			else{
				transform.SetPositionAndRotation(finalPostion, finalRotation);
			}
        }
		else{
			if(currentChildBehaviourData.delayPosUpdate){
				transform.position = Vector3.Lerp(transform.position, currentChildBehaviourData.lockPos.position, currentChildBehaviourData.followSpeed * Time.deltaTime);
			}
			else{
				transform.position = currentChildBehaviourData.lockPos.position;
			}
		}
	}

	public void SetChildBehaviourData(ChildBehaviourData childBehaviourData){
		if(childBehaviourData == currentChildBehaviourData) return;

		if(childBehaviourData.lockPos == null && defaultChildBehaviourData.lockPos != null){
			childBehaviourData.lockPos = defaultChildBehaviourData.lockPos;
		}

		currentChildBehaviourData = childBehaviourData;
	}

	public void ResetChildBehaviourData(){
		currentChildBehaviourData = defaultChildBehaviourData;
	}
}
