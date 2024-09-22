using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class TextPrinter{
    public static float DefaultTextPrintSpeed = 0.02f;
    
    public const float typeDivisible = 4f;

    public static event EventHandler OnTypeDivisible;
    private const string HMTL_ALPHA =  "<color=#00000000>";
    private static readonly List<Punctuation> punctuations = new List<Punctuation>(){
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.4f),
        new Punctuation(new HashSet<char>() {',', ';', ':'}, 0.2f)
    };

    public static void SetTextPrintSpeed(float newTextPrintSpeed){
        DefaultTextPrintSpeed = newTextPrintSpeed;
    }

    public static IEnumerator PrintSentence(string sentence, TextMeshProUGUI textObject, Action OnFinishedPrintingCallback, float textPrintSpeed = -1){
        if(textPrintSpeed == -1) textPrintSpeed = DefaultTextPrintSpeed;
        WaitForSeconds printTime = new WaitForSeconds(textPrintSpeed);

        textObject.text = "";
        string originalText = sentence;
        string displayedText = "";
        int alphaIndex = 0;
        
        char[] sentenceToPrint = sentence.ToCharArray();

        for (int i = 0; i < sentenceToPrint.Length; i++){
            alphaIndex++;

            // Only insert visible text part by part, text tags
            if (sentenceToPrint[i] == '<'){
                // Find the closing '>' of the tag and skip over the whole tag
                while (sentenceToPrint[i] != '>' && i < sentenceToPrint.Length - 1){
                    alphaIndex++;
                    i++;
                }
            }

            textObject.text = originalText;

            displayedText = textObject.text.Insert(alphaIndex, HMTL_ALPHA);
            
            textObject.text = displayedText;

            if(alphaIndex % typeDivisible == 0){
                OnTypeDivisible?.Invoke(null, EventArgs.Empty);
            }

            bool isLast = i >= sentence.Length - 1;

            if(IsPunctuation(sentenceToPrint[i], out float waitTime) && !isLast && !IsPunctuation(sentenceToPrint[i + 1], out _)){
                yield return new WaitForSeconds(waitTime);
            }
            else{
                yield return printTime;
            }
        }

        if(OnFinishedPrintingCallback != null){
            OnFinishedPrintingCallback?.Invoke();
        }
    }

    private static bool IsPunctuation(char character, out float waitTime){
        foreach (Punctuation punctuationCategory in punctuations){
            if (punctuationCategory.Punctuations.Contains(character)){
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }
    
    private struct Punctuation{
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime){
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }   
}

