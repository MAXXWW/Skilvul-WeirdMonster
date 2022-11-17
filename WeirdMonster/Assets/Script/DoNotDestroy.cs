using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] BGMObj = GameObject.FindGameObjectsWithTag("BGM");
        GameObject[] SFXObj = GameObject.FindGameObjectsWithTag("SFX");

        if (BGMObj.Length > 1)
        {
            Destroy(this.gameObject);
        }

        // if (SFXObj.Length > 1)
        // {
        //     Destroy(this.gameObject);
        // }

        DontDestroyOnLoad(this.gameObject);
    }
}
