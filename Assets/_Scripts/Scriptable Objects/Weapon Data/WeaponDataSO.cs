using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data", fileName = "NewWeaponDataSO")]
public class WeaponDataSO : ScriptableObject{
    [Header("Mechanic Variables")]
    [Tooltip("The rate the weapon will shoot in seconds.")]
    public float weaponFireRateInSeconds;
    [Tooltip("The time it takes to reload in seconds.")]
    public float weaponReloadTimeInSeconds;
    [Tooltip("The amount of damage dealt to a damageable object.")]
    public float weaponAttackDamage;
    [Tooltip("The time it takes to aim in if the weapon has aim functionality.")]
    public float aimSpeedInSeconds;
    [Tooltip("The base bloom angle that the weapon will use when doing bloom calculations. 0 is perfect accuracy.")]
    public float baseBloomAngle;
    [Tooltip("The bloom angle that is used when the weapon is steadied or aimed. 0 is perfect accuracy.")]
    public float steadiedBloomAngle;
    [Tooltip("The minimum amount of shots that are needed to be fired in the kick back start window to begin the kickback of the weapon.")]
    public int minKickBackShotAmount;
    [Tooltip("The time window in seconds that the minimum kickback shot amount must be fired to trigger the kickback amount.")]
    public float kickBackStartWindowInSeconds;
    [Tooltip("The amount of kickback that is applied to the Y input of the player. 0 is no kickback added.")]
    public float kickBackAmount;

    [Header("Visual Variables")]
    [Tooltip("The amount of camera shake that is applied every shot that the weapon is fired. 0 is no camera shake.")]
    public float cameraShakeAmplitudeOnFired;
    [Tooltip("The amount the crosshair will animate from the center of the screen on weapon fired.")]
    public float shootCrosshairMagnitude;
    [Tooltip("The animation time for the shoot crosshair animation.")]
    public float shootCrosshairAnimationTimeInSeconds;
}