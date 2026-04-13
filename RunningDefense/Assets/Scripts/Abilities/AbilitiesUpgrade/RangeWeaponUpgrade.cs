using UnityEngine;
[CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "UpgradeOptions/RangeWeaponUpgrade")]
public class RangeWeaponUpgrade : UpgradeOption
{
    public float damageIncreaseAmount = 3f;
    public float rangeIncreaseAmount = 2f;
    public int bulletPerShotIncreaseAmount = 1;

    RangeWeapon range;

    public override void ApplyUpgrade()
    {
        range = abilityUpgrade.GetRangeWeaponByName(AbilityName);
        switch (type)
        {
            case AbilityUpgradeType.RangeWeaponUnlock:
                abilityUpgrade.UnlockRangeWeapon(AbilityName);
                break;
            case AbilityUpgradeType.RangeWeaponDamageUp:
                range.IncreaseDamage(damageIncreaseAmount);
                break;
            case AbilityUpgradeType.RangeWeaponBulletPerShot:
                range.IncreaseBulletPerShot(bulletPerShotIncreaseAmount);
                break;
            case AbilityUpgradeType.RangeWeaponRangeIncrease:
                range.IncreaseRange(rangeIncreaseAmount);
                break;
        }
    }

    public override void DescriptionUpdate()
    {
        range = abilityUpgrade.GetRangeWeaponByName(AbilityName);
        switch (type)
        {
            case AbilityUpgradeType.RangeWeaponUnlock:
                description = $"Unlock {AbilityName} Weapon Shot enemy in range";
                break;
            case AbilityUpgradeType.RangeWeaponDamageUp:
                description = $"Increase Damage\n{range.GetDamage()} " +
                    $"> {range.GetDamage() + damageIncreaseAmount}";
                break;
            case AbilityUpgradeType.RangeWeaponBulletPerShot:
                description = $"Increase BulletPerShot\n{range.GetBulletPerShot()} " +
                    $"> {range.GetBulletPerShot() + bulletPerShotIncreaseAmount}";
                break;
            case AbilityUpgradeType.RangeWeaponRangeIncrease:
                description = $"Increase Range\n{range.GetRange()} " +
                    $"> {range.GetRange() + rangeIncreaseAmount}";
                break;
        }
    }
}
