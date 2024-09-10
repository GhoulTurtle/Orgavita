using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TextParser{
    //effectRegex looks for <effect=X> and </effect> tags, helping us find where to start and stop applying text effects. It also extracts the effect name (e.g., "wobble") for later use.
    private static Regex effectRegex = new Regex(@"<effect=([\w.]+)>|<\/effect>", RegexOptions.Compiled);

    private const string EFFECT_TAG_OPENER = "<effect=";
    private const string EFFECT_TAG_CLOSER = "</effect>";

    
    //codeRegex looks for <code> tags with optional parameters (startchar and charcount). It captures the file path to load the correct CodeSO and the starting character and character count to insert the appropriate portion of the code into the output text.
    private static Regex codeRegex = new Regex(@"<code=([^,<>]+)(?:,\s*startchar=(-?\d+))?(?:,\s*charcount=(\d+))?>", RegexOptions.Compiled);
    
    public static TextContentProfile Parse(string inputString, out List<TextEffectDefinition> textEffectDefinitionList){
        string outputString = ParseCode(inputString);
        outputString = ParseEffects(outputString, out textEffectDefinitionList);

        for (int i = 0; i < textEffectDefinitionList.Count; i++){
            Debug.Log(textEffectDefinitionList[i].textEffect + " Starting Index: " + textEffectDefinitionList[i].startIndex + " Ending Index: " + textEffectDefinitionList[i].endIndex);
        }

        TextContentProfile textContentProfile = new TextContentProfile(outputString, textEffectDefinitionList);

        return textContentProfile;
    }

    private static string ParseCode(string inputString){
        MatchCollection matches = codeRegex.Matches(inputString);

        for (int i = 0; i < matches.Count; i++){
            if(!matches[i].Success) continue;

            string filePath = matches[i].Groups[1].Value;
            int startChar = matches[i].Groups[2].Success ? int.Parse(matches[i].Groups[2].Value) : 0;
            int charCount = matches[i].Groups[3].Success ? int.Parse(matches[i].Groups[3].Value) : 0;

            CodeSO codeSO = Resources.Load<CodeSO>(filePath);

            if(codeSO == null){
                inputString = inputString.Replace(matches[i].Value, "");
                continue;
            }

            string fullCode = codeSO.GetCurrentCode();

            if(startChar < 0) startChar = 0;

            string codeToInsert = charCount > 0 ? fullCode.Substring(startChar, Mathf.Min(charCount, fullCode.Length - startChar)) : fullCode.Substring(startChar);

            inputString = inputString.Replace(matches[i].Value, codeToInsert);
        }

        return inputString;
    }

    private static string ParseEffects(string inputString, out List<TextEffectDefinition> textEffectDefinitionList){
        Stack<TextEffect> textEffectStack = new Stack<TextEffect>();
        textEffectDefinitionList = new List<TextEffectDefinition>();

        int indexOffset = 0;
        int startIndex = -1;

        MatchCollection matches = effectRegex.Matches(inputString);

        for (int i = 0; i < matches.Count; i++){
            if(!matches[i].Success) continue;

            if(matches[i].Value.StartsWith(EFFECT_TAG_OPENER)){
                string effectName = matches[i].Groups[1].Value;

                if(Enum.TryParse(effectName, out TextEffect textEffect)){
                    textEffectStack.Push(textEffect);
                    startIndex = matches[i].Index - indexOffset;
                }

                inputString = inputString.Remove(matches[i].Index - indexOffset, matches[i].Length);
                indexOffset += matches[i].Length;
            }
            else if(matches[i].Value.StartsWith(EFFECT_TAG_CLOSER)){
                if(textEffectStack.Count > 0){
                    TextEffect textEffect = textEffectStack.Pop();
                    int endIndex = matches[i].Index - indexOffset;

                    textEffectDefinitionList.Add(new TextEffectDefinition(textEffect, startIndex, endIndex));

                    startIndex = -1;
                }

                inputString = inputString.Remove(matches[i].Index - indexOffset, matches[i].Length);
                indexOffset += matches[i].Length;
            }
        }

        return inputString;
    }
}
