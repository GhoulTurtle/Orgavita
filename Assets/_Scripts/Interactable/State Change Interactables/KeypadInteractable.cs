using System;
using UnityEngine;
using UnityEngine.Events;

public class KeypadInteractable : StateChangeInteractable{
    public override string InteractionPrompt => "Enter Code";

    [Header("Keypad Variables")]
    [SerializeField] private int maxPasswordSize = 4;
    [SerializeField] private string password;
    [SerializeField] private UnityEvent unlockEvent;
    [SerializeField] private UnityEvent lockEvent;

    public event EventHandler<KeypadNumberEnteredEventArgs> OnNumberEntered;
    public event EventHandler OnKeypadCleared;
    public event EventHandler OnCorrectPasswordEntered;
    public event EventHandler OnIncorrectPasswordEntered;

    public class KeypadNumberEnteredEventArgs : EventArgs{
        public string Entry;
        public KeypadNumberEnteredEventArgs(string _entry){
            Entry = _entry;
        }
    }

    private string currentEntry = "";
    private int previousCharPos = 0;
    private char incomingChar = new();

    public override void EnterState(){
        base.EnterState();
    }

    public override void ExitState(object sender, PlayerInputHandler.InputEventArgs e){
        base.ExitState(sender, e);
    }

    public void NumButtonPressed(int number){
        incomingChar = number switch{
            0 => '0',
            1 => '1',
            2 => '2',
            3 => '3',
            4 => '4',
            5 => '5',
            6 => '6',
            7 => '7',
            8 => '8',
            9 => '9',
            _ => ' ',
        };

        UpdateAnswerPanel(incomingChar);
    }

    private void UpdateAnswerPanel(char num){
            if(num == ' '){
                Debug.Log("Invalid number put into the NumButtonPressed Function...");
                return;
            }

            //check the char count of current entry
            int charCount = 0;
            foreach (char character in currentEntry){
                charCount++;
            }

            if(charCount >= maxPasswordSize){
                //get the next char pos from the previousCharPos
                int nextCharPos = previousCharPos + 1;

                if(nextCharPos >= maxPasswordSize){
                    nextCharPos = 0;
                }

                char[] currentCharArray = currentEntry.ToCharArray();

                if(currentCharArray[nextCharPos] == num){
                    //same char
                    return;
                }

                //replace the number after previous char pos
                currentCharArray[nextCharPos] = num;
                currentEntry = new string(currentCharArray);

                //set the next char as the previous char
                previousCharPos = nextCharPos;
            }
            else{
                //add the char to the current string
                currentEntry += num;

                previousCharPos = currentEntry.Length - 1;
            }
            
            OnNumberEntered?.Invoke(this, new KeypadNumberEnteredEventArgs(currentEntry));
        }

    public void ClearNumber(){
        currentEntry = "";
        previousCharPos = -1;
        OnKeypadCleared?.Invoke(this, EventArgs.Empty);
        lockEvent?.Invoke();
        OnIncorrectPasswordEntered?.Invoke(this, EventArgs.Empty);
    }

    public void EnterNumber(){
        if(currentEntry == password){
            unlockEvent?.Invoke();
            OnCorrectPasswordEntered?.Invoke(this, EventArgs.Empty);
        }
        else{
            lockEvent?.Invoke();
            OnIncorrectPasswordEntered?.Invoke(this, EventArgs.Empty);
        }
    }
}
