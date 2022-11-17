using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;
    private int SelectedIndex;

    private void Start()
    {
        SelectedIndex = PlayerPrefs.GetInt("AvatarIndex", 0);
        // SelectedIndex = PlayerPrefs.GetInt("Avatar", 0);
        avatarImage.sprite = avatarSprites[SelectedIndex];
        SaveSelectedIndex();
    }

    public void SnifSelectedIndex(int shift)
    {
        // shifting index milihy ke kiri atau kanan dari sprite[]
        SelectedIndex += shift;

        while (SelectedIndex >= avatarSprites.Length)
        {
            SelectedIndex -= avatarSprites.Length;
        }

        while (SelectedIndex < 0)
        {
            SelectedIndex += avatarSprites.Length;
        }

        avatarImage.sprite = avatarSprites[SelectedIndex];
        SaveSelectedIndex();

        // // simpan di local storage
        // PlayerPrefs.SetInt("AvatarIndex", SelectedIndex);

        // // simpan di network
        // var property = new Hashtable();
        // property.Add("AvatarIndex", SelectedIndex);
        // PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }

    public void SaveSelectedIndex()
    {
        // simpan di local storage
        PlayerPrefs.SetInt("AvatarIndex", SelectedIndex);
        // PlayerPrefs.SetInt("Avatar", SelectedIndex);

        // simpan di network
        var property = new Hashtable();
        property.Add("AvatarIndex", SelectedIndex);
        // property.Add("Avatar", SelectedIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }
}
