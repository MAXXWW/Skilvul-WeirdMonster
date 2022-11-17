using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;

public class GameServiceInitialization : MonoBehaviour
{
    [SerializeField] string environmentName;

    async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Uninitialized)
        {
            return;
        }

        var options = new InitializationOptions();
        options.SetEnvironmentName(environmentName);
        await UnityServices.InitializeAsync(options);

        if (AuthenticationService.Instance.IsSignedIn == false)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
