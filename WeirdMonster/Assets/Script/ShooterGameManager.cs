using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShooterGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefabs.name, Vector2.zero, Quaternion.identity);
    }
}
