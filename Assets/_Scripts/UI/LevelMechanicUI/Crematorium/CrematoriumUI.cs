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

        for (int i = 0; i < sliderTitleText.Count; i++){
            sliderTitleText[i].text = "Heat Valve #" + i + 1 + " - Active";
        }
    
        UpdateNotificationText(false, false);
    }

    public void UpdateNotificationText(bool placedButton, bool placedCoolent){
        if(placedButton && placedCoolent){
            notificationText.text = readyAttentionText;
            return;
        }
        
        if(placedButton){
            notificationText.text = placedButtonOnlyAttentionText;
            return;
        }
        
        if(placedCoolent){
            notificationText.text = placedCoolentOnlyAttentionText;
            return;
        }
    
        notificationText.text = defaultAttentionText;
    }

    public void UpdateSliderTitleText(int currentIndex, bool isDraining, bool isResetting){
        for (int i = 0; i < sliderTitleText.Count; i++){
            if(i == currentIndex){
                if(isDraining){
                    sliderTitleText[i].text = "Heat Valve #" + i + 1 + " - Cooling";
                    continue;
                }

                if(isResetting){
                    sliderTitleText[i].text = "Heat Valve #" + i + 1 + " - Heating";
                    continue;
                }
            }

            if(i < currentIndex){
                sliderTitleText[i].text = "Heat Valve #" + i + 1 + " - Deactive";
                continue;
            }

            sliderTitleText[i].text = "Heat Valve #" + i + 1 + " - Active";
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
