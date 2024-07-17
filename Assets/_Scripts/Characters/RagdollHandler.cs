using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A script responsible to activate/deactive a characters ragdoll, and removing joints for a specific body part for gibs.
/// </summary>
public class RagdollHandler : MonoBehaviour{
    [SerializeField] private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    [SerializeField] private List<CharacterJoint> characterJoints = new List<CharacterJoint>();

    private void Awake() {
        if(rigidbodies.Count == 0){
            GetCharacterRagdoll();
        }
    }

    public void RemoveCharacterRagdollBodyPart(CharacterJoint jointToRemove){
        if(characterJoints.Contains(jointToRemove)){
            characterJoints.Remove(jointToRemove);
        }

        if(jointToRemove.TryGetComponent(out Rigidbody rigidbody)){
            if(rigidbodies.Contains(rigidbody)){
                rigidbodies.Remove(rigidbody);
            }

            Destroy(jointToRemove);
            Destroy(rigidbody);
        }
    }

    [ContextMenu("Get Character Ragdoll.")]
    private void GetCharacterRagdoll(){
        Rigidbody[] rigidbodiesComponents = transform.GetComponentsInChildren<Rigidbody>();
        CharacterJoint[] characterJointsComponents = transform.GetComponentsInChildren<CharacterJoint>();

        rigidbodies.Clear();
        for (int i = 0; i < rigidbodiesComponents.Length; i++){
            rigidbodies.Add(rigidbodiesComponents[i]);
        }

        characterJoints.Clear();
        for (int i = 0; i < characterJointsComponents.Length; i++){
            characterJoints.Add(characterJointsComponents[i]);
        }
    }

    [ContextMenu("Deactivate Character's Ragdoll.")]
    public void DeactivateCharacterRagdoll(){
        ChangeCharacterRagdollState(false);
    }

    [ContextMenu("Activate Character's Ragdoll.")]
    public void EnableCharacterRagdoll(){
        ChangeCharacterRagdollState(true);
    }

    private void ChangeCharacterRagdollState(bool state){
        for (int i = 0; i < rigidbodies.Count; i++){
            rigidbodies[i].detectCollisions = state;
            rigidbodies[i].useGravity = state;
            if(!state) rigidbodies[i].isKinematic = true;
            else rigidbodies[i].isKinematic = false;
        }

        for (int i = 0; i < characterJoints.Count; i++){
            characterJoints[i].enableCollision = state;
        }
    }

    public List<GameObject> GetRagdollGameObjects(){
        if(rigidbodies.Count != 0){
            List<GameObject> ragdollTransforms = new List<GameObject>();
            for (int i = 0; i < rigidbodies.Count; i++){
                ragdollTransforms.Add(rigidbodies[i].gameObject);
            }

            return ragdollTransforms;
        }

        return null;
    }
}