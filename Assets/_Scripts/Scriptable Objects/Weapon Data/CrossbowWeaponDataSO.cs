using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Crossbow Weapon Data", fileName = "NewCrossbowWeaponDataSO")]
public class CrossbowWeaponDataSO : WeaponDataSO{
    [Header("Crossbow Variables")]
    public float explosionForce;
    public float explosionRadius;
    public float arrowSpeed;
}