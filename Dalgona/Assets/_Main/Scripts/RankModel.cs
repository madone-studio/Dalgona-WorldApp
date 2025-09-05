using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "RankModel", menuName = "ScriptableObjects/RankModel")]
public class RankModel : ScriptableObject
{
    public int highScore;
    public RankData myRank;
    public List<RankData> rankData;

    [ContextMenu("Generate")]
    public void Generate()
    {
        rankData.Clear();
        for (int i = 1; i <= 10; i++)
        {
            rankData.Add(new RankData()
            {
                rank = i,
                id = System.Guid.NewGuid().ToString(),
                highScore = 20 - i
            });
        }
    }
}

[System.Serializable]
public class RankData
{
    public int rank;
    public string id;
    public int highScore;
    public string name;



    public string descTop() 
    {
        if (string.IsNullOrEmpty(name))
        {
            return "User." + Module.EasyRandom(1000, 10000);
        }
        else
        {
            return name;
        }
    }
    string GetLastN(string s, int n)
    {
        if (string.IsNullOrEmpty(s) || s.Length < n) return s;
        return s.Substring(s.Length - n);
    }
}


