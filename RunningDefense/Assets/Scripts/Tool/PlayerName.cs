using UnityEngine;
using TMPro;
public class PlayerName : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TextMeshProUGUI notification;
    public void SavePlayerName()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            notification.text = "Name cannot be empty!";
            return;
        }
        if (playerName.Length > 12)
        {
            notification.text = "Name is too long! Max 12 characters.";
            return;
        }
        PlayerPrefs.SetString("PlayerName", playerName); PlayerPrefs.Save(); gameObject.SetActive(false);
    }

    [ContextMenu("ResetPlayerName")]
    public void ResetPlayerName()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.Save();
    }
}