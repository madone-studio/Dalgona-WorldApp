using AssetKits.ParticleImage;
using DG.Tweening;
using I2.Loc;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class UIWin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtCoin;
    [SerializeField] private TextMeshProUGUI txtStar;
    [SerializeField] private GameObject objMess;

    [SerializeField] private UIButton btnClaim;
    [SerializeField] private UIButton btnContinue;
    [SerializeField] private UIButton btnHome;
    [SerializeField] private GameObject imgClaimed;

    [SerializeField] private AudioClip clipWin;
    [SerializeField] private List<ParticleImage> particleImages;
    [SerializeField] private UITransaction uITransaction;
    [SerializeField] private ParticleImage coinParticle;

    int rd_Coin = 0;
    float rd_Star = 0;
    public void CallStart()
    {
        btnClaim.enabled = true;
        imgClaimed.gameObject.SetActive(false);
        btnClaim.SetUpEvent(Action_btnClaim);
        btnContinue.SetUpEvent(Action_btnContinue);

        string localize = LocalizationManager.GetTranslation("level_name");
        string complete = LocalizationManager.GetTranslation("complete_text");
        txtTitle.text = $"{complete} {localize} {Module.levelCurrent.ToString("00")}";

        rd_Coin = Module.EasyRandom(5, 20);

        txtCoin.text = rd_Coin.ToString("00");
        txtStar.text = "00";

        if (Module.levelCurrent == 1)
        {
            //string unlock = LocalizationManager.GetTranslation("unlock_text");
            //string battle = LocalizationManager.GetTranslation("battle_mode");
            //GameManager.Instance.ShowMess($"{unlock} {battle}!",5f);
            objMess.gameObject.SetActive(true);
            btnContinue.SetUpEvent(() =>
            {
                coinParticle.gameObject.SetActive(true);
                objMess.gameObject.SetActive(false);
                btnContinue.enabled = false;
                DOVirtual.DelayedCall(2f, () => {

                    Module.coin_currency += rd_Coin;
                    Module.levelCurrent++;
                    coinParticle.gameObject.SetActive(false);

                    DOVirtual.DelayedCall(0.2f, () => {
                        btnContinue.enabled = true;
                        gameObject.SetActive(false);
                        UIManager.Instance.ShowHUD(UITYPE.HOME_BATTLE, true);
                        GameCtrl.Instance.ResetGame();
                    });

                });
               
            });
        }


        foreach (var k in particleImages)
        {
            k.gameObject.SetActive(true);
            k.Play();
        }
        SoundManager.Instance.PlayOnCamera(clipWin);
    }

    public void CallStartBattle()
    {

        btnClaim.SetUpEvent(Action_btnBattleClaim);
        btnHome.SetUpEvent(() => Module.LoadScene(1));
        btnContinue.SetUpEvent(() => MiniAppBridge.Instance.CallDeposit(Module.depositBattle()));
        imgClaimed.gameObject.SetActive(false);

        txtCoin.text = Module.claimBattle();

        foreach (var k in particleImages)
        {
            k.gameObject.SetActive(true);
            k.Play();
        }
        SoundManager.Instance.PlayOnCamera(clipWin);

        PlayFabLogin.Instance.rankModel.highScore += 1;
        PlayFabLogin.Instance.SubmitScore(PlayFabLogin.Instance.rankModel.highScore);


    }

    private void Action_btnClaim()
    {
        //Module.star_currency += rd_Star;
        coinParticle.gameObject.SetActive(true);
        btnClaim.enabled = false;
        imgClaimed.gameObject.SetActive(true);

        DOVirtual.DelayedCall(2f, () => {
            Module.coin_currency += rd_Coin;
            coinParticle.gameObject.SetActive(false);
        });
    }

    private void Action_btnBattleClaim()
    {
        MiniAppBridge.Instance.ClaimTokenWLD(Module.claimBattle());
        imgClaimed.gameObject.SetActive(true);
        btnClaim.enabled = false;

    }

    private void Action_btnContinue()
    {
        coinParticle.gameObject.SetActive(true);
        btnContinue.enabled = false;
        DOVirtual.DelayedCall(2f, () => {
           
            Module.coin_currency += rd_Coin;  
            Module.levelCurrent++;
            coinParticle.gameObject.SetActive(false);

            DOVirtual.DelayedCall(0.2f, () => {
                btnContinue.enabled = true;
                gameObject.SetActive(false);
                UIManager.Instance.ShowHUD(UITYPE.INGAME, true);
                GameCtrl.Instance.ResetGame();
            });
           
        });
       
    }

    void OnEnable()
    {
        MiniAppBridge.Instance.OnDepositSuccess += HandleDepositSuccess;
        MiniAppBridge.Instance.OnDepositFail += HandleDepositFail;
        MiniAppBridge.Instance.OnClaimSuccess += Instance_OnClaimSuccess;
        MiniAppBridge.Instance.OnClaimFail += Instance_OnClaimFail;
    }

    private void Instance_OnClaimFail(MiniAppBridge.ClaimResult obj)
    {
        btnClaim.enabled = true;
        imgClaimed.gameObject.SetActive(false);
    }

    private void Instance_OnClaimSuccess(MiniAppBridge.ClaimResult obj)
    {
        DOVirtual.DelayedCall(2, MiniAppBridge.Instance.CallGetToken);
    }

    void HandleDepositSuccess(DepositResult result)
    {
        Debug.Log("Nạp tiền thành công! Hash: " + result.hash);
        Module.LoadScene("Battle");
    }

    void HandleDepositFail(DepositResult result)
    {
        Debug.Log("Nạp tiền thất bại! Lý do: " + result.message);
        if (result.message == "user_rejected")
        {
            GameManager.Instance.ShowMess("User reject!");
        }

    }
    void OnDisable()
    {
        MiniAppBridge.Instance.OnDepositSuccess -= HandleDepositSuccess;
        MiniAppBridge.Instance.OnDepositFail -= HandleDepositFail;
        MiniAppBridge.Instance.OnClaimSuccess -= Instance_OnClaimSuccess;
        MiniAppBridge.Instance.OnClaimFail -= Instance_OnClaimFail;
    }

}
