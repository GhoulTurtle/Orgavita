using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPrinterApplicationUI : ApplicationUI{
    [Header("Button References")]
    [SerializeField] private Button createNewCardButton;
    [SerializeField] private Button upgradeCardSecurityLevelButton;
    [SerializeField] private Button ejectCardButton;
    [Header("Printer Status References")]
    [SerializeField] private TextMeshProUGUI cardPrinterStatusText;
    [SerializeField] private Image[] cardPrinterStatusIconImageArray = new Image[2];
    [SerializeField] private Image cardPrinterStatusBackgroundImage;
    [Header("Base Popup Window References")]
    [SerializeField] private Transform windowPopupParent;
    [SerializeField] private TextMeshProUGUI windowPopupText;
    [SerializeField] private Button closePopupWindowButton;
    [SerializeField] private float popupWindowAnimationTimeInSeconds = 0.5f;

    [Header("Popup Error Window References")]
    [SerializeField] private Image[] errorPopupWindowImageArray = new Image[2];
    [Header("Loading Window References")]
    //Loading Bar Reference
    [Header("Information Window References")]
    [SerializeField] private InputField informationInputField; 

    [Header("Card Printer Status Variables")]
    [SerializeField] private Color invalidPrintColor = Color.red;
    [SerializeField] private Color validPrintColor = Color.green;
    [SerializeField] private Sprite invalidPrintIcon;
    [SerializeField] private Sprite validPrintIcon;

    public Action OnCreateNewCard;
    public Action OnUpgradeCard;
    public Action OnEjectCard;

    public override void CloseApplication(){
        base.CloseApplication();
    }

    public void AttemptCreateNewCard(){
        OnCreateNewCard?.Invoke();
    }

    public void AttemptUpgradeCard(){
        OnUpgradeCard?.Invoke();
    }

    public void AttemptEjectCard(){
        OnEjectCard?.Invoke();
    }

    public void UpdateCardPrinterStatusUI(CardPrinterStatus currentStatus){
        switch (currentStatus){
            case CardPrinterStatus.Valid_Print:
                cardPrinterStatusText.text = "Valid Card Inserted.";
                cardPrinterStatusBackgroundImage.color = validPrintColor;
                for (int i = 0; i < cardPrinterStatusIconImageArray.Length; i++){
                    cardPrinterStatusIconImageArray[i].sprite = validPrintIcon;
                }
                return;
            case CardPrinterStatus.Invalid_Print_No_Card:
                cardPrinterStatusText.text = "Insert card to begin editing.";
                break;
            case CardPrinterStatus.Invalid_Print_Wrong_Card:
                cardPrinterStatusText.text = "ERROR: No valid secruity strip found on card.";
                break;
        }

        cardPrinterStatusBackgroundImage.color = invalidPrintColor;
        for (int i = 0; i < cardPrinterStatusIconImageArray.Length; i++){
            cardPrinterStatusIconImageArray[i].sprite = invalidPrintIcon;
        }
    }
}
