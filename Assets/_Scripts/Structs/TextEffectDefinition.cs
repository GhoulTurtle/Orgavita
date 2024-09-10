public struct TextEffectDefinition{
    public TextEffect textEffect;
    public int startIndex;
    public int endIndex;

    public TextEffectDefinition(TextEffect _textEffect, int _startIndex, int _endIndex){
        textEffect = _textEffect;
        startIndex = _startIndex;
        endIndex = _endIndex;
    }
}
