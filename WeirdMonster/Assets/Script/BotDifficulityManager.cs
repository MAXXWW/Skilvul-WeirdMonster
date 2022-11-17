using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using System;

public class BotDifficulityManager : MonoBehaviour
{
    [SerializeField] Bot bot;
    [SerializeField] int selectedDifficulity;
    [SerializeField] BotStats[] botDifficulties;

    [Header("Remote Config Parameters:")]
    [SerializeField] bool enableRemoteConfig = false;
    [SerializeField] string difficultyKey = "Difficulty";
    struct userAtrributes { };
    struct appAtrributes { };

    IEnumerator Start()
    {
        // tunggu bot selesai setup
        yield return new WaitUntil(() => bot.IsReady);

        // set stats default dari difficulty manager
        // sesuai selected Difficulty dari inspector
        var newStats = botDifficulties[selectedDifficulity];
        bot.SetStats(newStats, true);

        // ambil difficulty dari remote config kalau enabled
        if (enableRemoteConfig == false)
        {
            yield break;
        }

        // tapi tunggu hingga unity service siap
        yield return new WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn);

        // daftar dulu untuk event fetch complate
        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;
        // lalu fetch di sini, cukup sekali di awal permainan
        RemoteConfigService.Instance.FetchConfigs(new userAtrributes(), new appAtrributes());
    }

    private void OnDestroy()
    {
        // unregister event untuk menghindari memory leak
        RemoteConfigService.Instance.FetchCompleted -= OnRemoteConfigFetched;
    }

    // setiap kali data baru didapatkan (melalui fetch) fungsi ini akan dipanggil
    private void OnRemoteConfigFetched(ConfigResponse response)
    {
        if (RemoteConfigService.Instance.appConfig.HasKey(difficultyKey) == false)
        {
            Debug.LogWarning($"Difficulty Key: {difficultyKey} not found on remote config server");
            return;
        }

        switch (response.requestOrigin)
        {
            case ConfigOrigin.Default:
            case ConfigOrigin.Cached:
                break;
            case ConfigOrigin.Remote:
                selectedDifficulity = RemoteConfigService.Instance.appConfig.GetInt(difficultyKey);
                selectedDifficulity = Mathf.Clamp(selectedDifficulity, 0, botDifficulties.Length - 1);
                var newStats = botDifficulties[selectedDifficulity];
                bot.SetStats(newStats, true);
                break;
        }
    }
}
