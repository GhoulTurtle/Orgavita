using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Game Data/Codes/Integar Code", fileName = "NewIntegarCode")]
public class IntegarCodeSO : CodeSO{
    [Header("Integar Code Variables")]
    public int codeLength = 5;
    public string setCode;
    public string randomCode;

    [ContextMenu("Generate New Random Code")]
    public override void GenerateRandomCode(){
        char[] codeCharArray = new char[codeLength];

        for (int i = 0; i < codeLength; i++){
           codeCharArray[i] = GenerateRandomDigit();
        }

        string generatedRandomCode = new(codeCharArray);
        randomCode = generatedRandomCode;
    }

    private void OnEnable() {
        if(isRandom && generateRandomCodeOnPlay){
            GenerateRandomCode();
        }

        //Clamp set code to the code length
        char[] setCodeCharArray = setCode.ToCharArray();

        bool updatedCode = false;

        for (int i = 0; i < setCodeCharArray.Length; i++){
            if (!char.IsDigit(setCodeCharArray[i])){
                setCodeCharArray[i] = GenerateRandomDigit();
                updatedCode = true;
            }
        }

        if(!updatedCode) return;

        if(setCodeCharArray.Length < codeLength){
            //Add a random digit till the length equals the proper length
            char[] newSetCodeCharArray = new char[codeLength];

            Array.Copy(setCodeCharArray, newSetCodeCharArray, setCodeCharArray.Length);

            for (int i = setCodeCharArray.Length; i < codeLength; i++){
                newSetCodeCharArray[i] = GenerateRandomDigit();
            }

            string updatedSetCode = new(newSetCodeCharArray);
            setCode = updatedSetCode;

            return;
        }
        else if(setCodeCharArray.Length > codeLength){

            char[] truncatedArray = new char[codeLength];

            Array.Copy(setCodeCharArray, truncatedArray, codeLength);

            string clampedSetCode = new(truncatedArray);
            setCode = clampedSetCode;
            return;
        }
        else{
            string setCodeString = new(setCodeCharArray);
            setCode = setCodeString;
        }
    }

    public override string GetCurrentCode(){
        if(isRandom){
            return randomCode;
        }
        
        return setCode;
    }

    public override string GetSetCode(){
        return setCode;
    }

    public int GetCodeDigitLength(){
        return codeLength;
    }

    public override bool IsCodeCorrect(string attempt){
        if(attempt == GetCurrentCode()){
            return true;
        }
        
        return false;
    }

    private char GenerateRandomDigit(){
        int randomDigit = Random.Range(0, 10);
        //Insure the random digit is clamped between 0-9
        randomDigit = Mathf.Clamp(randomDigit, 0, 9);
        
        if(randomDigit == 0){
            return '0';
        }
        else{
            //ASCII value of 0 is 48 adding 1-9 to this value shifts it to the correct ASCII value
            return (char)(randomDigit + '0');
        }
    }
}