using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UITYPE
{
    HOME,
    HOME_BATTLE,
    INGAME,
    INGAME_BATTLE,
    SHOP,
    EARN,
    ENDGAME
}

public class UIManager : Singleton<UIManager>
{
    public HUDHome m_HUDHome;
    public UIInGame m_UIInGame;
    public UIWin m_UIWin;
    public UILose m_Lose;
    public UIShop m_UIShop;
    public UILoading m_UILoading;
    public UISelectMode m_UISelectMode;
    public UIDailyReward m_UIDailyReward;
    public UIRanking m_UIRanking;
    public UIEarn m_UIEarn;
    public UISetting m_UISetting;

    public List<GameObject> listHUD;

    public GameObject message;
    public TextMeshProUGUI txtMessage;

    private void Start()
    {
        //ShowLoading();
        ShowUIHome();
    }

    public void ShowUISetting()
    {
        m_UISetting.gameObject.SetActive(true);
        m_UISetting.CallStart();
    }

    public void ShowUIHome()
    {
        m_HUDHome.gameObject.SetActive(true);
 
        m_HUDHome.CallStart(Module.levelCurrent > 1);
    }

    public void ShowUIWin()
    {
        m_UIWin.gameObject.SetActive(true);
        m_UIWin.CallStart();
    }

    public void ShowLose()
    {
        m_Lose.gameObject.SetActive(true);
        m_Lose.CallStart();
    }

    public void ShowLoading()
    {
        m_UILoading.ShowLoading();
    }

    public void ShowShop()
    {
        m_UIShop.gameObject.SetActive(true);
        m_UIShop.CallStart();
    }

    public void ShowUISelect()
    {
        m_UISelectMode.gameObject.SetActive(true);
        m_UISelectMode.CallStart();
    }

    public void ShowDailyReward()
    {
        m_UIDailyReward.gameObject.SetActive(true);
        m_UIDailyReward.CallStart();
    }

    public void ShowUIRanking()
    {
        m_UIRanking.gameObject.SetActive(true);
        m_UIRanking.CallStart();
    }

    public void ShowUIEarn()
    {
        m_UIEarn.gameObject.SetActive(true);
        m_UIEarn.CallStart();
    }

    public void ShowHUD(UITYPE _type, bool _Isloading = false)
    {
        Camera.main.fieldOfView = 60;
        if (_Isloading)
        {
            GameCtrl.Instance.State = GameState.Loading;
            ShowLoading();

            DOVirtual.DelayedCall(3f, () => { SubShow(_type); });

            return;
        }
        else
        {
            SubShow(_type);
        }

    }

    public void SubShow(UITYPE _type)
    {
        Camera.main.fieldOfView = 60;
        foreach (var k in listHUD)
            k.gameObject.SetActive(false);

        switch (_type)
        {
            case UITYPE.HOME:
                GameCtrl.Instance.State = GameState.Home;
                GameCtrl.Instance.needleDrag.enabled = false;
                m_HUDHome.gameObject.SetActive(true);
                m_HUDHome.CallStart();

                //Check reward
                if (DateTime.Now.Day > Module.dayreward && Module.levelCurrent > 1 && Module.getRewardDaily <= 7)
                {
                    ShowDailyReward();
                }

                break;
            case UITYPE.HOME_BATTLE:
                GameCtrl.Instance.State = GameState.Home;
                GameCtrl.Instance.needleDrag.enabled = false;
                m_HUDHome.gameObject.SetActive(true);
                m_HUDHome.CallStart(true);

                break;
            case UITYPE.INGAME:
                GameCtrl.Instance.needleDrag.enabled = true;
                GameCtrl.Instance.State = GameState.Playing;
                m_UIInGame.gameObject.SetActive(true);
                m_UIInGame.CallStart();
                break;
            case UITYPE.INGAME_BATTLE:
                GameCtrl.Instance.needleDrag.enabled = true;
                break;
            case UITYPE.SHOP:
                break;
            case UITYPE.EARN:
                break;
            case UITYPE.ENDGAME:
                break;
            default:
                break;
        }
    }


    Tween twMess = null;
    public void ShowMess(string _txt,float _time =2f)
    {
        if (twMess != null)
            twMess.Kill();
        txtMessage.text = _txt;
        message.gameObject.SetActive(true);
        twMess= DOVirtual.DelayedCall(_time, ()=> message.gameObject.SetActive(false));
    }
}
