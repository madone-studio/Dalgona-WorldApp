using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UICheat : MonoBehaviour
{
    public TMP_InputField inputField;
    public UIButton btnClaim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btnClaim.SetUpEvent(Action_btnClaim);
    }

    public void Action_btnClaim()
    {
        MiniAppBridge.Instance.ClaimTokenWLD(inputField.text);
    }
}
