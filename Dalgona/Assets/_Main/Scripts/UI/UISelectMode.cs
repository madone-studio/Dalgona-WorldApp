using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISelectMode : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;
    [SerializeField] private UIButton btn5WLD;
    [SerializeField] private UIButton btn10WLD;
    [SerializeField] private UIButton btn15WLD;
    [SerializeField] private UIButton btnGoHST;

    [SerializeField] private GameObject objLoading;
    [SerializeField] private TextMeshProUGUI txtCost;

    public GameObject enough;
    public GameObject not_enough;

    public void CallStart()
    {
        btnExit.SetUpEvent(Action_btnExit);
        btn5WLD.SetUpEvent(Action_btn5WLD);
        btn10WLD.SetUpEvent(Action_btn10WLD);
        btn15WLD.SetUpEvent(Action_btn15WLD);
        btnGoHST.SetUpEvent(Action_BtnHST);

        bool isEnough = UserModel.Instance.TokenWLD>= 0.5f;
        enough.SetActive(isEnough);
        not_enough.SetActive(!isEnough);
    }

    private void Action_BtnHST()
    {
        MiniAppBridge.Instance.OpenSwapWld();
        gameObject.SetActive(false);
    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    private void Action_btn5WLD()
    {
        Module.typeBattle = ETypeBattle.Esasy;
        Debug.Log(Module.isEnough());
        if (!Module.isEnough())
        {
            MiniAppBridge.Instance.OpenSwapWld();
            return;
        } 

#if UNITY_EDITOR
        Loading();
#endif
        MiniAppBridge.Instance.CallDeposit(Module.depositBattle());
       

    }

    private void Action_btn10WLD()
    {
        Module.typeBattle = ETypeBattle.Normal;
        Debug.Log(Module.isEnough());
        if (!Module.isEnough())
        {
            MiniAppBridge.Instance.OpenSwapWld();
            return;
        }

#if UNITY_EDITOR
        Loading();
#endif
        MiniAppBridge.Instance.CallDeposit(Module.depositBattle());
    }

    private void Action_btn15WLD()
    {
        Module.typeBattle = ETypeBattle.Hard;
        Debug.Log(Module.isEnough());
        if (!Module.isEnough())
        {
            MiniAppBridge.Instance.OpenSwapWld();
            return;
        }

#if UNITY_EDITOR
        Loading();
#endif
        MiniAppBridge.Instance.CallDeposit(Module.depositBattle());
    }

    public void Loading()
    {
        objLoading.gameObject.SetActive(true);
        txtCost.text = "searching for an opponent\n" + Module.depositBattle() + " WLD";
        DOVirtual.DelayedCall(2, () => Module.LoadScene("Battle"));
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
        Loading();
    }

    void HandleDepositFail(DepositResult result)
    {
        Debug.Log("Nạp tiền thất bại! Lý do: " + result.message);
        if (result.message == "user_rejected")
        {
            GameManager.Instance.ShowMess("User reject!");
        }

    }
}
