using UnityEngine;

public class Cookie : MonoBehaviour
{
    public Transform trLose;
    public Transform trWin;


    public void Init()
    {
        GameCtrl.Instance.InitCandy(this);
        foreach (Transform child in trLose)
        {
            child.gameObject.SetActive(true);
        }

        foreach (Transform child in trWin)
        {
            child.gameObject.SetActive(true);
        }
    }


    [ContextMenu("OnValidate")]
    private void SetValidate()
    {
        if (trLose == null) trLose = transform.GetChild(0);
        if (trWin == null) trWin = transform.GetChild(1);

        int candyLayer = LayerMask.NameToLayer("Candy");
        // Đệ quy gán tag/layer cho tất cả con cháu bên trLose
        if (trLose != null)
            SetTagAndLayerRecursively(trLose, "CandyLose", candyLayer);

        // Đệ quy gán tag/layer cho tất cả con cháu bên trWin
        if (trWin != null)
            SetTagAndLayerRecursively(trWin, "CandyPiece", candyLayer);
    }

    // Hàm đệ quy
    void SetTagAndLayerRecursively(Transform parent, string tag, int layer)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.tag = tag;
            child.gameObject.layer = layer;
            // Đệ quy xuống các cấp cháu, chắt...
            SetTagAndLayerRecursively(child, tag, layer);
        }
    }
}
