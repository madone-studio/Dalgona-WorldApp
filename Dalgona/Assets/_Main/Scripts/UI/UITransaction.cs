using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UITransaction : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI txtSend;
    [SerializeField] private TextMeshProUGUI txtReceive;
    [SerializeField] private TextMeshProUGUI txtWallet;
    [SerializeField] private UIButton btnAllow;

    public void CallStart(ECurrency type,string _value, UnityAction action)
    {
        txtWallet.text = Module.walletID;
        txtReceive.text = _value;

        btnAllow.SetUpEvent(() => {
            gameObject.SetActive(false);
            action.Invoke();
            });

    }
}
