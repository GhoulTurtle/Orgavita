using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;

    [Header("UI References")]
    [SerializeField] private GameObject playerDotCrosshair;
    [SerializeField] private List<GameObject> playerCrossCrosshair = new List<GameObject>();

    private EquippedItemBehaviour equippedItemBehaviour;

    private void Start() {
        playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += EvaulateWeapon;
        playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += WeaponDespawned;
    }

    private void WeaponDespawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        if(equippedItemBehaviour != null){
            //Unsub here
            equippedItemBehaviour = null;
            playerDotCrosshair.SetActive(true);
            foreach (GameObject crosshairGameobject in playerCrossCrosshair){
                crosshairGameobject.SetActive(false);
            }
        } 
    }

    private void EvaulateWeapon(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        equippedItemBehaviour = e.equippedItemBehaviour;
        //Sub here
        playerDotCrosshair.SetActive(false);
        foreach (GameObject crosshairGameobject in playerCrossCrosshair){
            crosshairGameobject.SetActive(true);
        }
    }
}
