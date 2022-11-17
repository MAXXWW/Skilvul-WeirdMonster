using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using DG.Tweening;

public class ShooterPlayer : MonoBehaviourPun
{
    [SerializeField] float speed = 5;
    [SerializeField] float health = 10;
    [SerializeField] int damage = 1;
    [SerializeField] TMP_Text playerName;

    private void Start()
    {
        playerName.text = photonView.Owner.NickName + $" ({health})";
    }

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        transform.Translate(moveDir * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("TakeDamage", RpcTarget.All, damage);
        }
    }

    [PunRPC]
    public void TakeDamage(int amoount)
    {
        health -= amoount;
        playerName.text = photonView.Owner.NickName + $" ({health})";
        GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
    }
}
