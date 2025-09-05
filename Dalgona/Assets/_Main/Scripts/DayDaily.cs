
using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayDaily : MonoBehaviour
{
    public int day;
    public float reward;

    public TextMeshProUGUI txtDay;
    public TextMeshProUGUI txtReward;

    [SerializeField] private Image imgBoder;
    [SerializeField] private Sprite[] sprBorder;
    [SerializeField] private GameObject claimed;
    [SerializeField] private GameObject fx;
    public void CallStart()
    {
        string localizedDay = LocalizationManager.GetTranslation("day_text");
        txtDay.text = $"{"Day"} {day}";
        txtReward.text = "+" + reward.ToString();
        claimed.SetActive(false);
        imgBoder.sprite = sprBorder[0];
        fx.gameObject.SetActive(false);
        if (day < Module.getRewardDaily)
        {
            //Debug.LogError("Claimed: " + day);
            claimed.SetActive(true);
        }
        else if (day == Module.getRewardDaily)
        {
            if (Module.dayreward != DateTime.Now.Day)
            {
                //Debug.LogError("Get : " + day);
                imgBoder.sprite = sprBorder[1];
                transform.localScale = Vector3.one * 1.2f;
                fx.gameObject.SetActive(true);
            }
            else
            {
                //Debug.LogError("Claimed: " + day);
                claimed.SetActive(true);
            }
           
        }
        else
        {
            //Debug.LogError("Not Claim: " + day);

        }
    }
}
