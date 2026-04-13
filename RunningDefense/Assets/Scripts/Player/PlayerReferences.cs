using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [SerializeField] public SupplyShowUI supplyUI;

    private void Awake()
    {
        supplyUI = GetComponentInChildren<SupplyShowUI>(true);
    }
}
