using System;
using UnityEngine;

public class UpgradeOption : ScriptableObject
{
    public string AbilityName;          // Tên nâng cấp (UI)
    public string description;   // Mô tả
    public Sprite icon;          // Icon trong UI
    public AbilityUpgradeType type;  // Loại năng lực

    public AbilityUpgrade abilityUpgrade; // Tham chiếu đến AbilityUpgrade để áp dụng nâng cấp

    public void Init(AbilityUpgrade abilityUpgrade)
    {
        this.abilityUpgrade = abilityUpgrade;
    }
    public virtual void ApplyUpgrade()
    {
      
    }
    public virtual void DescriptionUpdate()
    {

    }
}