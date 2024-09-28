[System.Serializable]
public struct FadeTime{
    public float FadeInTime;
    public float FadeOutTime;

    public FadeTime(float _fadeInTime, float _fadeOutTime){
        FadeInTime = _fadeInTime;
        FadeOutTime = _fadeOutTime;
    }    
}
