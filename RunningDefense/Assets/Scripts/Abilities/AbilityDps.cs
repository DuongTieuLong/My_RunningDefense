using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDps : MonoBehaviour
{
    public List<MeleeWeapon> meleeWeapons = new List<MeleeWeapon>();
    public List<RangeWeapon> rangedWeapons = new List<RangeWeapon>();
    public List<ThrustMeleeWeapon> thrustMeleeWeapons = new List<ThrustMeleeWeapon>();

    public PlayerStats playerStats;

    public List<AbilityPauseInfo> abilityPauseInfos = new List<AbilityPauseInfo>();

    public List<AbilityPauseInfo> currentActiveAbility = new List<AbilityPauseInfo>();

    private void OnEnable()
    {
        playerStats.OnDeath += BackUpOnDeath;
    }

    private void OnDisable()
    {
        playerStats.OnDeath -= BackUpOnDeath;
    }

    public List<AbilityPauseInfo> GetSaveInfo()
    {
        if (playerStats.IsDead)
        {
            return currentActiveAbility;
        }
        else
        {
            return GetInfo();
        }
    }

    public void BackUpOnDeath()
    {
        currentActiveAbility = GetInfo();
    }

    public List<AbilityPauseInfo> GetInfo()
    {
        abilityPauseInfos.Clear();
        foreach (var melee in meleeWeapons)
        {
            if (melee != null && melee.isActiveAndEnabled)
            {
                AbilityPauseInfo info = new AbilityPauseInfo
                {
                    abilityName = melee.abilityName,
                    icon = melee.icon,
                    level = melee.GetAbilityLevel()
                };
                abilityPauseInfos.Add(info);
            }
        }
        foreach (var ranged in rangedWeapons)
        {
            if (ranged != null && ranged.isActiveAndEnabled)
            {
                AbilityPauseInfo info = new AbilityPauseInfo
                {
                    abilityName = ranged.abilityName,
                    icon = ranged.icon,
                    level = ranged.GetAbilityLevel()
                };
                abilityPauseInfos.Add(info);
            }
        }
        foreach (var thrust in thrustMeleeWeapons)
        {
            if (thrust != null && thrust.isActiveAndEnabled)
            {
                AbilityPauseInfo info = new AbilityPauseInfo
                {
                    abilityName = thrust.abilityName,
                    icon = thrust.icon,
                    level = thrust.GetAbilityLevel()
                };
                abilityPauseInfos.Add(info);
            }
        }

        return abilityPauseInfos;
    }

    public int GetPlayerLevel()
    {
        return playerStats.level;
    }   

    public float GetTotalDPS()
    {
        float totalDPS = 0f;
        foreach (var melee in meleeWeapons)
        {
            if (melee.isActiveAndEnabled == false)
                continue;
            if (melee != null && melee.GetRotateSpeed() > 0)
            {
                float attacksPerSecond = (playerStats.atkPoint + melee.GetDamage()) / (melee.GetRotateSpeed() / 360f * 2); 
                totalDPS += attacksPerSecond;
            }
        }
        foreach (var ranged in rangedWeapons)
        {
            if (ranged.isActiveAndEnabled == false)
                continue;
            if (ranged != null && ranged.cooldownTime > 0)
            {
                float attacksPerSecond = 1f / ranged.cooldownTime; // Số lần bắn mỗi giây
                totalDPS += (playerStats.atkPoint + ranged.GetDamage()) * ranged.GetBulletPerShot() * attacksPerSecond;
            }
        }
        foreach (var thrust in thrustMeleeWeapons)
        {
            if(thrust.isActiveAndEnabled == false) 
                continue;
            if (thrust != null && thrust.GetCooldown() > 0)
            {
                float attacksPerSecond = 1f / thrust.GetCooldown(); // Số lần đâm mỗi giây
                totalDPS += (playerStats.atkPoint + thrust.GetDamage()) * attacksPerSecond;
            }
        }
        return totalDPS;
    }

}

public class AbilityPauseInfo
{
    public string abilityName;
    public Sprite icon;
    public int level;   
}
