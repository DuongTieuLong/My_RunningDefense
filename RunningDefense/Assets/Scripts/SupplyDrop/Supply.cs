using UnityEngine;
using Cysharp.Threading.Tasks;
public class Supply : MonoBehaviour
{
    private PlayerStats playerStats;
    public Transform towerTranform;
    private TowerHealth towerHeath;
    private TowerBoom towerBoom;
    public AudioClip pickSFX;

    private SupplyPoolingManager poolManager;
    private SupplyShowUI supplyShowUI;
    private float[] weights = { 0.5f, 0, 3, 0.2f }; // xác suất rơi



    public void Init(SupplyPoolingManager manager, float[] weights)
    {
        poolManager = manager;
        this.weights = weights;
    }


    private void Start()
    {
        playerStats = poolManager.playerStats;
        towerTranform = poolManager.towerTranform;
        towerBoom = poolManager.towerBoom;
        towerHeath = poolManager.towerHeath;
        supplyShowUI = poolManager.playerReferences.supplyUI;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickSFX != null)
                SoundManager.Instance.PlaySFX(pickSFX);
            CallRandomFunction();
            OnPicked();
        }

    }


    public void CallRandomFunction()
    {
        int index = GetWeightedRandomIndex(weights);

        switch (index)
        {
            case 0: HealingPlayer(); supplyShowUI.ShowSupplyIcon(poolManager.GetSupplyIcon(SupplyType.playerHeal), poolManager.showDuration); break;
            case 1: HealingTower(); supplyShowUI.ShowSupplyIcon(poolManager.GetSupplyIcon(SupplyType.towerHeal), poolManager.showDuration); break;
            case 2: TowerBoom(); supplyShowUI.ShowSupplyIcon(poolManager.GetSupplyIcon(SupplyType.towerBoom), poolManager.showDuration); break;
        }
    }

    private int GetWeightedRandomIndex(float[] weights)
    {
        float total = 0;
        foreach (float w in weights)
            total += w;

        float r = UnityEngine.Random.value * total;

        float sum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if (r <= sum)
                return i;
        }

        return 0;
    }

    public void HealingPlayer()
    {
        // hồi 30% máu tối đa
        float heal = playerStats.maxHealth * 0.3f;
        playerStats.Heal(heal);
    }
    public void HealingTower()
    {
        // hồi 30% máu tối đa
        float heal = towerHeath.MaxHealth * 0.3f;
        towerHeath.Regeneration(heal);
    }

    public void TowerBoom()
    {
        if (towerBoom == null) return;
        //Phát nổ
        towerBoom.Explode();
    }


    public void OnPicked()
    {
        poolManager.RePoolSupply(this);
    }
    public void Respawn()
    {
        poolManager.RePoolSupply(this);
        UniTask.Delay(1000);
        poolManager.SpawnSupply();


    }

}
public enum SupplyType { playerHeal, towerHeal, towerBoom }