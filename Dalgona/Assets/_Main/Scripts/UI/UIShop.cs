using UnityEngine;

public class UIShop : MonoBehaviour
{
    [SerializeField] private UIButton btnBack;

    public void CallStart()
    {
        btnBack.SetUpEvent(Action_btnBack);
    }

    private void Action_btnBack()
    {
        gameObject.SetActive(false);
    }
}
