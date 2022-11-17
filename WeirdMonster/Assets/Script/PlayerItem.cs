using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;
    [SerializeField] TMP_Text playerName;

    public void Set(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.TryGetValue("AvatarIndex", out var value))
        {
            avatarImage.sprite = avatarSprites[(int)value];
        }

        playerName.text = player.NickName;

        if (player == PhotonNetwork.MasterClient)
        {
            playerName.text += " (Master)";
        }
    }
}
