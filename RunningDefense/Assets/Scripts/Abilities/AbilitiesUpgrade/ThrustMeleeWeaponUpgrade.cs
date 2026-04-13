using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "UpgradeOptions/ThrustMeleeWeaponUpgrade")]
public class ThrustMeleeWeaponUpgrade : UpgradeOption
{
    public float damageIncreaseAmount = 3f;
    public float cooldownReductionAmount = 0.1f;

    ThrustMeleeWeapon thrust;

    public override void ApplyUpgrade()
    {
        thrust = abilityUpgrade.GetThrustWeaponByName(AbilityName);
        switch (type)
        {
            case AbilityUpgradeType.ThrustWeaponUnlock:
                abilityUpgrade.UnlockThrustWeapon(AbilityName);
                break;
            case AbilityUpgradeType.ThrustWeaponDamageUp:
                abilityUpgrade.UpgradeThrustWeaponDamage(AbilityName, damageIncreaseAmount);
                description = $"Increase Damage: {thrust.GetDamage()} > {thrust.GetDamage() + damageIncreaseAmount}";
                break;
            case AbilityUpgradeType.ThrustWeaponReductionCooldown:
                abilityUpgrade.UpgradeThrustWeaponCooldown(AbilityName, cooldownReductionAmount);
                description = $"Reduce Cooldown: {thrust.GetCooldown()} > {thrust.GetCooldown()+ cooldownReductionAmount}";
                break;
        }
    }
    public override void DescriptionUpdate()
    {
        thrust = abilityUpgrade.GetThrustWeaponByName(AbilityName);
        switch (type)
        {
            case AbilityUpgradeType.ThrustWeaponUnlock:
                description = $"Unlock {AbilityName} thrust enemy in range";
                break;
            case AbilityUpgradeType.ThrustWeaponDamageUp:
                description = $"Increase Damage\n{thrust.GetDamage()} " +
                    $"> {thrust.GetDamage() + damageIncreaseAmount}";
                break;
            case AbilityUpgradeType.ThrustWeaponReductionCooldown:
                description = $"Reduce Cooldown\n{thrust.GetCooldown()} " +
                    $"> {thrust.GetCooldown() - cooldownReductionAmount}";
                break;
        }
    }

}
