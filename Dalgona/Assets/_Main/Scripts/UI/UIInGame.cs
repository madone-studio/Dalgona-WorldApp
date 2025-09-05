using I2.Loc;
using TMPro;
using UnityEngine;

public class UIInGame : MonoBehaviour
{
    [SerializeField] private UIButton btnBack;
    [SerializeField] private TextMeshProUGUI txtLevel;
    public void CallStart()
    {
        string localize = LocalizationManager.GetTranslation("level_name");
        txtLevel.text = $"{localize} {Module.levelCurrent.ToString("00")}";
        btnBack.SetUpEvent(Action_btnBack);
    }

    private void Action_btnBack()
    {
        if(Module.levelCurrent>1)
            UIManager.Instance.ShowHUD(UITYPE.HOME_BATTLE, true);
        else
            UIManager.Instance.ShowHUD(UITYPE.HOME, true);
        GameCtrl.Instance.ResetGame();
        GameCtrl.Instance.needleDrag.Refresh();
    }
}
