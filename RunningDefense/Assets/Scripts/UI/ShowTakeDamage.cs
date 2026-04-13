using System.Collections;
using TMPro;
using UnityEngine;

public class ShowTakeDamage : MonoBehaviour
{
    public TextMeshProUGUI damageText; // gán trong Inspector
    public float showTime = 0.3f;      // thời gian hiển thị

    private Coroutine showRoutine;
    private Color baseColor;

    void Awake()
    {
        if (damageText == null)
        {
            return;
        }

        baseColor = damageText.color;
        baseColor.a = 0f;               // ẩn hoàn toàn
        damageText.color = baseColor;
    }

    public void ShowDamage(float dmg)
    {
        if (damageText == null) return;

        int val = Mathf.Max(0, Mathf.RoundToInt(dmg));
        damageText.text = $"-{val}";

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        // hiện text (alpha = 1)
        Color c = damageText.color;
        c.a = 1f;
        damageText.color = c;

        // giữ trong showTime giây
        yield return new WaitForSeconds(showTime);

        // ẩn text (alpha = 0)
        c.a = 0f;
        damageText.color = c;

        showRoutine = null;
    }
}
