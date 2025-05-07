using UnityEngine;

public class LightBulb : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private MeshRenderer lightGlassMeshRenderer;
    [SerializeField] private Material lightGlassOffMaterial;
    [SerializeField] private Material lightGlassOnMaterial;
    [SerializeField] private Light pointLight;
    [SerializeField] private LayerMask playerLayer;

    [Header("Light Bulb Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private float distanceCheck = 10f;
    
    [Header("Gizmos Setttings")]
    [SerializeField] private bool showGizmos;
    [SerializeField] private Color gizmosColor = Color.yellow;

    private int frameCheck;
    private int frameCounter  = 0;

    private Collider[] playerCollider = new Collider[1];

    private void Awake(){
        if(isOn){
            ActivateLight();
        }
        else{
            DeactivateLight();
        }

        frameCheck = Random.Range(10, 31);
    }

    private void Update(){
        frameCounter++;
        if(frameCounter != frameCheck) return;

        CheckPlayer();
    }

    private void CheckPlayer(){
        frameCounter = 0;
        
        if(!isOn) return;
        
        if(Physics.OverlapSphereNonAlloc(transform.position, distanceCheck, playerCollider, playerLayer) == 0){
            pointLight.enabled = false;
        }
        else{
            pointLight.enabled = true;
        }
    }

    public void ActivateLight(){
        lightGlassMeshRenderer.material = lightGlassOnMaterial;
        pointLight.enabled = true;
        isOn = true;
        CheckPlayer();
    }

    public void DeactivateLight(){
        lightGlassMeshRenderer.material = lightGlassOffMaterial;
        pointLight.enabled = false;
        isOn = false;
    }

    private void OnDrawGizmosSelected(){
        if(!showGizmos) return; 

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, distanceCheck);
    }
}
