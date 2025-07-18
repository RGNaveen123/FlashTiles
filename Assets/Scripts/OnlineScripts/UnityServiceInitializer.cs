using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;

public class UnityServiceInitializer : MonoBehaviour
{
    [SerializeField] GameObject namePopup;          // drag the prefab in Inspector

    async Task SignInAsync()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // If the user never set a name, show the pop‑up
        if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))    
            namePopup.SetActive(true);
    }

    async void Awake()
    {
        await SignInAsync();          // ← call the helper so the panel can open
    }
}
