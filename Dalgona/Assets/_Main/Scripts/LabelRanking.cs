using TMPro;
using UnityEngine;

public class LabelRanking : MonoBehaviour
{
    public RankData rankData;
    public TextMeshProUGUI txtRank;
    public TextMeshProUGUI txtScore;

    public void CallStart(RankData _data)
    {
        rankData = _data;
        if (string.IsNullOrEmpty(rankData.name))
        {
            txtRank.text = rankData.rank + "." + "NUser." + Module.EasyRandom(1000, 10000);
        }
        else
        {
            txtRank.text = rankData.rank + "." + rankData.name;
        }
        
        txtScore.text = rankData.highScore.ToString("00");
    }

    string GetLastN(string s, int n)
    {
        if (string.IsNullOrEmpty(s) || s.Length < n) return s;
        return s.Substring(s.Length - n);
    }
}
