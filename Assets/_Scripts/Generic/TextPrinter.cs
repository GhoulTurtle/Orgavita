using System;
using System.Collections;
using TMPro;
using UnityEngine;

public static class TextPrinter{
    public static float DefaultTextPrintSpeed = 0.02f;
    
    public const float typeDivisible = 4f;

    public static event EventHandler OnTypeDivisible;

    public static void SetTextPrintSpeed(float newTextPrintSpeed){
        DefaultTextPrintSpeed = newTextPrintSpeed;
    }

    public static IEnumerator PrintSentence(string sentence, TextMeshProUGUI textObject, Action OnFinishedPrintingCallback, float textPrintSpeed = -1){
        if(textPrintSpeed == -1) textPrintSpeed = DefaultTextPrintSpeed;
        WaitForSeconds printTime = new WaitForSeconds(textPrintSpeed);

        textObject.text = "";
        char[] characters = sentence.ToCharArray();
        
        for (int i = 0; i < characters.Length; i++){
            textObject.text += characters[i];
            
            if(i % typeDivisible == 0){
                OnTypeDivisible?.Invoke(null, EventArgs.Empty);
            }
            yield return printTime;
        }

        if(OnFinishedPrintingCallback != null){
            OnFinishedPrintingCallback?.Invoke();
        }
    }    
}
