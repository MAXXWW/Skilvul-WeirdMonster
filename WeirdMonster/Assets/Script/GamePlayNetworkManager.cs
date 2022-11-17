using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GamePlayNetworkManager : MonoBehaviourPunCallbacks
{
    public void BackToMainMenu()
    {
        StartCoroutine(BackToMainMenuCR());
    }

    IEnumerator BackToMainMenuCR()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null; //menunggu satu frame hingga nilainya berubah
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void BackToLobby()
    {
        StartCoroutine(BackToLobbyCR());
    }

    IEnumerator BackToLobbyCR()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom || PhotonNetwork.IsConnectedAndReady == false)
        {
            yield return null; //menunggu satu frame hingga nilainya berubah
        }

        SceneManager.LoadScene("Lobby");
    }

    public void Replay()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var scene = SceneManager.GetActiveScene();
            PhotonNetwork.LoadLevel(scene.name);
        }
    }

    public void Quit()
    {
        StartCoroutine(QuitCR());
    }

    IEnumerator QuitCR()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null; //menunggu satu frame hingga nilainya berubah
        }

        Application.Quit();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            BackToLobby();
        }
    }
}
