using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class NeedleDrag3D : MonoBehaviour
{
    
    public Transform needle;
    public LayerMask candyLayer;
    public float stabAnimTime = 0.45f;
    public float dragY = 0f;
    public float idleY = 0.35f;
    public ParticleSystem breakEffectPrefab;
    public Transform candyPlaneRef; // 1 empty đặt ở mặt phẳng candy, trục y bằng mặt kẹo
    public float screenOffset = 200f; // Offset pixel trên màn hình (tùy chỉnh)
    public float lerpSpeed = 10;// chỉnh lớn hơn để nhanh, nhỏ hơn để slow

    private bool isDragging = false;
    private Tween yTween;
    private float currentY;


    [Header("Camera")]
    public Camera cam;
    public float cameraFOVIdle = 60f;
    public float cameraFOVDrag = 50f;
    public float cameraZoomTime = 0.3f;
    private Tween camFOVTween;

    void Start()
    {
        currentY = idleY;
        SetNeedleY(currentY);
    }

    void Update()
    {
        if (GameCtrl.Instance.State != GameState.Playing)
        {
            isDragging = false;
            return;
        }



        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            isDragging = true;
            if (yTween != null) yTween.Kill();
            float fromY = needle.localPosition.y;
            yTween = DOTween.To(
                () => fromY,
                y => { SetNeedleY(y); currentY = y; },
                dragY, stabAnimTime)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() => fromY = currentY);

            // ---- Zoom in camera khi bắt đầu drag ----
            if (camFOVTween != null) camFOVTween.Kill();
            camFOVTween = cam.DOFieldOfView(cameraFOVDrag, cameraZoomTime)
                .SetEase(Ease.OutCubic);
        }

        if (isDragging)
        {
            Vector3 dragPos = needle.localPosition;

            // +++ Offset vị trí chuột lên trên (theo chiều dọc màn hình)
            Vector3 mousePos = Input.mousePosition;
            mousePos.y += screenOffset; // Offset lên trên

            // Ưu tiên raycast trúng collider candy
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            bool hitCandy = Physics.Raycast(ray, out hit, 100f, candyLayer);
            if (hitCandy)
            {
                Vector3 local = needle.parent.InverseTransformPoint(hit.point);
                dragPos.x = local.x;
                dragPos.z = local.z;

                // Check va chạm với CandyPiece
                if (hit.collider.CompareTag("CandyPiece") && hit.collider.gameObject.activeSelf)
                {
                    GameCtrl.Instance.CandyBreak(hit.collider.gameObject);
                    hit.collider.gameObject.SetActive(false);
                    if (breakEffectPrefab != null)
                    {
                        SoundManager.Instance.PlayCookie();
                        GameObject g=  SimplePool.Spawn(breakEffectPrefab.gameObject, hit.point + Vector3.up/4, Quaternion.identity);
                        DOVirtual.DelayedCall(1,()=> SimplePool.Despawn(g));
                    }
                       
                }

                // Check va chạm với CandyLose
                if (hit.collider.CompareTag("CandyLose") && hit.collider.gameObject.activeSelf)
                {
                    Debug.LogError("CandyLose");
                    
                    hit.collider.gameObject.SetActive(false);
                    if (breakEffectPrefab != null)
                    {
                        SoundManager.Instance.PlayCookie();
                        GameObject g = SimplePool.Spawn(breakEffectPrefab.gameObject, hit.point, Quaternion.identity);
                        DOVirtual.DelayedCall(1, () => SimplePool.Despawn(g));
                    }

                    GameCtrl.Instance.State = GameState.End;

                    if (BattleManager.Instance != null)
                    {
                        BattleManager.Instance.ShowLose();
                    }
                    else
                    {
                        UIManager.Instance.ShowLose();
                    }

                    
                }
            }
            else
            {
                // Nếu không raycast trúng collider, raycast lên Plane mặt kẹo
                if (candyPlaneRef != null)
                {
                    Plane plane = new Plane(candyPlaneRef.up, candyPlaneRef.position);
                    float dist;
                    if (plane.Raycast(ray, out dist))
                    {
                        Vector3 worldPt = ray.GetPoint(dist);
                        Vector3 local = needle.parent.InverseTransformPoint(worldPt);
                        dragPos.x = local.x;
                        dragPos.z = local.z;
                    }
                }
            }

            //SetNeedleXZ(dragPos.x, dragPos.z);
            // BẰNG LERP MƯỢT:
            Vector3 pos = needle.localPosition;
            pos.x = Mathf.Lerp(pos.x, dragPos.x, Time.deltaTime * lerpSpeed);
            pos.z = Mathf.Lerp(pos.z, dragPos.z, Time.deltaTime * lerpSpeed);
            needle.localPosition = pos;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            if (yTween != null) yTween.Kill();
            float fromY = needle.localPosition.y;
            yTween = DOTween.To(
                () => fromY,
                y => { SetNeedleY(y); currentY = y; },
                idleY, stabAnimTime)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() => fromY = currentY);

            // ---- Zoom out camera khi ngừng drag ----
            if (camFOVTween != null) camFOVTween.Kill();
            camFOVTween = cam.DOFieldOfView(cameraFOVIdle, cameraZoomTime)
                .SetEase(Ease.OutCubic);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "CandyPiece")
        {
            GameCtrl.Instance.CandyBreak(other.gameObject);
            other.gameObject.SetActive(false);
            if (breakEffectPrefab != null)
            {
                SoundManager.Instance.PlayCookie();
                GameObject g = SimplePool.Spawn(breakEffectPrefab.gameObject, other.transform.position + Vector3.up / 4, Quaternion.identity);
                DOVirtual.DelayedCall(1, () => SimplePool.Despawn(g));
            }
        }
    }

    void SetNeedleY(float y)
    {
        var pos = needle.localPosition;
        pos.y = y;
        needle.localPosition = pos;
    }

    void SetNeedleXZ(float x, float z)
    {
        var pos = needle.localPosition;
        pos.x = x;
        pos.z = z;
        needle.localPosition = pos;
    }

    public void Refresh()
    {
        transform.localPosition = Vector3.zero;
    }
}
