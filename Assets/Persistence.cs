using UnityEngine;

public class Persistence : MonoBehaviour{
    public static Persistence Instance { get; private set;}

    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else{
            Destroy(gameObject);
            return;
        }
    }
}