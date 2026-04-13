using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityUpgrade : MonoBehaviour
{
    public List<GameObject> meleeWeaponObjects;
    public List<GameObject> weaponThrustMeleeObjects;
    public List<GameObject> rangeWeaponObjects;


    public List<MeleeWeapon> meleeWeapons;  // Tham chiếu đến năng lực
    public List<ThrustMeleeWeapon> weaponThrustMelees; // Tham chiếu đến năng lực
    public List<RangeWeapon> rangeWeapons;  // Tham chiếu đến năng lực


    private Dictionary<string, GameObject> meleeWeaponDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> thrustWeaponDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> rangeWeaponDict = new Dictionary<string, GameObject>();

    private void Start()
    {
        for (int i = 0; i < rangeWeapons.Count; i++)
        {
            rangeWeaponDict[rangeWeapons[i].abilityName] = rangeWeaponObjects[i];
        }
        for (int i = 0; i < meleeWeapons.Count; i++)
        {
            meleeWeaponDict[meleeWeapons[i].abilityName] = meleeWeaponObjects[i];
        }
        for (int i = 0; i < weaponThrustMelees.Count; i++)
        {
            thrustWeaponDict[weaponThrustMelees[i].abilityName] = weaponThrustMeleeObjects[i];
        }
    }


    public void UnlockMeleeWeapon(string abilityName)
    {
        meleeWeaponDict.TryGetValue(abilityName, out var weapon);
        if (weapon != null && !weapon.activeSelf)
        {
            weapon.SetActive(true);
        }
    }
    public void UnlockThrustWeapon(string ability)
    {
       thrustWeaponDict.TryGetValue(ability, out var weapon);
        if (weapon != null && !weapon.activeSelf)
        {
            weapon.SetActive(true);
        }
    }

    public void UnlockRangeWeapon(string abilityName)
    {
        rangeWeaponDict.TryGetValue(abilityName, out var weapon);
        if (weapon != null && !weapon.activeSelf)
        {
            weapon.SetActive(true);
        }
    }
    //====================Melee Weapon Upgrades ===================
    public void UpgradeMeleeWeaponDamage(string abilityName, float damageIncrease)
    {
         GetMeleeWeaponByName(abilityName).IncreaseDamage(damageIncrease);
    }
    public void UpgradeMeleeWeaponRotationSpeed(string abilityName, float rotationSpeedIncrease)
    {
        GetMeleeWeaponByName(abilityName).IncreaseRotateSpeed(rotationSpeedIncrease);
    }

    //====================Thrust Weapon Upgrades ===================
    public void UpgradeThrustWeaponDamage(string abilityName, float damageIncrease)
    {
       GetThrustWeaponByName(abilityName).IncreaseDamage(damageIncrease);
    }
    public void UpgradeThrustWeaponCooldown(string abilityName, float cooldownReduction)
    {
       GetThrustWeaponByName(abilityName).ReduceCooldown(cooldownReduction);
    }

    //====================Range Weapon Upgrades ===================
    public void UpgradeRangeWeaponDamage(string abilityName, float damageIncrease)
    {
       GetThrustWeaponByName(abilityName).IncreaseDamage(damageIncrease);
    }
    public void UpgradeRangeWeaponBulletPerShot(string abilityName, int bulletIncrease)
    {
       GetRangeWeaponByName(abilityName).IncreaseBulletPerShot(bulletIncrease);
    }
    public void UpgradeRangeWeaponRange(string abilityName, float rangeIncrease)
    {
       GetRangeWeaponByName(abilityName).IncreaseRange(rangeIncrease);
    }

    public bool IsUnlocked(UpgradeOption option)
    {
        switch (option.type)
        {
            case AbilityUpgradeType.MeleeWeaponUnlock:
            case AbilityUpgradeType.MeleeWeaponDamageUp:
            case AbilityUpgradeType.MeleeWeaponRotationSpeed:
                return CheckLockedAbility(AbilityType.Melee, option.AbilityName);

            case AbilityUpgradeType.ThrustWeaponUnlock:
            case AbilityUpgradeType.ThrustWeaponDamageUp:
            case AbilityUpgradeType.ThrustWeaponReductionCooldown:
                return CheckLockedAbility(AbilityType.Thrust, option.AbilityName);

            case AbilityUpgradeType.RangeWeaponUnlock:
            case AbilityUpgradeType.RangeWeaponDamageUp:
            case AbilityUpgradeType.RangeWeaponBulletPerShot:
            case AbilityUpgradeType.RangeWeaponRangeIncrease:
                return CheckLockedAbility(AbilityType.Range, option.AbilityName);

            default:
                return false;
        }
    }

    public bool CheckLockedAbility(AbilityType abilityType, string abilityName)
    {
        switch (abilityType)
        {
            case AbilityType.Melee:
                meleeWeaponDict.TryGetValue(abilityName, out var weapon);
                if (weapon != null && weapon.activeSelf) return true;
                return false;
            case AbilityType.Thrust:
                thrustWeaponDict.TryGetValue(abilityName, out var thrustWeapon);
                if (thrustWeapon != null && thrustWeapon.activeSelf) return true;
                return false;
            case AbilityType.Range:
                rangeWeaponDict.TryGetValue(abilityName, out var rangeWeapon);
                if (rangeWeapon != null && rangeWeapon.activeSelf) return true;
                return false;
            default:
                return false;
        }
    }


    public MeleeWeapon GetMeleeWeaponByName(string abilityName)
    {
        foreach (var weapon in meleeWeapons)
        {
            if (weapon.abilityName == abilityName)
            {
                return weapon;
            }
        }
        return null;
    }
    public ThrustMeleeWeapon GetThrustWeaponByName(string abilityName)
    {
        foreach (var weapon in weaponThrustMelees)
        {
            if (weapon.abilityName == abilityName)
            {
                return weapon;
            }
        }
        return null;
    }

    public RangeWeapon GetRangeWeaponByName(string abilityName)
    {
        foreach (var weapon in rangeWeapons)
        {
            if (weapon.abilityName == abilityName)
            {
                return weapon;
            }
        }
        return null;
    }
}
