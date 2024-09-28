using UnityEngine;

public class LevelTransition : MonoBehaviour, IInteractable{
    public string InteractionPrompt => interactionPrompt; 

    public TransitionPointToSpawnAt TransitionPointToSpawn => thisTransistionPoint;

    [Header("Level Transition Definition")]
    [SerializeField] private TransitionPointToSpawnAt thisTransistionPoint;
    [SerializeField] private Vector3 transitionSpawn  = new Vector3(0, 1, 0);
    [SerializeField, Range(0, 1)] private float lookRotationX = 0f;
    [SerializeField, Range(-1, 1)] private float lookRotationY = 0f;

    [Header("Required References")]
    [SerializeField] private RoomDataSO roomToTransitionTo;
    [SerializeField] private TransitionPointToSpawnAt pointToSpawnAt;

    [Header("Level Transition Variables")]
    [SerializeField] private FadeTime fadeTime = new FadeTime(-1f, -1f);

    [Header("Debugging")]
    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private float lookRotationXCircleRadius = 0.5f;
    [SerializeField] private Color spawnGizmoColor = Color.green;
    [SerializeField] private Color lookGizmoColor = Color.red;

    private Vector2 lookRotationXVector;

    private string interactionPrompt = "Transition to ";

    private void Awake() {
        interactionPrompt += roomToTransitionTo.roomName;
        RoomSceneManager.OnGetLevelTransition += EvaluateTransitionPoint;
    }

    private void OnDestroy() {
        RoomSceneManager.OnGetLevelTransition -= EvaluateTransitionPoint;
    }

    private void EvaluateTransitionPoint(TransitionPointToSpawnAt transitionPointToSpawnAt){
        if(transitionPointToSpawnAt != thisTransistionPoint) return;
        
        //This is the proper spawn point
        RoomSceneManager.SetTransitionPoint(this);
    }

    public bool Interact(PlayerInteract player){
        //Trigger our scene level manager
        RoomSceneManager.LoadRoom(roomToTransitionTo, pointToSpawnAt, fadeTime);
        return true;
    }

    private Vector2 TranslateLookVectorX(){
        float angle = Mathf.PI * 2f * lookRotationX;

        float x = Mathf.Cos(angle) * lookRotationXCircleRadius;
        float y = Mathf.Sin(angle) * lookRotationXCircleRadius;

        return new Vector2(x, y);
    }

    public Vector3 GetPointToSpawnAt(){
        Vector3 translatedSpawn = transform.TransformPoint(transitionSpawn);
        return translatedSpawn;
    }

    public Vector2 GetLookRotation(){
        lookRotationXVector = TranslateLookVectorX();
        
        Vector3 invertedLookDirection = new Vector3(-lookRotationXVector.x, -lookRotationY, -lookRotationXVector.y);

        float yaw;
        //If the x axis is zero then point either right at 0 or 180
        if(Mathf.Abs(invertedLookDirection.x) < 0.0001f){
            yaw = (invertedLookDirection.z > 0) ? 0f : 180f;
        }
        //If the z axis is zero then point either right at 0 or 180
        else if(Mathf.Abs(invertedLookDirection.z) < 0.0001f){
            yaw = (invertedLookDirection.x > 0) ? 0f : 180f;
        }
        else{
            yaw = Mathf.Atan2(invertedLookDirection.x, invertedLookDirection.z) * Mathf.Rad2Deg;
        }
        float pitch = Mathf.Asin(invertedLookDirection.y / invertedLookDirection.magnitude) * Mathf.Rad2Deg;
        
        Vector2 cameraRotationVector = new Vector2(yaw, pitch);

        return cameraRotationVector;
    }

    private void OnDrawGizmosSelected() {
        if(!drawGizmos) return;

        Gizmos.color = spawnGizmoColor;

        Vector3 translatedSpawn = transform.TransformPoint(transitionSpawn);
        Gizmos.DrawSphere(translatedSpawn, 0.05f);
        
        Gizmos.color = lookGizmoColor;

        Vector2 translatedLookRotationX = TranslateLookVectorX();

        Vector3 translatedLook = transform.TransformPoint(transitionSpawn + new Vector3(translatedLookRotationX.x, lookRotationY, translatedLookRotationX.y));
        Gizmos.DrawSphere(translatedLook, 0.05f);
    }
}