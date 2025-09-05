using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BotController : MonoBehaviour
{
    public Transform needle;
    public Cookie cookie;
    public ParticleSystem breakEffectPrefab;
    public float stabAnimTime = 0.45f;
    public float dragY = 0f;
    public float idleY = 0.35f;
    public float lerpSpeed = 10f;
    public float minDelay = 0.3f; // delay giữa mỗi lần "đâm"
    public float maxDelay = 0.6f;
    // offset nhỏ để kim nằm trên mặt bánh, không chìm
    public float stabYOffset = 0.01f; // Cho public để chỉnh Inspector nếu cần


    [Header("Spawn")]
    public Transform trSpawnCookie;

    private List<Transform> listCandyPieces = new List<Transform>();
    private Coroutine botRoutine;

    public bool isDone => listCandyPieces.Count == 0;

    public void SpawnCookie()
    {
        Cookie _cook;
        _cook = BattleManager.Instance.rdCookie;

        switch (Module.typeBattle)
        {
            case ETypeBattle.Esasy:
                minDelay = 0.01f;
                maxDelay = 0.05f;
                stabAnimTime = 0.1f;
                break;
            case ETypeBattle.Normal:
                minDelay = 0.01f;
                maxDelay = 0.04f;
                stabAnimTime = 0.075f;
                break;
            case ETypeBattle.Hard:
                minDelay = 0.01f;
                maxDelay = 0.03f;
                stabAnimTime = 0.05f;
                break;
            default:
                minDelay = 0.01f;
                maxDelay = 0.05f;
                stabAnimTime = 0.1f;
                break;
        }


        //Cookie _cook = cookiePrefabs[Random.Range(0, cookiePrefabs.Count)];

        GameObject _g = SimplePool.Spawn(_cook.gameObject, Vector3.zero, _cook.transform.rotation, trSpawnCookie);
        _g.transform.localPosition = Vector3.zero;
        _g.transform.localScale = Vector3.one;
        cookie = _g.GetComponent<Cookie>();
    }

    public void Init()
    {
        //if(cookie != null)
        //    cookie = _cookie;

        listCandyPieces.Clear();
        foreach (Transform t in cookie.trWin)
        {
            if (t.gameObject.activeSelf)
                listCandyPieces.Add(t);
        }
        needle.localPosition = new Vector3(0, idleY, 0);
        if (botRoutine != null) StopCoroutine(botRoutine);
        botRoutine = StartCoroutine(BotPlayRoutine());
    }

    IEnumerator BotPlayRoutine()
    {
        yield return new WaitForSeconds(0.7f);

        while (listCandyPieces.Count > 0)
        {
            // Random chọn 1 mảnh chưa vỡ
            int idx = Random.Range(0, listCandyPieces.Count);
            Transform targetPiece = listCandyPieces[idx];

            Vector3 targetXZ = needle.parent.InverseTransformPoint(targetPiece.position);
            targetXZ.y += stabYOffset; // kim chạm sát mặt mảnh bánh (dùng luôn vị trí y thực tế của mảnh)
            yield return MoveNeedleTo(targetXZ);

            // Anim stab xuống (nếu muốn, hoặc giữ luôn dragY)
            yield return StabDown();

            // "Vỡ" mảnh này
            if (breakEffectPrefab != null)
            {
                SoundManager.Instance.PlayCookie();
                GameObject g= SimplePool.Spawn(breakEffectPrefab.gameObject, targetPiece.position, Quaternion.identity);
                DOVirtual.DelayedCall(1, () => SimplePool.Despawn(g));
            }
               

            targetPiece.gameObject.SetActive(false);
            listCandyPieces.RemoveAt(idx);

            // Check win
            if (listCandyPieces.Count == 0)
            {
                // Gọi thắng cho bot nếu cần, hoặc báo về GameCtrl
                if (GameCtrl.Instance.State == GameState.Playing)
                {
                    GameCtrl.Instance.State = GameState.End;
                    BattleManager.Instance.ShowLose();
                }
                  

                yield break;
            }

            // Anim needle nhấc lên
            yield return StabUp();

            // Chờ delay ngắn trước khi chuyển mảnh mới
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

    IEnumerator MoveNeedleTo(Vector3 targetLocal)
    {
        float t = 0;
        Vector3 start = needle.localPosition;
        while (Vector3.Distance(new Vector3(needle.localPosition.x, 0, needle.localPosition.z), new Vector3(targetLocal.x, 0, targetLocal.z)) > 0.01f)
        {
            t += Time.deltaTime * lerpSpeed;
            Vector3 pos = Vector3.Lerp(start, targetLocal, t);
            pos.y = needle.localPosition.y; // không đổi Y khi di chuyển XZ
            needle.localPosition = pos;
            yield return null;
        }
        // Đặt XZ đúng vị trí, giữ Y hiện tại
        Vector3 fix = needle.localPosition;
        fix.x = targetLocal.x;
        fix.z = targetLocal.z;
        needle.localPosition = fix;
    }

    IEnumerator StabDown()
    {
        float t = 0.2f;
        float fromY = needle.localPosition.y;
        while (Mathf.Abs(needle.localPosition.y - dragY) > 0.01f)
        {
            t += Time.deltaTime / stabAnimTime;
            Vector3 pos = needle.localPosition;
            pos.y = Mathf.Lerp(fromY, dragY, t);
            needle.localPosition = pos;
            yield return null;
        }
        Vector3 fix = needle.localPosition;
        fix.y = dragY;
        needle.localPosition = fix;
        yield return null;
    }

    IEnumerator StabUp()
    {
        float t = 0;
        float fromY = needle.localPosition.y;
        while (Mathf.Abs(needle.localPosition.y - idleY) > 0.01f)
        {
            t += Time.deltaTime / stabAnimTime;
            Vector3 pos = needle.localPosition;
            pos.y = Mathf.Lerp(fromY, idleY, t);
            needle.localPosition = pos;
            yield return null;
        }
        Vector3 fix = needle.localPosition;
        fix.y = idleY;
        needle.localPosition = fix;
        yield return null;
    }
}
