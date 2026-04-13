using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public string WeaponID;
    public AbilityType abilityType;
    public string weaponName;

}

public enum AbilityType
{
    Melee,
    Thrust,
    Range
}

public enum AbilityUpgradeType
{
    MeleeWeaponUnlock,
    MeleeWeaponRotationSpeed,
    MeleeWeaponDamageUp,
    ThrustWeaponUnlock,
    ThrustWeaponDamageUp,
    ThrustWeaponReductionCooldown,
    RangeWeaponUnlock,
    RangeWeaponDamageUp,
    RangeWeaponBulletPerShot,
    RangeWeaponRangeIncrease,
}