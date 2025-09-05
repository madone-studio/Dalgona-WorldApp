
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "HistoryModel", menuName = "ScriptableObjects/HistoryModel")]

public class HistoryModel : ScriptableObject
{
    private static HistoryModel _instance;
    public static HistoryModel Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<HistoryModel>("HistoryModel");
            return _instance;
        }
    }
    public string LastClaimTime = string.Empty;

    public List<ClaimHistoryItem> items;
    string strTest;
    [ContextMenu("LOAD")]
    public void Load()
    {
        OveriteJson(strTest);
        
    }

    public string json()
    {
        return JsonUtility.ToJson(this);
    }

    public void Sort()
    {
        items = items.OrderByDescending(i => i.time).ToList();

        if(items.Count > 10)
        {
            items.RemoveRange(10, items.Count-10);
        }
    }

    public void OveriteJson(string _json)
    {
        //Debug.Log("ClaimHistory: " + _json);
        JsonUtility.FromJsonOverwrite(_json, this);
        Sort();
    }

    public void ResetData()
    {
        items = new List<ClaimHistoryItem>();
        LastClaimTime = string.Empty;
    }
}


[System.Serializable]
public class ClaimHistoryItem
{
    public string time;      // ISO datetime
    public float amount;     // Số coin hoặc DAL đã claim
}
