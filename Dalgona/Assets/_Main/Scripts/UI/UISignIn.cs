using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class UISignIn : MonoBehaviour
{
    [SerializeField] private UIButton btnSignIn;
    [SerializeField] private Image imgDisable;

    string myWallet = "0x707a428824a790226d78827eecbc428197c15aa1";
    private void OnEnable()
    {
        imgDisable.gameObject.SetActive(false);
        btnSignIn.enabled = true;
        btnSignIn.SetUpEvent(Action_btnSignIn);
        Module.Event_SignIn += Module_Event_SignIn;

        btnSignIn.gameObject.SetActive(false);
        
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1); // đợi PlayerPrefs và bridge init

        string s = string.Format("Wallet ID: {0}", Module.walletID);
        Debug.LogError(s);

        if (string.IsNullOrEmpty(Module.walletID))
        {
            InvokeRepeating(nameof(Action_btnSignIn), 0f, 5f);
        }
        else
        {
            Debug.Log(s);
            PlayFabLogin.Instance.CallLogin(Module.walletID);
            yield return null;
            yield return null;
            yield return null;
            yield return new WaitUntil(()=>UserModel.Instance!=null);
            yield return new WaitUntil(()=>Module.isLoaded);
            yield return null;
            Module.LoadScene("MainGame");

        }
    }

    private void Module_Event_SignIn(bool _result)
    {
        if (_result)
        {
            CancelInvoke(nameof(Action_btnSignIn)); // Dừng gọi khi login thành công
            string s = string.Format("Wallet ID: {0}", Module.walletID);
            Debug.Log(s);
            if (!string.IsNullOrEmpty(Module.walletID))
            {
                DOVirtual.DelayedCall(1, () => Module.LoadScene("MainGame"));
            }
            else
            {
                MiniAppBridge.Instance.CallLogin();
            }
        }
        else
        {
            //btnSignIn.gameObject.SetActive(true);
            //btnSignIn.enabled = true;
            GameManager.Instance.ShowMess("loading....");
        }

    }

    private void Action_btnSignIn()
    {
        MiniAppBridge.Instance.CallLogin();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Action_btnSignIn)); // Dừng khi out UI
        Module.Event_SignIn -= Module_Event_SignIn;
    }

}
