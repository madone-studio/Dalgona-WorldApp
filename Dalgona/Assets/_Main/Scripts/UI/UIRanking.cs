
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIRanking : MonoBehaviour
{
    public RankModel rankModel;
    public TextMeshProUGUI txtR1;
    public TextMeshProUGUI txtR2;
    public TextMeshProUGUI txtR3;
  
    public List<LabelRanking> labelRankings; //4-5-6-7
    public LabelRanking userRanking;

    [SerializeField] private UIButton btnBack;

    public void CallStart()
    {
        btnBack.SetUpEvent(Action_btnBack);
        txtR1.text = rankModel.rankData[0].descTop();
        txtR2.text = rankModel.rankData[1].descTop();
        txtR3.text = rankModel.rankData[2].descTop();

        

        for (int i = 0; i < labelRankings.Count; i++)
        {
            if (i+3 < rankModel.rankData.Count)
            {
                labelRankings[i].gameObject.SetActive(true);
                labelRankings[i].CallStart(rankModel.rankData[i+3]);

            }
            else
            {
                labelRankings[i].gameObject.SetActive(false);
            }
           
        }

        userRanking.CallStart(rankModel.myRank);
    }

    private void Action_btnBack()
    {
        gameObject.SetActive(false);
    }

}
