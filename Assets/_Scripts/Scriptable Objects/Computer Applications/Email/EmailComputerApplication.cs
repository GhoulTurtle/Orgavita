using UnityEngine;

[CreateAssetMenu(menuName = "Computer Application/Email/Email Computer Application")]
public class EmailComputerApplication : ComputerApplication{
    [Header("Email Application Variables")]
    public EmailMessageListSO EmailMessageListSO;
    public float EmailLoadingTime = 0.5f;
}
