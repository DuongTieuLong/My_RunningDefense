using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject[] blocks;

    public event Action OnUpgradeSelected;

    private Dictionary<Button, UnityAction> buttonListeners = new();

    public CanvasGroup canvasGroup;
    public RectTransform rect;
    private void Start()
    {
       
    }
    private void Awake()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        rect = panel.GetComponent<RectTransform>();
    }

    public void ShowOptions(List<UpgradeOption> options, AbilityUpgrade abilityUpgrade)
    {
        panel.SetActive(true);

        Sequence currentSeq = DOTween.Sequence();
        currentSeq.SetUpdate(true);

        canvasGroup.alpha = 0;
        rect.localScale = new Vector3(0.8f, 0.8f, 1f);

        currentSeq.Append(rect.DOScale(Vector3.one, 0.4f).SetEase(Ease.InBack));
        currentSeq.Join(canvasGroup.DOFade(1, 0.5f));

   

        for (int i = 0; i < blocks.Length; i++)
        {
            if (i < options.Count)
            {
                var option = options[i];
                blocks[i].gameObject.SetActive(true);
                blocks[i].transform.Find("Title").GetComponent<TextMeshProUGUI>().text = option.AbilityName;
                blocks[i].transform.Find("Decription").GetComponent<TextMeshProUGUI>().text = option.description;
                blocks[i].transform.Find("FrameIcon").Find("Icon").GetComponent<Image>().sprite = option.icon;

                var button = blocks[i].GetComponentInChildren<Button>();
                SetupButton(button, option);
            }
            else
            {
                blocks[i].gameObject.SetActive(false);
            }
        }
    }

    void SetupButton(Button button, UpgradeOption option)
    {
        // Nếu button này đã có listener SelectUpgrade thì xóa đi
        if (buttonListeners.ContainsKey(button))
        {
            button.onClick.RemoveListener(buttonListeners[button]);
            buttonListeners.Remove(button);
        }

        // Tạo delegate mới và lưu lại
        UnityAction listener = () => SelectUpgrade(button, option);
        buttonListeners[button] = listener;

        // Gắn listener mới
        button.onClick.AddListener(listener);
    }

    public void SelectUpgrade(Button button, UpgradeOption option)
    {
        option.ApplyUpgrade();
        OnUpgradeSelected.Invoke();

        Sequence currentSeq = DOTween.Sequence();


        currentSeq.Append(rect.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.4f).SetEase(Ease.OutBack));
        currentSeq.Join(canvasGroup.DOFade(0, 0.5f)).onComplete  += () =>
        {
            panel.SetActive(false);
        };
    }


    void OnDisable()
    {
        buttonListeners.Clear();
    }
}
