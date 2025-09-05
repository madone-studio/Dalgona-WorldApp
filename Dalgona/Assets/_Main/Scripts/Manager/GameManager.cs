using UnityEngine;
using I2.Loc;
using TMPro;
using DG.Tweening;
public class GameManager : Singleton<GameManager>
{
    public GameObject message;
    public TextMeshProUGUI txtMessage;

    public void Start()
    {

        LocalizationManager.CurrentLanguage = Module.langueSave;
        DOVirtual.DelayedCall(5, () =>
        {
            if (!string.IsNullOrEmpty(Module.walletID))
            {
                MiniAppBridge.Instance.CallGetToken();
            }

        }).SetLoops(-1);
    }


    Tween twMess = null;
    public void ShowMess(string _txt,float _time=2)
    {
        if (twMess != null)
            twMess.Kill();
        txtMessage.text = _txt;
        message.gameObject.SetActive(true);
        twMess = DOVirtual.DelayedCall(_time, () => message.gameObject.SetActive(false));
    }
}
