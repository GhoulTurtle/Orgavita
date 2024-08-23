using System;
using System.Collections;
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
    [SerializeField] private Image popupWindowBackgroundImage;
    [SerializeField] private Transform windowPopupParent;
    [SerializeField] private TextMeshProUGUI popupWindowText;
    [SerializeField] private GameObject popupWindowBorder;
    [SerializeField] private Button popupWindowButton;
    [SerializeField] private TextMeshProUGUI popupWindowButtonText;
    [SerializeField] private GameObject extraPopupWindowBorder;
    [SerializeField] private Button extraPopupWindowButton;
    [SerializeField] private TextMeshProUGUI extraPopupWindowButtonText;
    [SerializeField] private float popupWindowAnimationTimeInSeconds = 0.5f;

    [Header("Popup Error Window References")]
    [SerializeField] private Image[] popupWindowImageArray = new Image[2];
    [Header("Loading Window References")]
    [SerializeField] private LoadingBarUI popupLoadingBarUI;
    [Header("Information Window References")]
    [SerializeField] private TMP_InputField informationInputField; 

    [Header("Card Printer Status Variables")]
    [SerializeField] private Color invalidPrintColor = Color.red;
    [SerializeField] private Color validPrintColor = Color.green;
    [SerializeField] private Sprite invalidPrintIcon;
    [SerializeField] private Sprite validPrintIcon;

    [Header("Card Window Wait Variables")]
    [SerializeField] private float ejectCardWindowOpenTimeInSeconds = 1.5f;
    [SerializeField] private float createCardWindowOpenTimeInSeconds = 0.5f;
    [SerializeField] private float upgradeCardWindowOpenTimeInSeconds = 0.5f;

    [Header("Card Loading Bar Wait Variables")]
    [SerializeField] private float minEjectTimeInSeconds = 1.5f;
    [SerializeField] private float maxEjectTimeInSeconds = 3f;
    [SerializeField] private float minCreateCardTimeInSeconds;
    [SerializeField] private float maxCreateCardTimeInSeconds;
    [SerializeField] private float minUpgradeCardTimeInSeconds;
    [SerializeField] private float maxUpgradeCardTimeInSeconds;    

    private CardPrinter cardPrinter;

    public Action OnCreateNewCard;
    public Action OnUpgradeCard;
    public Action OnEjectCard;

    public CardPrinterPopupState cardPrinterPopupState = CardPrinterPopupState.None;

    private void Awake() {
        windowPopupParent.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        StopAllCoroutines();
        popupWindowButton.onClick.RemoveAllListeners();
        extraPopupWindowButton.onClick.RemoveAllListeners();
        popupLoadingBarUI.OnFinishAction -= LoadingBarFinished;
    }

    public override void CloseApplication(){
        base.CloseApplication();
    }

    //Update the cardPrinterPopupState in these methods
    public void AttemptCreateNewCard(){
        if(cardPrinter == null || cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_No_Card) return;
        StartCoroutine(ApplicationLoadCoroutine(createCardWindowOpenTimeInSeconds, OnCreateNewCard));
    }

    public void AttemptUpgradeCard(){
        if(cardPrinter == null || cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_No_Card) return;
        StartCoroutine(ApplicationLoadCoroutine(upgradeCardWindowOpenTimeInSeconds, OnUpgradeCard));
    }

    public void AttemptEjectCard(){
        if(cardPrinter == null || cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_No_Card) return;
        cardPrinterPopupState = CardPrinterPopupState.Eject_Popup_Card_Menu;
        StartCoroutine(ApplicationLoadCoroutine(ejectCardWindowOpenTimeInSeconds, UpdateCardPopupUI));
    }

    public void SetupCardPrinterUI(CardPrinter _cardPrinter){
        cardPrinter = _cardPrinter;
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
                cardPrinterStatusText.text = "WARNING: No secruity strip found on card.";
                break;
        }

        cardPrinterStatusBackgroundImage.color = invalidPrintColor;
        for (int i = 0; i < cardPrinterStatusIconImageArray.Length; i++){
            cardPrinterStatusIconImageArray[i].sprite = invalidPrintIcon;
        }
    }

    private void UpdateCardPopupUI(){
        switch (cardPrinterPopupState){
            case CardPrinterPopupState.None:
                windowPopupParent.gameObject.SetActive(false);
                return;
            case CardPrinterPopupState.Create_Card_Menu:
                break;
            case CardPrinterPopupState.Upgrade_Card_Menu:
                break;
            case CardPrinterPopupState.Eject_Popup_Card_Menu:
                popupWindowBackgroundImage.color = validPrintColor;
                for (int i = 0; i < popupWindowImageArray.Length; i++){
                    popupWindowImageArray[i].sprite = validPrintIcon;
                    popupWindowImageArray[i].gameObject.SetActive(false);
                }

                popupLoadingBarUI.gameObject.SetActive(true);
                popupLoadingBarUI.OnFinishAction += LoadingBarFinished;
                popupLoadingBarUI.StartLoading(minEjectTimeInSeconds, maxEjectTimeInSeconds);

                informationInputField.gameObject.SetActive(false);

                popupWindowText.text = "Ejecting Card...";
                popupWindowButton.interactable = false;
                popupWindowButtonText.text = "OK";
                extraPopupWindowBorder.SetActive(false);
                popupWindowButton.onClick.AddListener(CloseCardPopupUI);
                break;
            case CardPrinterPopupState.Create_Card_Error_Invalid_Card:
                break;
            case CardPrinterPopupState.Create_Card_Error_Used_Card:
                break;
            case CardPrinterPopupState.Upgrade_Card_Error_Not_Available:
                break;
            case CardPrinterPopupState.Upgrade_Card_Error_Max_Level:
                break;
        }

        //Play window popup animation
        windowPopupParent.gameObject.SetActive(true);

        //After the window popup animation start the loading bar

    }

    private void CloseCardPopupUI(){
        popupWindowButton.onClick.RemoveAllListeners();
        extraPopupWindowButton.onClick.RemoveAllListeners();

        //Play window popup animation
        windowPopupParent.gameObject.SetActive(false);
        createNewCardButton.gameObject.SetActive(true);
        upgradeCardSecurityLevelButton.gameObject.SetActive(true);
        ejectCardButton.gameObject.SetActive(true);
    }

    private void LoadingBarFinished(){
        Debug.Log("FINISHED!");

        switch (cardPrinterPopupState){
            case CardPrinterPopupState.None:
                break;
            case CardPrinterPopupState.Create_Card_Menu:
                break;
            case CardPrinterPopupState.Create_Card_Error_Invalid_Card:
                break;
            case CardPrinterPopupState.Create_Card_Error_Used_Card:
                break;
            case CardPrinterPopupState.Upgrade_Card_Menu:
                break;
            case CardPrinterPopupState.Upgrade_Card_Error_Not_Available:
                break;
            case CardPrinterPopupState.Upgrade_Card_Error_Max_Level:
                break;
            case CardPrinterPopupState.Eject_Popup_Card_Menu:
                popupWindowText.text = "Card Successfully Ejected.";            
                for (int i = 0; i < popupWindowImageArray.Length; i++){
                    popupWindowImageArray[i].gameObject.SetActive(true);
                }
                popupWindowButton.interactable = true;
                popupLoadingBarUI.OnFinishAction -= LoadingBarFinished;
                break;
        }
    }

    protected override void DisableApplicationUIInteractivity(){
        base.DisableApplicationUIInteractivity();
    }

    protected override void EnableApplicationUIInteractivity(){
        base.EnableApplicationUIInteractivity();
    }

    private IEnumerator ApplicationLoadCoroutine(float amount, Action actionToTrigger){
        DisableApplicationUIInteractivity();
        computerUI.UpdateCursor(ComputerCursorState.Loading);
        yield return new WaitForSeconds(amount);
        computerUI.UpdateCursor(ComputerCursorState.Default);
        EnableApplicationUIInteractivity();
        actionToTrigger?.Invoke();
    }
}