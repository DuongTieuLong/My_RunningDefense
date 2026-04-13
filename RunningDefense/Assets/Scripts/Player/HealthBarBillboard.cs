using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HealthBarBillboard : MonoBehaviour
{
    public Slider heathBar;

    public void UpdateHealthBar(float healthPercent)
    {
        if (heathBar != null)
        {
            heathBar.value = healthPercent;
        }
    }


}
