using UnityEngine;

public abstract class CodeSO : ScriptableObject{
    [Header("Base Code Variables")]
    public string codeName;
    public string codeDescription;

    [Header("Base Code Debug Variables")]
    public bool isRandom;
    public bool generateRandomCodeOnPlay;

    private void OnEnable() {
        if(generateRandomCodeOnPlay && isRandom){
            GenerateRandomCode();
        }
    }

    public abstract void GenerateRandomCode();

    public abstract string GetCurrentCode();

    public abstract string GetSetCode();

    public abstract bool IsCodeCorrect(string attempt);

    public virtual string GetCodeName(){
        return codeName;
    }

    public virtual string GetCodeDescription() { 
        return codeDescription;
    }
}
