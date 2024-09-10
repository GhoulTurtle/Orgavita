using System.Collections.Generic;

public struct TextContentProfile{
    public string textContent;
    public List<TextEffectDefinition> textEffectDefinitionList;

    public TextContentProfile(string _textContent, List<TextEffectDefinition> _textEffectDefinitionList){
        textContent = _textContent;
        textEffectDefinitionList = _textEffectDefinitionList;
   }
}
