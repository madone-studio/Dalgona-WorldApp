using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{

    [SerializeField] private Image imgProgress;
    [SerializeField] private SkeletonGraphic title;

    public void ShowLoading()
    {
        gameObject.SetActive(true);
        imgProgress.fillAmount = 0;
        imgProgress.DOFillAmount(1, 2.867f).OnComplete(() => { gameObject.SetActive(false); });

    }
}
