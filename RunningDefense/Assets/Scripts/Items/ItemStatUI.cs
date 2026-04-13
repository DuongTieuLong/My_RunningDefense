using System.Collections;
using TMPro;
using UnityEngine;
public class ItemStatUI : MonoBehaviour
{
    private BasePlayerStats baseStats;
    public ItemShopManager shopManager;

    // bonuses...
    private float helmetHPBonus;
    private float armorHPBonus;
    private float ringATKBonus;
    private float bootsdogdeCDBonus;
    private float bootsMoveSpeedBonus;

    public TextMeshProUGUI healthPointText;
    public TextMeshProUGUI attackPointText;
    public TextMeshProUGUI moveSpeedPointText;
    public TextMeshProUGUI dogdeCooldownText;

    void Awake()
    {
        if (shopManager == null)
        {
            shopManager = FindAnyObjectByType<ItemShopManager>();
        }
    }

    IEnumerator Start()
    {
        // đợi 1 frame để các singleton/scene object khác có thời gian khởi tạo
        yield return null;

        if (baseStats == null)
        {
            var statsObj = GameObject.FindWithTag("BasePlayerStats");
            if (statsObj != null)
                baseStats = statsObj.GetComponent<BasePlayerStats>();
        }

        RefreshStatUI();
    }
    public void RefreshStatUI()
    {

        // recompute bonuses
        GetBonusValue();
        ApplyItemStats();

        if (baseStats != null)
        {
            if (baseStats.maxHealth == baseStats.MaxHP)
                healthPointText.text = $"HP:\n{baseStats.maxHealth}";
            else
                healthPointText.text = $"HP:\n{baseStats.maxHealth} > <color=green>{baseStats.maxHealth + helmetHPBonus + armorHPBonus}</color>";

            if (baseStats.atkPoint == baseStats.ATK)
                attackPointText.text = $"ATK:\n{baseStats.atkPoint}";
            else
                attackPointText.text = $"ATK:\n{baseStats.atkPoint} > <color=green>{baseStats.atkPoint + ringATKBonus}</color>";

            if (baseStats.moveSpeed == baseStats.MoveSpeed)
                moveSpeedPointText.text = $"Speed:\n{baseStats.moveSpeed}";
            else
                moveSpeedPointText.text = $"Speed:\n{baseStats.moveSpeed} > <color=green>{baseStats.moveSpeed + bootsMoveSpeedBonus}</color>";

            if (baseStats.dogdeCooldown == baseStats.DodgeCooldown)
                dogdeCooldownText.text = $"DodgeCD:\n{baseStats.dogdeCooldown:F1}s";
            else
                dogdeCooldownText.text = $"DodgeCD:\n{baseStats.dogdeCooldown:F1}s > <color=green>{baseStats.dogdeCooldown - bootsdogdeCDBonus:F1}s</color>";


        }
        else
        {
            healthPointText.text = $"HP:\n - <color=green>(+{helmetHPBonus + armorHPBonus})</color>";
            attackPointText.text = $"ATK:\n - <color=green>(+{ringATKBonus})</color>";
            moveSpeedPointText.text = $"Speed:\n - <color=green>(+{bootsMoveSpeedBonus})</color>";
            dogdeCooldownText.text = $"DodgeCD:\n - <color=green>(-{bootsdogdeCDBonus:F1})</color>";
        }
    }

    /* backkup old version
        public void RefreshStatUI()
        {

            // recompute bonuses
            GetBonusValue();
            ApplyItemStats();

            if (baseStats != null)
            {
                healthPointText.text = $"Health:\n {baseStats.MaxHP} <color=green>(+{helmetHPBonus + armorHPBonus})</color>";
                attackPointText.text = $"Attack:\n {baseStats.ATK} <color=green>(+{ringATKBonus})</color>";
                moveSpeedPointText.text = $"Speed:\n {baseStats.MoveSpeed} <color=green>(+{bootsMoveSpeedBonus})</color>";
                dogdeCooldownText.text = $"DodgeCD:\n {baseStats.DodgeCooldown:F1}s <color=green>(-{bootsdogdeCDBonus:F1})</color>";
            }
            else
            {
                healthPointText.text = $"HP:\n - <color=green>(+{helmetHPBonus + armorHPBonus})</color>";
                attackPointText.text = $"ATK:\n - <color=green>(+{ringATKBonus})</color>";
                moveSpeedPointText.text = $"Speed:\n - <color=green>(+{bootsMoveSpeedBonus})</color>";
                dogdeCooldownText.text = $"DodgeCD:\n - <color=green>(-{bootsdogdeCDBonus:F1})</color>";
            }


        }*/


    public void ClearItemStats()
    {

        baseStats.ClearItemModifiers();
        helmetHPBonus = armorHPBonus = ringATKBonus = bootsdogdeCDBonus = bootsMoveSpeedBonus = 0f;
    }

    // tính tổng bonus từ currentEquipedItems (khởi tạo = 0, cộng dồn)
    public void GetBonusValue()
    {
        helmetHPBonus = armorHPBonus = ringATKBonus = bootsdogdeCDBonus = bootsMoveSpeedBonus = 0f;
        if (shopManager == null || shopManager.currentEquipedItems == null) return;

        foreach (var itemData in shopManager.currentEquipedItems)
        {
           // var itemData = itemUI
            if (itemData == null) continue;

            switch (itemData.ItemType)
            {
                case ItemType.Helmet:
                    var helmet = itemData as HelmetItemDataSO;
                    if (helmet != null) helmetHPBonus += helmet.healthValue;
                    break;
                case ItemType.Armor:
                    var armor = itemData as ArmorItemDataSO;
                    if (armor != null) armorHPBonus += armor.healthValue;
                    break;
                case ItemType.Boots:
                    var boots = itemData as BootsItemDataSO;
                    if (boots != null)
                    {
                        bootsMoveSpeedBonus += boots.speedValue;
                        bootsdogdeCDBonus += boots.dodgeCooldownValue;
                    }
                    break;
                case ItemType.Ring:
                    var ring = itemData as RingItemDataSO;
                    if (ring != null) ringATKBonus += ring.atkValue;
                    break;
            }
        }
    }

    public void ApplyItemStats()
    {

        baseStats.ClearItemModifiers();

        if (helmetHPBonus != 0f) baseStats.AddHPBonus(helmetHPBonus);
        if (armorHPBonus != 0f) baseStats.AddHPBonus(armorHPBonus);
        if (ringATKBonus != 0f) baseStats.AddATKBonus(ringATKBonus);
        if (bootsMoveSpeedBonus != 0f) baseStats.AddMoveSpeedBonus(bootsMoveSpeedBonus);
        if (bootsdogdeCDBonus != 0f) baseStats.AddDodgeCDBonus(-bootsdogdeCDBonus);
    }
}
