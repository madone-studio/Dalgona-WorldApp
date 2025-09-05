
using I2.Loc;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDailyReward : MonoBehaviour
{
    public List<DayDaily> listDay;
    public UIButton btnGet;

    [ContextMenu("CallStart")]
    public void CallStart()
    {
        foreach (var d in listDay)
        {
            d.CallStart();
        }

        btnGet.SetUpEvent(Action_btnReward);

    }

    private void Action_btnReward()
    {
        if (Module.getRewardDaily==7)
        {
            if (Module.isPlayPVP == 1)
                MiniAppBridge.Instance.ClaimTokenWLD("0.1");
            else
            {
                string localize = LocalizationManager.GetTranslation("pvp_first");
                GameManager.Instance.ShowMess(localize);
                UIManager.Instance.ShowHUD(UITYPE.HOME_BATTLE);
                gameObject.SetActive(false);
                return;
            }
               
        }
        else
        {
            Module.coin_currency += (int)listDay[Module.getRewardDaily - 1].reward;
        }

        Module.dayreward = DateTime.Now.Day;
        Module.getRewardDaily++;
        gameObject.SetActive(false);

    }
}
