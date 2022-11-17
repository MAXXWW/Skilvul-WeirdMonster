using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] Button startGameButton;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject RoomListObject;
    [SerializeField] GameObject PlayerListObject;
    [SerializeField] RoomItem roomItemPrefabs;
    [SerializeField] PlayerItem playerItemPrefabs;
    List<RoomItem> roomItemList = new List<RoomItem>();
    List<PlayerItem> playerItemList = new List<PlayerItem>();

    Dictionary<string, RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();

    public void Start()
    {
        PhotonNetwork.JoinLobby();
        roomPanel.SetActive(false);
    }

    public void ClickCreateRoom()
    {
        feedbackText.text = "";

        if (newRoomInputField.text.Length < 3)
        {
            feedbackText.text = "Room name min 3 character";
            return;
        }

        // create room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(newRoomInputField.text, roomOptions);
    }

    public void ClickStartGame(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(levelName);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Created room: {PhotonNetwork.CurrentRoom.Name}");
        feedbackText.text = $"Created room: {PhotonNetwork.CurrentRoom.Name}";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        feedbackText.text = $"Joined room: {PhotonNetwork.CurrentRoom.Name}";
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);

        // Update player list
        UpdatePlayerList();

        // atur start game button
        SetStartGameButton();

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Update player list
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // Update player list
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        // atur start game button
        SetStartGameButton();
    }

    private void SetStartGameButton()
    {
        // tampilkan hanya di master client
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // bisa diklik ketika jumlah player lebih dari 1
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;
    }

    private void UpdatePlayerList()
    {
        // destroy dulu semua player yang sudah ada
        foreach (var item in playerItemList)
        {
            Destroy(item.gameObject);
        }

        playerItemList.Clear();

        // foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        // {

        // }

        // bikin ulang player list
        foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefabs, PlayerListObject.transform);
            newPlayerItem.Set(player);
            playerItemList.Add(newPlayerItem);

            if (player == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.transform.SetAsFirstSibling();
            }
        }

        // hanya bisa diklik ketika jumlah pemain tertentu
        // jadi atur juga di sini
        SetStartGameButton();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + "," + message);
        feedbackText.text = returnCode.ToString() + ": " + message;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        Debug.Log("Room Updated");

        // destroy semua gameobject button
        foreach (var item in this.roomItemList)
        {
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear(); //membuat kosong lagi

        var roomInfoList = new List<RoomInfo>(roomInfoCache.Count);

        // create sesuai dengan room list yang baru

        // sort yang open di add duluan
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen)
            {
                roomInfoList.Add(roomInfo);
            }
        }

        // kemudian yang close
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen == false)
            {
                roomInfoList.Add(roomInfo);
            }
        }

        foreach (var roomInfo in roomInfoList)
        {
            if (roomInfo.IsVisible == false || roomInfo.MaxPlayers == 0)
            {
                continue;
            }

            RoomItem newRoomItem = Instantiate(roomItemPrefabs, RoomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            this.roomItemList.Add(newRoomItem);
        }
    }
}
