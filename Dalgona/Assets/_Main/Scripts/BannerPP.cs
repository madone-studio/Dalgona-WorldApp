using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BannerPP : MonoBehaviour
{
    public List<Sprite> sprites;
    public Image imgBanner;

    private int IndexLangue()
    {
        switch (Module.langueSave)
        {
            case "English": return 0;
            case "Spanish": return 1;
            case "Thailand": return 2;
            case "Indo": return 3;
            default:
                return 0;
        }
    }

    private void OnEnable()
    {
        Module.Event_ChangeLangue += Module_Event_ChangeLangue;
        imgBanner.sprite = sprites[IndexLangue()];
    }

    private void Module_Event_ChangeLangue()
    {
        imgBanner.sprite = sprites[IndexLangue()];
    }

    private void OnDisable()
    {
        Module.Event_ChangeLangue -= Module_Event_ChangeLangue;
    }
}
