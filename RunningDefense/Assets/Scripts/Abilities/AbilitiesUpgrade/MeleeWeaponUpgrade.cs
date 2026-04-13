using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "UpgradeOptions/MeleeWeaponUpgrade", order = 1)]
public class MeleeWeaponUpgrade : UpgradeOption
{
    public float damageIncreaseAmount = 3f;
    public float rotationSpeedIncreaseAmount = 20f;

    MeleeWeapon melee;
    public override void ApplyUpgrade()
    {
        melee = abilityUpgrade.GetMeleeWeaponByName(AbilityName);
        switch (type)
        {
            case AbilityUpgradeType.MeleeWeaponUnlock:
                abilityUpgrade.UnlockMeleeWeapon(AbilityName);
                break;
            case AbilityUpgradeType.MeleeWeaponDamageUp:
                abilityUpgrade.UpgradeMeleeWeaponDamage(AbilityName, damageIncreaseAmount);
                description = $"Increase Damage: {melee.GetDamage()} > {melee.GetDamage() + damageIncreaseAmount}";
                break;
            case AbilityUpgradeType.MeleeWeaponRotationSpeed:
                abilityUpgrade.UpgradeMeleeWeaponRotationSpeed(AbilityName, rotationSpeedIncreaseAmount);
                description = $"Increase RotationSpeed: {melee.GetRotateSpeed()} > {melee.GetRotateSpeed() + damageIncreaseAmount}";
                break;
        }
    }

    public override void DescriptionUpdate()
    {
        melee = abilityUpgrade.GetMeleeWeaponByName(AbilityName);
        switch (type)
        {
            case AbilityUpgradeType.MeleeWeaponUnlock:
                description = $"Unlock {AbilityName}, that rotate around the character";
                break;
            case AbilityUpgradeType.MeleeWeaponDamageUp:
                description = $"Increase Damage\n {melee.GetDamage()} " +
                    $"> {melee.GetDamage() + damageIncreaseAmount}";
                break;
            case AbilityUpgradeType.MeleeWeaponRotationSpeed:
                description = $"Increase RotationSpeed \n {melee.GetRotateSpeed()} " +
                    $">  {melee.GetRotateSpeed() + rotationSpeedIncreaseAmount}";
                break;
        }
    }
}
