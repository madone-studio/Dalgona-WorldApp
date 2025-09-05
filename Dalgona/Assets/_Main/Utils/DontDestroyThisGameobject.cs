using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyThisGameobject : Singleton<DontDestroyThisGameobject>
{
    protected override void Awake()
    {
        base.Awake();
        if (transform.childCount > 0)
            DontDestroyOnLoad(this.gameObject);
        else
            Destroy(this.gameObject);

    }
    void Start()
    {
        Application.targetFrameRate = 120;
    }
}
