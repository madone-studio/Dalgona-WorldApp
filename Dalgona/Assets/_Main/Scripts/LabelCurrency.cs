using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public enum ECurrency
{
    Coin,
    Star
}
public class LabelCurrency : MonoBehaviour
{
    [SerializeField] private ECurrency Type;
    [SerializeField] private TextMeshProUGUI txtCurrency;
    [SerializeField] private UIButton btnReload;
    private void OnEnable()
    {
        Module.Event_Change_Money += Module_Event_Change_Money;
        if (btnReload != null & Type == ECurrency.Star)
        {
            btnReload.SetUpEvent(MiniAppBridge.Instance.CallGetToken);
        }

        DOVirtual.DelayedCall(0.1f, () => { Module_Event_Change_Money(); });

    }

    private void Module_Event_Change_Money()
    {
        switch (Type)
        {
            case ECurrency.Coin:
                txtCurrency.text = Module.coin_currency.ToString();
                break;
            case ECurrency.Star:
                txtCurrency.text = FormatF2Fallback(Module.wld_currency);
               
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        Module.Event_Change_Money -= Module_Event_Change_Money;
    }

    string FormatF2Safe(string raw)
    {
        if (double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
        {
            double rounded = Math.Floor(val * 100) / 100.0; // cắt xuống 2 chữ số thập phân
            string str = rounded.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            return str;
        }
        return "0.00";
    }

    string FormatF2Fallback(string raw)
    {
        if (raw.Contains("."))
        {
            var parts = raw.Split('.');
            string decimals = parts[1].Length > 2 ? parts[1].Substring(0, 2) : parts[1].PadRight(2, '0');
            return $"{parts[0]}.{decimals}";
        }
        return $"{raw}.00";
    }
}
