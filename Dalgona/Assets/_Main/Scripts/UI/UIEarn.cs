
using DG.Tweening;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEarn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtTotal;
    [SerializeField] private TextMeshProUGUI txtClaim;
    [SerializeField] private TextMeshProUGUI txtTime;
    [SerializeField] private UIButton btnClaim;
    [SerializeField] private UIButton btnBack;
    [SerializeField] private GameObject disableBtn;
    [SerializeField] private TextMeshProUGUI txtHistory;
    

    [SerializeField] private UITransaction uITransaction;

    bool CanClaim()
    {
        if (Module.coin_currency < 50)
            return false;

        if (string.IsNullOrEmpty(HistoryModel.Instance.LastClaimTime)) return true; // Chưa claim lần nào

        DateTime last = DateTime.Parse(HistoryModel.Instance.LastClaimTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
        TimeSpan passed = DateTime.Now - last;
        return passed.TotalMinutes >= 5; // Đủ 5 phút chưa
    }

    public void CallStart()
    {
        txtTotal.text = Module.coin_currency.ToString();
        btnClaim.SetUpEvent(Action_btnClaim);
        btnBack.SetUpEvent(Action_btnBack);

        if (Module.coin_currency < 50)
        {
            disableBtn.SetActive(true);
            btnClaim.enabled = false;
            txtClaim.gameObject.SetActive(false);
            txtTime.gameObject.SetActive(true);
            txtTime.text = "Not enough!";
        }
        else if (CanClaim())
        {
            btnClaim.enabled = true;
            disableBtn.SetActive(false);
            txtClaim.gameObject.SetActive(true);
            txtTime.gameObject.SetActive(false);
        }
        else
        {
            btnClaim.enabled = false;
            disableBtn.SetActive(true);
            DateTime last = DateTime.Parse(HistoryModel.Instance.LastClaimTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            TimeSpan remaining = TimeSpan.FromMinutes(5) - (DateTime.Now - last);
            if (remaining.TotalSeconds > 0)
                txtTime.text = $"Wait {remaining.Minutes:D2}:{remaining.Seconds:D2}";
            else
            {
                txtClaim.gameObject.SetActive(true);
                txtTime.gameObject.SetActive(false);
            }
                
        }

        UpdateClaimHistoryUI();
    }

    private void Update()
    {
        if (!btnClaim.enabled && !string.IsNullOrEmpty(HistoryModel.Instance.LastClaimTime))
        {
            DateTime last = DateTime.Parse(HistoryModel.Instance.LastClaimTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            TimeSpan remaining = TimeSpan.FromMinutes(5) - (DateTime.Now - last);
            if (remaining.TotalSeconds <= 0)
            {
                btnClaim.enabled = true;
                disableBtn.SetActive(false);
                txtClaim.gameObject.SetActive(true);
                txtTime.gameObject.SetActive(false);
            }
            else
            {
                txtClaim.gameObject.SetActive(false);
                txtTime.gameObject.SetActive(true);
                txtTime.text = $"Wait {remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }
        }
    }

    private void Action_btnClaim()
    {
        if (Module.coin_currency < 50)
        {
            Debug.Log("Không đủ coin để claim!");
            return;
        }
        if (!CanClaim())
        {
            Debug.Log("Chưa đủ thời gian cooldown!");
            return;
        }

        // ... gọi API claim ...
        MiniAppBridge.Instance.ClaimTokenDAL(50);


    }

    private void Action_btnBack()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        MiniAppBridge.Instance.OnClaimSuccess += Instance_OnClaimSuccess;
    }

    private void Instance_OnClaimSuccess(MiniAppBridge.ClaimResult obj)
    {
        HistoryModel.Instance.LastClaimTime = DateTime.Now.ToString("o");

        Module.coin_currency -= 50;
        AddClaimHistory(50);
        // Sau khi claim xong:
        //PlayerPrefs.SetString("LastClaimTime", DateTime.Now.ToString("o"));
        //PlayerPrefs.Save();


        // Disable nút, update lại UI (có thể gọi lại CallStart)
        DOVirtual.DelayedCall(0.5f, CallStart);
       
    }

    private void OnDisable()
    {
        MiniAppBridge.Instance.OnClaimSuccess -= Instance_OnClaimSuccess;
    }

    #region History
    public List<LabelHistory> labelHistories = new List<LabelHistory>();
    public void UpdateClaimHistoryUI()
    {
        List<ClaimHistoryItem> history = HistoryModel.Instance.items;
        txtHistory.text = "";
        //string claim = LocalizationManager.GetTranslation("claim_button");
        //history = history.OrderByDescending(i => i.time).ToList();
        foreach (var k in labelHistories)
            k.gameObject.SetActive(false);


        for (int i = 0; i < 10; i++)
        {
            if (i < history.Count)
            {
                var item = history[i];
                //txtHistory.text += $"{item.time}    :    {claim} {item.amount} $DAL\n";
                labelHistories[i].gameObject.SetActive(true);
                labelHistories[i].CallStart(item.time, item.amount.ToString());
            }         
        }
    }

    public void AddClaimHistory(float amount)
    {
        ClaimHistoryItem item = new ClaimHistoryItem
        {
            time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            amount = amount
        };

        HistoryModel.Instance.items.Add(item);
        HistoryModel.Instance.Sort(); 
        //history.Insert(0, item); // Thay cho history.Add(item);

        PlayFabLogin.Instance.SavePlayerData("ClaimHistory", HistoryModel.Instance.json());
    }

    #endregion
}
