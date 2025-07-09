using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public Button btnSave;

    void Awake()
    {
        btnSave.onClick.AddListener(OnSaveClicked);
    }

    void OnEnable()
    {
        // Prefill with current name (if any)
        inputField.text = AuthenticationService.Instance.PlayerName ?? "";
    }

    async void OnSaveClicked()
    {
        string newName = inputField.text.Trim();
        if (string.IsNullOrEmpty(newName)) return;

        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            PlayerPrefs.SetString("CachedPlayerName", newName);    // local cache
            PlayerPrefs.Save();
            gameObject.SetActive(false);                           // close pop‑up
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Name set failed: {e.Message}");
        }
    }
}
