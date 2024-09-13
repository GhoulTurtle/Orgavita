using UnityEngine;

[CreateAssetMenu(menuName = "Computer Application/Basic Computer Application", fileName = "NewComputerApplication")]
public class ComputerApplication : ScriptableObject {
    [Header("Default Computer Application Variables")]
    public string Name;
    public Sprite Icon;
    public Sprite LoadingWindowSprite;
    public ApplicationUI ApplicationUI;
    public float LoadTime = 1.5f;
}