using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Assault Rifle Weapon Data", fileName = "NewAssualtRifleWeaponDataSO")]
public class AssaultRifleWeaponDataSO : WeaponDataSO{
    [Header("Assault Rifle Variables")]
    [Range(1, 60)] public int burstShotAmount;
    public float burstDamageAmount;
    public float burstKickBackAmount;
    public float burstShotRateCooldownInSeconds;
    public float burstFireRateInSeconds;
}