using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    public AudioClip customClip;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(PlaySound);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(PlaySound);
    }

    public void ReAddListener()
    {
        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (customClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(customClip);
        }
        else
        {
            SoundManager.Instance.PlayButtonSFX();
        }
    }
          
}
