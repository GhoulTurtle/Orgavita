using UnityEngine;

[CreateAssetMenu(menuName = "Computer Application")]
public class ComputerApplication : ScriptableObject {
    public string Name;
    public Sprite Icon;
    public Sprite LoadingWindowSprite;
    public ApplicationUI ApplicationUI;
    public float LoadTime = 1.5f;
}