using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Shotgun Weapon Data", fileName = "NewShotgunWeaponDataSO")]
public class ShotgunWeaponDataSO : WeaponDataSO{
    [Header("Shotgun Variables")]
    [Range(1, 60)] public int pelletsPerShot;
}
