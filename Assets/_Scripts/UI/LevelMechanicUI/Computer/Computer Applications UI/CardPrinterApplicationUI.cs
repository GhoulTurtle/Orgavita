using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPrinterApplicationUI : ApplicationUI{
    [Header("Button References")]
    [SerializeField] private GameObject createNewCardBorder;
    [SerializeField] private GameObject upgradeCardSecurityLevelBorder;
    [SerializeField] private GameObject ejectCardBorder;
    
    [Header("Printer Status References")]
    [SerializeField] private TextMeshProUGUI cardPrinterStatusText;
    [SerializeField] private Image[] cardPrinterStatusIconImageArray = new Image[2];
    [SerializeField] private Image cardPrinterStatusBackgroundImage;
    
    [Header("Base Popup Window References")]
    [SerializeField] private CanvasGroup popupWindowCanvasGroup;
    [SerializeField] private Image popupWindowBackgroundImage;
    [SerializeField] private Transform windowPopupParent;
    [SerializeField] private TextMeshProUGUI popupWindowText;
    [SerializeField] private GameObject popupWindowButtonBorder;
    [SerializeField] private Button popupWindowButton;
    [SerializeField] private TextMeshProUGUI popupWindowButtonText;
    [SerializeField] private GameObject closePopupWindowButtonBorder;
    [SerializeField] private Button closePopupWindowButton;
    [SerializeField] private TextMeshProUGUI closePopupWindowButtonText;

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
    [SerializeField] private float minEjectTimeInSeconds = 0.5f;
    [SerializeField] private float maxEjectTimeInSeconds = 2f;
    [SerializeField] private float minCreateCardTimeInSeconds;
    [SerializeField] private float maxCreateCardTimeInSeconds;
    [SerializeField] private float minUpgradeCardTimeInSeconds;
    [SerializeField] private float maxUpgradeCardTimeInSeconds;    
    [SerializeField] private float minSearchTimeInSeconds = 2f;
    [SerializeField] private float maxSearchTimeInSeconds = 3f;

    [Header("Card Popup Animation Variables")]
    [SerializeField] private float popupWindowAnimationTimeInSeconds = 0.5f;
    [SerializeField] private Vector2 popupWindowOpenScale = Vector2.one;
    [SerializeField] private Vector2 popupWindowCloseScale = Vector2.zero;
    [SerializeField] private Vector2 popupWindowOpenOffset;

    private Vector2 popupWindowClosePos;
    private Vector2 popupWindowOpenPos;

    private CardPrinter cardPrinter;

    private IEnumerator currentLoadingCoroutine;
    private IEnumerator currentPopupStretchCoroutine;
    private IEnumerator currentPopupAlphaCoroutine;
    private IEnumerator currentPopupLerpCoroutine;

    public CardPrinterPopupState cardPrinterPopupState = CardPrinterPopupState.None;
    private CardPrinterSearchResultType cardPrinterSearchResultType = CardPrinterSearchResultType.None;

    private void Awake() {
        popupWindowClosePos = windowPopupParent.localPosition;
        popupWindowOpenPos = popupWindowClosePos + popupWindowOpenOffset;
    }

    private void OnDestroy() {
        StopAllCoroutines();
        StopCurrentPopupAnimation();
        popupWindowButton.onClick.RemoveAllListeners();
        closePopupWindowButton.onClick.RemoveAllListeners();
        popupLoadingBarUI.OnFinishAction -= LoadingBarFinished;
        popupLoadingBarUI.OnFinishAction -= SearchingFinished;
    }

    public override void CloseApplication(){
        base.CloseApplication();
    }

    //Update the cardPrinterPopupState in these methods
    public void AttemptCreateNewCard(){
        if (cardPrinter == null) return;
        
        if(cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_No_Card){
            cardPrinterPopupState = CardPrinterPopupState.Invalid_Print_No_Card;
        }
        else if(cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_Wrong_Card){
            cardPrinterPopupState = CardPrinterPopupState.Invalid_Print_Wrong_Card;
        }
        else if(!cardPrinter.CanCreateNewCard()){
            cardPrinterPopupState = CardPrinterPopupState.Create_Card_Error_Used_Card;
        }
        else{
            cardPrinterPopupState = CardPrinterPopupState.Create_Card_Menu;
        }

        StopCurrentLoadingCoroutine();
        currentLoadingCoroutine = ApplicationLoadCoroutine(createCardWindowOpenTimeInSeconds, UpdateCardPopupUI);
        StartCoroutine(currentLoadingCoroutine);
    }

    public void AttemptUpgradeCard(){
        if(cardPrinter == null) return;
        
        if(!cardPrinter.CanUpgradeCards()){
            cardPrinterPopupState = CardPrinterPopupState.Upgrade_Card_Error_Not_Available; 
        }
        else if(cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_No_Card){
            cardPrinterPopupState = CardPrinterPopupState.Invalid_Print_No_Card;
        }
        else if(cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_Wrong_Card){
            cardPrinterPopupState = CardPrinterPopupState.Invalid_Print_Wrong_Card;
        }
        else if(!cardPrinter.CanUpgradeCard()){
            cardPrinterPopupState = CardPrinterPopupState.Upgrade_Card_Error_Max_Level;
        }
        else if(cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Valid_Print){
            cardPrinterPopupState = CardPrinterPopupState.Upgrade_Card_Menu;
        }
        
        StopCurrentLoadingCoroutine();
        currentLoadingCoroutine = ApplicationLoadCoroutine(upgradeCardWindowOpenTimeInSeconds, UpdateCardPopupUI);
        StartCoroutine(currentLoadingCoroutine);
    }

    public void AttemptEjectCard(){
        if(cardPrinter == null) return;
        
        if(cardPrinter.GetCardPrinterStatus() == CardPrinterStatus.Invalid_Print_No_Card){
            cardPrinterPopupState = CardPrinterPopupState.Invalid_Print_No_Card;
        }
        else{
            cardPrinterPopupState = CardPrinterPopupState.Eject_Popup_Card_Menu;
        }

        StopCurrentLoadingCoroutine();
        currentLoadingCoroutine = ApplicationLoadCoroutine(ejectCardWindowOpenTimeInSeconds, UpdateCardPopupUI);
        StartCoroutine(currentLoadingCoroutine);
    }

    private void StopCurrentLoadingCoroutine(){
        if (currentLoadingCoroutine != null){
            StopCoroutine(currentLoadingCoroutine);
            currentLoadingCoroutine = null;
        }
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

    public void InputFieldUpdated(string input){
        if(input.Length == informationInputField.characterLimit){
            //enable the submit button
            popupWindowButton.interactable = true;
        }
        else{
            popupWindowButton.interactable = false;
        }
    }

    private void UpdateCardPopupUI(){
        switch (cardPrinterPopupState){
            case CardPrinterPopupState.None:
                popupWindowButton.onClick.RemoveAllListeners();
                closePopupWindowButton.onClick.RemoveAllListeners();

                //Play close animation
                StartClosePopupAnimation();

                SetActiveMenuButtons(true);
                return;
            case CardPrinterPopupState.Create_Card_Menu:
                PopupMenuWindowUI(false, "Enter employee code into the text box below. A employee code should be a 5 digit code with numbers only.");
                popupLoadingBarUI.gameObject.SetActive(false);
                informationInputField.gameObject.SetActive(true);

                informationInputField.text = "";
                popupWindowButton.interactable = false;

                SetPopupButtons(true, "SUBMIT", true, "BACK");
                closePopupWindowButton.onClick.AddListener(CloseCardPopupUI);

                popupWindowButton.onClick.AddListener(SearchForCard);
                break;
            case CardPrinterPopupState.Upgrade_Card_Menu:

                break;
            case CardPrinterPopupState.Eject_Popup_Card_Menu:
                PopupMenuWindowUI(false, "Ejecting Card...");

                popupLoadingBarUI.gameObject.SetActive(true);
                popupLoadingBarUI.OnFinishAction += LoadingBarFinished;
                popupLoadingBarUI.StartLoading(minEjectTimeInSeconds, maxEjectTimeInSeconds);

                informationInputField.gameObject.SetActive(false);
                
                popupWindowButton.onClick.AddListener(CloseCardPopupUI);

                SetPopupButtons(false, "OK");
                break;
            case CardPrinterPopupState.Create_Card_Error_Used_Card:
                ErrorPopupWindowUI("ERROR C04: Card is already in use. Insert a Blank Security Card.");
                break;
            case CardPrinterPopupState.Upgrade_Card_Error_Not_Available:
                ErrorPopupWindowUI("ERROR C03: No upgrade card module installed in connected printer. Contact your card printer manufacturer if this is incorrect.");
                break;
            case CardPrinterPopupState.Upgrade_Card_Error_Max_Level:
                ErrorPopupWindowUI("ERROR C02: Card detected as max security level. Contact administration if this is incorrect.");
                break;
            case CardPrinterPopupState.Invalid_Print_Wrong_Card:
                ErrorPopupWindowUI("ERROR C01: No Security Strip detected on card. Attach Security Strip onto the card before attempting any operations.");
                break;
            case CardPrinterPopupState.Invalid_Print_No_Card:
                ErrorPopupWindowUI("ERROR C00: No card detected in connected card printer. Insert card into connected printer before attempting any operations.");
                break;
        }
        SetActiveMenuButtons(false);

        //Play window popup animation
        StartPopupAnimation();
    }

    private void ErrorPopupWindowUI(string popupText){
        popupWindowBackgroundImage.color = invalidPrintColor;
        for (int i = 0; i < popupWindowImageArray.Length; i++){
            popupWindowImageArray[i].sprite = invalidPrintIcon;
            popupWindowImageArray[i].gameObject.SetActive(true);
        }

        popupLoadingBarUI.gameObject.SetActive(false);
        informationInputField.gameObject.SetActive(false);

        popupWindowText.text = popupText;
                
        popupWindowButton.onClick.AddListener(CloseCardPopupUI);
        SetPopupButtons(true, "OK");
    }

    private void PopupMenuWindowUI(bool popupIconsActive, string popupText){
        popupWindowBackgroundImage.color = validPrintColor;
        for (int i = 0; i < popupWindowImageArray.Length; i++){
            popupWindowImageArray[i].sprite = validPrintIcon;
            popupWindowImageArray[i].gameObject.SetActive(popupIconsActive);
        }

        popupWindowText.text = popupText;
    }

    private void CloseCardPopupUI(){
        popupWindowButton.interactable = true;
        closePopupWindowButton.interactable = true;
        cardPrinterPopupState = CardPrinterPopupState.None;
        UpdateCardPopupUI();
    }

    private void LoadingBarFinished(){
        popupLoadingBarUI.OnFinishAction -= LoadingBarFinished;

        switch (cardPrinterPopupState){
            case CardPrinterPopupState.Create_Card_Menu:
                PopupMenuWindowUI(true, "Card Successfully Created.");
                popupWindowButtonBorder.SetActive(true);
                cardPrinter.CreateNewCard();
                break;
            case CardPrinterPopupState.Upgrade_Card_Menu:
                break;
            case CardPrinterPopupState.Eject_Popup_Card_Menu:
                PopupMenuWindowUI(true, "Card Successfully Ejected.");
                popupWindowButtonBorder.SetActive(true);
                cardPrinter.EjectHeldItem();
                break;
        }
    }

    private void SearchForCard(){
        //Update the popup window to be in the searching state
        SetPopupButtons(false, "OK");

        popupWindowButton.onClick.RemoveAllListeners();
        popupWindowText.text = "Searching...";

        informationInputField.gameObject.SetActive(false);

        popupLoadingBarUI.gameObject.SetActive(true);
        popupLoadingBarUI.OnFinishAction += SearchingFinished;
        popupLoadingBarUI.StartLoading(minSearchTimeInSeconds, maxSearchTimeInSeconds);

        //Get the results for the search
        cardPrinterSearchResultType = cardPrinter.AttemptCreateNewCard(informationInputField.text);
    }

    private void SearchingFinished(){
        popupLoadingBarUI.OnFinishAction -= SearchingFinished;

        //Update UI based on result type and the current popup state
        if(cardPrinterPopupState == CardPrinterPopupState.Create_Card_Menu){
            switch (cardPrinterSearchResultType){
                case CardPrinterSearchResultType.None:
                    CloseCardPopupUI();
                    return;
                case CardPrinterSearchResultType.Invalid_Code:
                    ErrorPopupWindowUI("ERROR C05: Invalid employee code inputted. Contact administration if this is incorrect.");
                    return;
                case CardPrinterSearchResultType.Code_In_Use:
                    ErrorPopupWindowUI("ERROR C06: Code is already in use by another employee. Contact administration if this is incorrect.");
                    return;
                case CardPrinterSearchResultType.Valid_Code:
                    PopupMenuWindowUI(true, "Valid Code Found!");
                    popupLoadingBarUI.gameObject.SetActive(false);
                    informationInputField.gameObject.SetActive(false);
                    popupWindowButton.onClick.AddListener(StartCardUpgrade);
                    closePopupWindowButton.onClick.AddListener(CloseCardPopupUI);

                    SetPopupButtons(true, "Print", true, "Back");
                    return;
            }
        }

        if(cardPrinterPopupState == CardPrinterPopupState.Upgrade_Card_Menu){
            switch (cardPrinterSearchResultType){
                case CardPrinterSearchResultType.None:
                    CloseCardPopupUI();
                    return;
                case CardPrinterSearchResultType.Invalid_Code:
                    return;
                case CardPrinterSearchResultType.Code_In_Use:
                    return;
                case CardPrinterSearchResultType.Valid_Code:
                    return;
            }
        }
    }

    private void StartCardUpgrade(){
        popupWindowButton.onClick.RemoveAllListeners();
        closePopupWindowButton.onClick.RemoveAllListeners();

        popupWindowButton.onClick.AddListener(CloseCardPopupUI);
        popupLoadingBarUI.gameObject.SetActive(true);

        SetPopupButtons(false, "OK");
        switch (cardPrinterPopupState){
            case CardPrinterPopupState.Create_Card_Menu:         
                PopupMenuWindowUI(false, "Creating New Card, Please Wait...");
                popupLoadingBarUI.StartLoading(minCreateCardTimeInSeconds, maxCreateCardTimeInSeconds);
                break;
            case CardPrinterPopupState.Upgrade_Card_Menu:
                PopupMenuWindowUI(false, "Upgrading Card Level, Please Wait...");
                popupLoadingBarUI.StartLoading(minUpgradeCardTimeInSeconds, minUpgradeCardTimeInSeconds);
                break;
        }

        popupLoadingBarUI.OnFinishAction += LoadingBarFinished;
    }

    private void SetPopupButtons(bool popupButtonState, string popupButtonText, bool closePopupButtonState = false, string closePopupButtonText = "Back"){
        popupWindowButtonText.text = popupButtonText;
        popupWindowButtonBorder.SetActive(popupButtonState);

        closePopupWindowButtonText.text = closePopupButtonText;
        closePopupWindowButtonBorder.SetActive(closePopupButtonState);
    }

    private void SetActiveMenuButtons(bool state){
        createNewCardBorder.SetActive(state);
        upgradeCardSecurityLevelBorder.SetActive(state);
        ejectCardBorder.SetActive(state);
    }

    private void StartPopupAnimation(){
        StopCurrentPopupAnimation();

        currentPopupStretchCoroutine = UIAnimator.StretchAnimationCoroutine(windowPopupParent, popupWindowOpenScale, popupWindowAnimationTimeInSeconds, false);
        currentPopupAlphaCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(popupWindowCanvasGroup, popupWindowAnimationTimeInSeconds, 0, 1);
        currentPopupLerpCoroutine = UIAnimator.LerpingAnimationCoroutine(windowPopupParent, popupWindowOpenPos, popupWindowAnimationTimeInSeconds, false);

        StartCoroutine(currentPopupStretchCoroutine);
        StartCoroutine(currentPopupAlphaCoroutine);
        StartCoroutine(currentPopupLerpCoroutine);
    }

    private void StartClosePopupAnimation(){
        StopCurrentPopupAnimation();

        currentPopupStretchCoroutine = UIAnimator.StretchAnimationCoroutine(windowPopupParent, popupWindowCloseScale, popupWindowAnimationTimeInSeconds, false);
        currentPopupAlphaCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(popupWindowCanvasGroup, popupWindowAnimationTimeInSeconds, 1, 0);
        currentPopupLerpCoroutine = UIAnimator.LerpingAnimationCoroutine(windowPopupParent, popupWindowClosePos, popupWindowAnimationTimeInSeconds, false);

        StartCoroutine(currentPopupStretchCoroutine);
        StartCoroutine(currentPopupAlphaCoroutine);
        StartCoroutine(currentPopupLerpCoroutine);
    }

    private void StopCurrentPopupAnimation(){
        if(currentPopupStretchCoroutine != null){
            StopCoroutine(currentPopupStretchCoroutine);
            currentPopupStretchCoroutine = null;
        }

        if(currentPopupAlphaCoroutine != null){
            StopCoroutine(currentPopupAlphaCoroutine);
            currentPopupAlphaCoroutine = null;
        }
        
        if(currentPopupLerpCoroutine != null){
            StopCoroutine(currentPopupLerpCoroutine);
            currentPopupLerpCoroutine = null;
        }
    }

    private IEnumerator ApplicationLoadCoroutine(float amount, Action actionToTrigger){
        DisableApplicationUIInteractivity();
        computerUI.UpdateCursor(ComputerCursorState.Loading);
        yield return new WaitForSeconds(amount);
        computerUI.UpdateCursor(ComputerCursorState.Default);
        EnableApplicationUIInteractivity();
        currentLoadingCoroutine = null;
        actionToTrigger?.Invoke();
    }
}