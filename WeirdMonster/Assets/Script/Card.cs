using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Attack AttackValue;
    public CardPlayer player;
    // public Transform AtkPosRef;
    public Vector2 OriginalPosition;
    public Vector2 OriginalScale;
    Color originalColor;
    bool isClickAble = true;

    private void Start()
    {
        OriginalPosition = this.transform.position;
        OriginalScale = this.transform.localScale;
        originalColor = GetComponent<Image>().color;

        // var seq = DOTween.Sequence();

        // seq.Append(transform.DOMove(AtkPosRef.position, 5).SetLoops(-1, LoopType.Yoyo));
        // seq.Append(transform.DOMove(startPosition, 5).SetLoops(-1, LoopType.Yoyo));
    }

    public void OnClick()
    {
        if (isClickAble)
        {
            OriginalPosition = this.transform.position; //baru
            player.SetChoosenCard(this);
        }
    }

    public void SetCLickAble(bool value)
    {
        isClickAble = value;
    }


    internal void Reset()
    {
        transform.position = OriginalPosition;
        transform.localScale = OriginalScale;
        GetComponent<Image>().color = originalColor;
    }


    // float timer = 0;

    // private void Update()
    // {
    //     if (timer < 1)
    //     {
    //         var newX = Linear(startPosition.x, AtkPosRef.position.x, timer);
    //         var newY = Linear(startPosition.y, AtkPosRef.position.y, timer);
    //         this.transform.position = new Vector2(newX, newY);
    //         timer += Time.deltaTime;
    //     }
    //     else
    //     {
    //         timer = 0;
    //     }


    //     // if (timer <= 5)
    //     // {
    //     //     timer += Time.deltaTime;
    //     // }
    //     // else
    //     // {
    //     //     this.transform.position = AtkPosRef.position;
    //     // }
    // }

    // private float Linear(float start, float end, float time)
    // {
    //     return (end - start) * time + start;
    // }
}
