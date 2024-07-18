using UnityEngine;

public class FadeObjectSpawner : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private FadeObjectDataList fadeObjectPrefabDataList;

    public void SpawnRandomFadeObjects(Vector3 position, int objectsAmountToSpawn, bool addExplosiveForce = true, float explosionForce = 500f, float explosionRadius = 5f, float upwardModifier = 1f){
        if(fadeObjectPrefabDataList == null){
            Debug.LogWarning("No FadeObjectDataList set for fadeObjectPrefabDataList, did you forget to add it in the inspector?");
            return;
        }
        
        for (int i = 0; i < objectsAmountToSpawn; i++){
            FadeObject spawnObject = Instantiate(fadeObjectPrefabDataList.GetRandomFadeObject(), position, Random.rotation); 
            
            spawnObject.StartFadeTime();

            if(!addExplosiveForce) continue;

            if(spawnObject.TryGetComponent(out Rigidbody fadeObjectRigidbody)){
                fadeObjectRigidbody.AddExplosionForce(explosionForce, position, explosionRadius, upwardModifier);
            }
        }
    }
}