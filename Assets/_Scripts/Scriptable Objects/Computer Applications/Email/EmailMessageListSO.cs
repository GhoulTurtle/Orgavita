using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Computer Application/Email/Email Messages", fileName = "NewEmailMessageList")]
public class EmailMessageListSO : ScriptableObject{
    public List<Email> EmailMessageList;
}
