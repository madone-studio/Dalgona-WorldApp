using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using TMPro;
using System;

public class UISetting : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;
    [SerializeField] TMP_Dropdown dropDownLang;
    public Toggle togSound;
    public Toggle togMusic;

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

    public void CallStart()
    {
        togSound.isOn = Module.soundFx == 1;
        togMusic.isOn = Module.musicFx == 1;
        dropDownLang.value = IndexLangue();
        dropDownLang.onValueChanged.AddListener(OnLanguageChanged);
        togSound.onValueChanged.AddListener((x) => Action_Change_Sound(x));
        togMusic.onValueChanged.AddListener((x) => Action_Change_Music(x));
        btnExit.SetUpEvent(Action_btnExit);
    }

    private void OnDisable()
    {
        togSound.onValueChanged.RemoveListener((x) => Action_Change_Sound(x));
        togMusic.onValueChanged.RemoveListener((x) => Action_Change_Music(x));
    }
    private void OnLanguageChanged(int index)
    {
        string selectedLang = dropDownLang.options[index].text;
        LocalizationManager.CurrentLanguage = selectedLang;
        Module.langueSave = selectedLang;
        Debug.Log("Language changed to: " + selectedLang);

        Module.Action_Event_ChangeLanguae();
    }
    public void Action_Change_Sound(bool _isOn = true)
    {
        if (!_isOn)
            Module.soundFx = 0;
        else
            Module.soundFx = 1;


        Module.Action_ChangeSound();
    }

    public void Action_Change_Music(bool _isOn = true)
    {
        if (!_isOn)
            Module.musicFx = 0;
        else
            Module.musicFx = 1;

        Module.Action_ChangeMusic();
    }

    public void Action_btnExit()
    {
        gameObject.SetActive(false);
    }
}
