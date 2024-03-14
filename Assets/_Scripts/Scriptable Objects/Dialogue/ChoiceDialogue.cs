using System;

[Serializable]
public class ChoiceDialogue : Dialogue{
    public event EventHandler OnChoiceChosen;

    public void ChooseChoice(){
        OnChoiceChosen?.Invoke(this, EventArgs.Empty);
    }
}
