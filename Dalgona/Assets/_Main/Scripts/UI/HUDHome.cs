using DG.Tweening;
using I2.Loc;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHome : MonoBehaviour
{
    public Camera camera;

    [Header("Button")]
    [SerializeField] private UIButton btnRanking;
    [SerializeField] private UIButton btnStart;
    [SerializeField] private UIButton btnBattleStart;
    [SerializeField] private UIButton btnSetting;
    [SerializeField] private UIButton btnLogo;
    [SerializeField] private UIButton btnBanner;

    [SerializeField] private UIButton btnHome;
    [SerializeField] private UIButton btnBattle;
    [SerializeField] private UIButton btnShop;
    [SerializeField] private UIButton btnEarn;

    [Header("Bottom")]

    [SerializeField] private List<GameObject> btnBottom;

    public void CallStart(bool _isBattle = false)
    {
        btnRanking.SetUpEvent(Action_BtnRanking);
        btnStart.SetUpEvent(Action_BtnStart);
        btnHome.SetUpEvent(Aciton_BtnHome);
        btnBattle.SetUpEvent(Aciton_BtnBattle);
        btnShop.SetUpEvent(Aciton_BtnShop);
        btnEarn.SetUpEvent(Action_BtnEarn);
        btnBattleStart.SetUpEvent(Action_BtnBattleStart);
        btnSetting.SetUpEvent(Aciton_btnSetting);
        //btnLogo.SetUpEvent(Action_btnLogo);
        btnBanner.SetUpEvent(Action_btnLogo);

        btnStart.gameObject.SetActive(!_isBattle);
        btnBattleStart.gameObject.SetActive(_isBattle);
        btnBattle.gameObject.SetActive(Module.levelCurrent>1);
        if (_isBattle)
        {
            SetSkin(btnHome.GetComponent<SkeletonGraphic>(), "normal");
            SetSkin(btnBattle.GetComponent<SkeletonGraphic>(), "active");
        }
        else
        {
            SetSkin(btnBattle.GetComponent<SkeletonGraphic>(), "normal");
            SetSkin(btnHome.GetComponent<SkeletonGraphic>(), "active");
        }


    }

    private void Action_btnLogo()
    {
        //MiniAppBridge.Instance.OpenSwapWld();
        Application.OpenURL("https://world.org/mini-app?app_id=app_1643b4b5ac6c1f00612b4cc29366ba8d&path=/dlink/Games/Dalgona");
    }

    private void Aciton_btnSetting()
    {
        //MiniAppBridge.Instance.CallLogin();
        //MiniAppBridge.Instance.CallDeposit();
        UIManager.Instance.ShowUISetting();
    }

    private void Action_BtnRanking()
    {
        UIManager.Instance.ShowUIRanking();
        //UIManager.Instance.ShowMess("Coming Soon!");
        //MiniAppBridge.Instance.CallDeposit();
    }

    private void Action_BtnStart()
    {
        UIManager.Instance.ShowHUD(UITYPE.INGAME);
    }

    public void Action_ButtonBottom(UITYPE _type)
    {
        foreach (var k in btnBottom)
        {
            SetSkin(k.GetComponent<SkeletonGraphic>(),"normal");
        }

        switch (_type)
        {
            case UITYPE.HOME:
                UIManager.Instance.SubShow(UITYPE.HOME);
                SetSkin(btnHome.GetComponent<SkeletonGraphic>(), "active");
                break;
            case UITYPE.HOME_BATTLE:
                UIManager.Instance.SubShow(UITYPE.HOME_BATTLE);
                SetSkin(btnBattle.GetComponent<SkeletonGraphic>(), "active");
                break;
            case UITYPE.EARN:

                break;
            case UITYPE.SHOP:

                break;
            default:
                break;
        }
    }

    private void SetSkin(SkeletonGraphic ske,string _skin)
    {
        ske.Skeleton.SetSkin(_skin);
        ske.Skeleton.SetSlotsToSetupPose(); // cập nhật lại pose
        ske.AnimationState.Apply(ske.Skeleton); // áp lại
    }

    private void Action_BtnBattleStart()
    {
        UIManager.Instance.ShowUISelect();
    }



    private void Aciton_BtnHome()
    {
        Action_ButtonBottom(UITYPE.HOME);
        //btnHome.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void Aciton_BtnBattle() 
    {
        Action_ButtonBottom(UITYPE.HOME_BATTLE);
       
    }
    private void Aciton_BtnShop()
    {
        //UIManager.Instance.ShowShop();
        string localize = LocalizationManager.GetTranslation("coming_soon_title");
        GameManager.Instance.ShowMess(localize);
        //FirebaseManager.Instance.SaveScore("user1", "Alice", 1234);

    }

    private void Action_BtnEarn()
    {
        UIManager.Instance.ShowUIEarn();
        MiniAppBridge.Instance.CallGetToken();
        //FirebaseManager.Instance.GetTopRanking(10);
    }
}
