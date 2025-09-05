using DG.Tweening;
using UnityEngine;

public class UILose : MonoBehaviour
{
    [Header("Normal")]
    [SerializeField] private UIButton btnBuyNow;
    [SerializeField] private UIButton btnRetry;
    [SerializeField] private UIButton btnSkip;
    [SerializeField] private GameObject panelBuyNow;
    [SerializeField] private GameObject enough;


    [Header("Battle")]
    [SerializeField] private UIButton btnContinue;
    [SerializeField] private UIButton btnHome;

    [SerializeField] private AudioClip clipLose;
    public void CallStart()
    {
        btnBuyNow.SetUpEvent(Action_BtnBuyNow);
        btnRetry.SetUpEvent(Action_BtnRetry);
        btnSkip.SetUpEvent(Action_BtnSkip);

        bool isEnough = UserModel.Instance.TokenWLD >= Module.deposit_SKIP();
        panelBuyNow.SetActive(!isEnough);
        enough.gameObject.SetActive(isEnough);
        SoundManager.Instance.PlayOnCamera(clipLose);
    }

    public void CallStartBattle()
    {
        SoundManager.Instance.PlayOnCamera(clipLose);
        btnHome.SetUpEvent(Action_btnHome);
        btnContinue.SetUpEvent(Action_btnContinue);
    }

    bool isBattle = false;
    private void Action_btnContinue()
    {
        isBattle = true;
        MiniAppBridge.Instance.CallDeposit(Module.depositBattle());
    }

    private void Action_btnHome()
    {
        Module.LoadScene(1);
    }

    private void Action_BtnBuyNow()
    {
        MiniAppBridge.Instance.OpenSwapWld();

        DOVirtual.DelayedCall(3, () => {
            bool isEnough = UserModel.Instance.TokenWLD >= Module.deposit_SKIP();
            panelBuyNow.SetActive(!isEnough);
            enough.gameObject.SetActive(isEnough);

        });
      
    }

    public void Action_BtnRetry()
    {
        gameObject.SetActive(false);
        UIManager.Instance.ShowHUD(UITYPE.INGAME, true);
        GameCtrl.Instance.RetryGame();
    }

    private void Action_BtnSkip()
    {
        bool isEnough = UserModel.Instance.TokenWLD >= Module.deposit_SKIP();

        if (isEnough)
        {
            MiniAppBridge.Instance.CallDepositSKIP(Module.depositSKIP());
        }
        else
        {
            panelBuyNow.SetActive(!isEnough);
            enough.gameObject.SetActive(isEnough);
        }

       
        //gameObject.SetActive(false);
        //UIManager.Instance.ShowHUD(UITYPE.HOME, true);
        //GameCtrl.Instance.ResetGame();
    }

    void OnEnable()
    {
        MiniAppBridge.Instance.OnDepositSuccess += HandleDepositSuccess;
        MiniAppBridge.Instance.OnDepositFail += HandleDepositFail;
    }

    void OnDisable()
    {
        MiniAppBridge.Instance.OnDepositSuccess -= HandleDepositSuccess;
        MiniAppBridge.Instance.OnDepositFail -= HandleDepositFail;
    }

    void HandleDepositSuccess(DepositResult result)
    {
        Debug.Log("Nạp tiền thành công! Hash: " + result.hash);

        if (!isBattle)
        {
            gameObject.SetActive(false);
            Module.levelCurrent++;
            MiniAppBridge.Instance.CallGetToken();
            UIManager.Instance.ShowHUD(UITYPE.INGAME, true);
            GameCtrl.Instance.ResetGame();
        }
        else
        {
            Module.LoadScene("Battle");
            MiniAppBridge.Instance.CallGetToken();
        }
      
    }

    void HandleDepositFail(DepositResult result)
    {
        Debug.Log("Nạp tiền thất bại! Lý do: " + result.message);
        if(result.message == "user_rejected")
        {
            GameManager.Instance.ShowMess("User reject!");
        }
      
    }
}
