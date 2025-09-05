
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ETypeBattle
{
    Esasy,
    Normal,
    Hard
}
public static class Module
{
    #region Statics
    public static string idDevice = "";
    public static int highScore = 0;
    public static float modeType = 1;
    public static int languetype = 0;
    public static bool isLoaded = false;

    public static ETypeBattle typeBattle = ETypeBattle.Esasy;
    public static string depositBattle()
    {
        switch (typeBattle)
        {
            case ETypeBattle.Esasy:
                return "0.5";
            case ETypeBattle.Normal:
                return "3";
            case ETypeBattle.Hard:
                return "5";
            default:
                return "0.5";
        }
    }

    public static string claimBattle()
    {
        switch (typeBattle)
        {
            case ETypeBattle.Esasy:
                return "0.9";
            case ETypeBattle.Normal:
                return "5.4";
            case ETypeBattle.Hard:
                return "9";
            default:
                return "0.9";
        }
    }

    public static string depositSKIP()
    {
        int _lv = levelCurrent;
        if (_lv > 0 && _lv <= 3)
            return "0.1";
        else if (_lv > 3 && _lv <= 7)
            return "0.3";
        else
            return "0.5";
    }

    public static float deposit_SKIP()
    {
        int _lv = levelCurrent;
        if (_lv > 0 && _lv <= 3)
            return 0.1f;
        else if (_lv > 3 && _lv <= 7)
            return 0.3f;
        else
            return 0.5f;
    }

    public static bool isEnough()
    {
        Debug.Log("WLD : " + UserModel.Instance.TokenWLD);
        switch (typeBattle)
        {
            case ETypeBattle.Esasy:
                return UserModel.Instance.TokenWLD >= 0.5f;
            case ETypeBattle.Normal:
                return UserModel.Instance.TokenWLD >= 3;
            case ETypeBattle.Hard:
                return UserModel.Instance.TokenWLD >= 5; ;
            default:
                return false;
        }
    }


    public static string FormatF2Fallback(string raw)
    {
        Debug.Log(raw);
        if (raw.Contains("."))
        {
            var parts = raw.Split('.');
            string decimals = parts[1].Length > 2 ? parts[1].Substring(0, 2) : parts[1].PadRight(2, '0');
            return $"{parts[0]}.{decimals}";
        }
        return $"{raw}.00";
    }

    public static float FormatF2FallbackDouble(string raw)
    {
        if (raw.Contains("."))
        {
            var parts = raw.Split('.');
            string decimals = parts[1].Length > 2 ? parts[1].Substring(0, 2) : parts[1].PadRight(2, '0');
            return float.Parse(FormatF2Fallback($"{parts[0]}.{decimals}"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
        }


        return float.Parse(FormatF2Fallback($"{raw}.00"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture); 
    }
    #endregion

    #region Data PlayerPrefs
    public static string langueSave
    {
        get
        {
            string txtbase = "English";

            if (!PlayerPrefs.HasKey("languageSave"))
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        txtbase = "English";
                        break;
                    case SystemLanguage.Spanish:
                        txtbase = "Spanish";
                        break;
                    case SystemLanguage.Thai:
                        txtbase = "Thailand";
                        break;
                    case SystemLanguage.Indonesian:
                        txtbase = "Indo";
                        break;
                    default:
                        txtbase = "English";
                        break;
                }
                Debug.LogError(Application.systemLanguage + "--" + txtbase);
                // ❗ Lưu lại giá trị ngôn ngữ lần đầu vào PlayerPrefs
                PlayerPrefs.SetString("languageSave", txtbase);
                PlayerPrefs.Save();
            }

            return PlayerPrefs.GetString("languageSave");
        }

        set
        {
            PlayerPrefs.SetString("languageSave", value);
            PlayerPrefs.Save(); // nên thêm Save() để đảm bảo ghi xuống WebGL
        }
    }

    public static int isPlayPVP
    {
        get { return  UserModel.Instance.isPlayPVP; ; }
        set
        {
            UserModel.Instance.isPlayPVP = value;
            PlayFabLogin.Instance.SaveUserData();
        }
    }
    public static string jwt
    {
        get { return PlayerPrefs.GetString("jwt", string.Empty); }
        set { PlayerPrefs.SetString("jwt", value); }
    }

    public static string walletID
    {
        get { return PlayerPrefs.GetString("wallet", string.Empty); }
        set {
            PlayerPrefs.SetString("wallet", value); 
        }
    }

    public static float soundFx
    {
        get { return PlayerPrefs.GetFloat("sound_fx", 1); }
        set { PlayerPrefs.SetFloat("sound_fx", value); }
    }

    public static float musicFx
    {
        get { return PlayerPrefs.GetFloat("music_fx", 1); }
        set { PlayerPrefs.SetFloat("music_fx", value); }
    }
    public static int coin_currency
    {
        get { return UserModel.Instance.token_dal; }
        set
        {

            if (value <= 0)
                value = 0;
            UserModel.Instance.token_dal = value;
            Action_Event_Change_Money();
            PlayFabLogin.Instance.SaveUserData();
        }
    }

    public static string wld_currency
    {
        get { return UserModel.Instance.token_wld; }
        set
        {
            UserModel.Instance.token_wld = value;
            Action_Event_Change_Money();
        }
    }

    public static int levelCurrent
    {
        get { return UserModel.Instance.levelCurrent; }
        set {
            UserModel.Instance.levelCurrent = value;
            PlayFabLogin.Instance.SaveUserData();
        }
    }

    public static int dayreward
    {
        get { return UserModel.Instance.dayReward; }
        set { UserModel.Instance.dayReward = value;}
    }

    public static int getRewardDaily
    {
        get { return UserModel.Instance.getRewardDaily; }
        set {
            UserModel.Instance.getRewardDaily = value;
            PlayFabLogin.Instance.SaveUserData();
        }

    }

    #endregion

    #region Internet 
    public static bool isNetworking()
    {
        bool result = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
            result = false;
        return result;
    }

    #endregion

    #region Random

    private static System.Random mRandom = new System.Random();
    public static int EasyRandom(int range)
    {
        return mRandom.Next(range);
    }

    public static int EasyRandom(int min, int max)//không bao gồm max
    {
        return mRandom.Next(min, max);
    }

    public static float EasyRandom(float min, float max)
    {
        return UnityEngine.Random.RandomRange(min, max);
    }

    #endregion

    #region Convert
    public static string TimestampNow()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
    }

    // Chuyển số thành text
    public static string NumberCustomToString(float _number)
    {
        string str = "";
        if (_number < 10000)
            str = _number.ToString("00");
        else if (10000 <= _number && _number < 1000000)
            str = (_number / 1000).ToString("0.#") + "K";
        else if (1000000 <= _number && _number < 1000000000)
            str = (_number / 1000000).ToString("0.##") + "M";
        else
            str = (_number / 1000000000).ToString("0.##") + "B";
        return str;
    }

    //Chuyển time s => form
    public static string SecondCustomToTime(int _second)
    {
        string str = "";
        int second = 0;
        int minute = 0;
        int hour = 0;
        second = _second % 60;
        if (second > 59) second = 59;
        minute = (int)(Mathf.Floor(_second / 60) % 60);
        hour = (int)(_second / 3600);


        if (hour > 0)
            str += hour.ToString("00") + "h";

        if (minute >= 0)
            str += minute.ToString("00") + "m";

        if (_second < 3600)
            str += second.ToString("00") + "s";

        //str = hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");
        return str;
    }
    public static string SecondCustomToTime2(int _second)
    {
        string str = "00:00";
        int second = 0;
        int minute = 0;
        //int hour = 0;
        second = _second % 60;
        if (second > 59) second = 59;
        minute = (int)(Mathf.Floor(_second / 60) % 60);
        //hour = (int)(_second / 3600);


        str =/* hour.ToString("00") + ":" +*/ minute.ToString("00") + ":" + second.ToString("00");
        return str;
    }

    #endregion

    #region Scenes Change
    public static void LoadScene(int _index)
    {
        SceneManager.LoadSceneAsync(_index);
        //PlayFabLogin.Instance.SaveUserData();

    }

    public static void LoadScene(string _name)
    {
        SceneManager.LoadSceneAsync(_name);


    }

    #endregion

    #region Match Format
    public static int money_idle_offline(int _time, int _progress)
    {
        int money = 0;

        return money;
    }

    #endregion

    #region Event
    public static event GAME_STATE_CHANGE Event_Change_State;

    public static void Action_Event_Change_State(GameState _state)
    {
        if (Event_Change_State != null)
        {
            Event_Change_State(_state);
        }
    }


    public static event REFRESH Event_Change_Money;
    public static void Action_Event_Change_Money()
    {
        if (Event_Change_Money != null)
        {
            Event_Change_Money();
        }
    }

    public static event Action Event_ChangeSound;
    public static void Action_ChangeSound()
    {
        if (Event_ChangeSound != null)
        {
            Event_ChangeSound();
        }

    }

    public static event Action Event_ChangeMusic;
    public static void Action_ChangeMusic()
    {
        if (Event_ChangeMusic != null)
        {
            Event_ChangeMusic();
        }

    }

    public static event Action Event_ChangeLangue;
    public static void Action_Event_ChangeLanguae()
    {
        if (Event_ChangeLangue != null)
        {
            Event_ChangeLangue();
        }

    }

    public static event SIGNIN Event_SignIn;
    public static void Action_EventSignIn(bool _result)
    {
        if (Event_SignIn != null)
        {
            Event_SignIn(_result);
        }

    }

    #endregion
}

public delegate void REFRESH();
public delegate void SIGNIN(bool _result);
public delegate void GAME_STATE_CHANGE(GameState _state);

