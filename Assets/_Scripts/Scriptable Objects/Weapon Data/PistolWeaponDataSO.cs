using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Pistol Weapon Data", fileName = "NewPistolWeaponDataSO")]
public class PistolWeaponDataSO : WeaponDataSO{
    [Header("Pistol Variables")]
    public int maxBounceCount = 3;   
}
