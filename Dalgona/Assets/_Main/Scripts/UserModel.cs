using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserModel", menuName = "ScriptableObjects/UserModel")]
public class UserModel : ScriptableObject
{
    private static UserModel _instance;
    public static UserModel Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<UserModel>("UserModel");
            return _instance;
        }
    }

    public int levelCurrent = 1;
  
    public string id;
    public string username;
    public string walletID;
    public string jwt;
    public int token_dal;
    public int isPlayPVP=0;
    public string token_wld;
    public float claimDAL=0;
    public float claimWLD=0;
    public float depositWLD=0;

    [Header("Daily")]
    public int dayReward = 0;
    public int getRewardDaily = 1;
    public int auto = 0;
    public double TokenWLD => double.Parse(FormatF2Fallback(token_wld), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
    string FormatF2Fallback(string raw)
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

    public string json()
    {
        return JsonUtility.ToJson(this);
    }

    public void OveriteJson(string _json)
    {
        JsonUtility.FromJsonOverwrite(_json, this);
    }

    public void ResetData()
    {
         levelCurrent = 1;
         walletID = Module.walletID;
         jwt = Module.jwt;
         isPlayPVP = 0;
         token_dal = 0;
         dayReward = 0;
         getRewardDaily = 1;
         claimWLD = 0;
         depositWLD = 0;
         claimDAL=0;
         auto = 0;
    }
}


