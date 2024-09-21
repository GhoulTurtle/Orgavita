using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrematoriumUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private List<Slider> sliders = new List<Slider>();
    [SerializeField] private List<Image> pressZoneImages = new List<Image>(); 

    [SerializeField] private List<TextMeshProUGUI> sliderTitleText = new List<TextMeshProUGUI>();
    [SerializeField] private TextMeshProUGUI notificationText;
    [Header("Attention Text Strings")]
    [SerializeField] private string defaultAttentionText;
    [SerializeField] private string placedButtonOnlyAttentionText;
    [SerializeField] private string placedCoolentOnlyAttentionText;
    [SerializeField] private string readyAttentionText = "Crematorium cooling is ready...";
    [SerializeField] private string openingDoorAttentionText;
    [SerializeField] private string finishedAttentionText;

    [Header("Press Zones Colors")]
    [SerializeField] private Color pressZonesDefaultColor = Color.yellow;
    [SerializeField] private Color pressZonesSuccessColor = Color.green;
    [SerializeField] private Color pressZonesFailedColor = Color.red;

    private void Awake() {
        for (int i = 0; i < sliders.Count; i++){
            sliders[i].value = 0;
        }

        for (int i = 0; i < pressZoneImages.Count; i++){
            pressZoneImages[i].color = pressZonesDefaultColor;
        }
    
        UpdateSliderTitleText(0, false, false);
        UpdateNotificationText(NotificationTextMessage.Default);
    }

    public void UpdateNotificationText(NotificationTextMessage notificationTextMessage){
        switch (notificationTextMessage){
            case NotificationTextMessage.Default: notificationText.text = defaultAttentionText;
                break;
            case NotificationTextMessage.No_Coolant: notificationText.text = placedButtonOnlyAttentionText;
                break;
            case NotificationTextMessage.No_Button: notificationText.text = placedCoolentOnlyAttentionText;
                break;
            case NotificationTextMessage.Ready: notificationText.text = readyAttentionText;
                break;
            case NotificationTextMessage.Opening_Door: notificationText.text = openingDoorAttentionText;
                break;
            case NotificationTextMessage.Finished: notificationText.text = finishedAttentionText;
                break;
        }
    }

    public void UpdateSliderTitleText(int currentIndex, bool isDraining, bool isResetting){
        for (int i = 0; i < sliderTitleText.Count; i++){
            int sliderNumber = i + 1;

            if(i == currentIndex){
                if(isDraining){
                    sliderTitleText[i].text = "Heat Valve #" + sliderNumber + " - Cooling";
                    continue;
                }

                if(isResetting){
                    sliderTitleText[i].text = "Heat Valve #" + sliderNumber + " - Heating";
                    continue;
                }
            }

            if(i < currentIndex){
                sliderTitleText[i].text = "Heat Valve #" + sliderNumber + " - Deactive";
                continue;
            }

            sliderTitleText[i].text = "Heat Valve #" + sliderNumber + " - Active";
        }
    }

    public void UpdatePressZone(Image pressZoneImage, PressZoneResultType pressZoneResultType){
        switch (pressZoneResultType){
            case PressZoneResultType.Default: pressZoneImage.color = pressZonesDefaultColor;
                break;
            case PressZoneResultType.Success: pressZoneImage.color = pressZonesSuccessColor;
                break;
            case PressZoneResultType.Failed: pressZoneImage.color = pressZonesFailedColor;
                break;
        }
    }

    public Slider GetSliderByIndex(int index){
        return sliders[index];
    }
}
