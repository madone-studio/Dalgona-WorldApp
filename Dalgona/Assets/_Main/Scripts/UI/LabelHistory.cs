using TMPro;
using UnityEngine;

public class LabelHistory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtTime;
    [SerializeField] private TextMeshProUGUI txtValue;

    public void CallStart(string _time,string _value)
    {
        gameObject.SetActive(true);
        txtTime.text = _time ;
        txtValue.text = "+" + _value ;
    }


}
