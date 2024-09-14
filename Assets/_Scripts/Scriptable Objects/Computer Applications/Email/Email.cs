using System;
using UnityEngine;

[Serializable]
public class Email{
    public string Subject;
    public string Sender;
    public string Recipient;
    [Space, TextArea(4, 4)]
    public string Body;
    public bool saveNote;
    public NoteSO noteToSave;
}
