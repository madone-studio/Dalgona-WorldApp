using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    Home,
    Playing,
    End,
    Loading
}

public class GameCtrl : Singleton<GameCtrl>
{

    public bool isTesting;
    public Cookie cookie;
    public List<GameObject> listCandies;
    public List<Cookie> cookiePrefabs;
    public List<Cookie> cookieHard;

    private Cookie cookieSave;
    public NeedleDrag3D needleDrag;
    [SerializeField] private Transform trSpawnCookie;

    public GameState _state;
    public GameState State
    {
        get { return _state; }
        set
        {
            _state = value;
            Module.Action_Event_Change_State(_state);
        }
    }

    private void Start()
    {
        State = GameState.Home;

        if (SceneManager.GetActiveScene().name == "Battle")
        {
            DOVirtual.DelayedCall(0.5f, SpawnCandyBattle);
        }
        else
        {
            StartCoroutine(SpawnCandy());
        }

    }

    [ContextMenu("InitCandy")]
    public void InitCandy(Cookie _cook)
    {
        cookie = _cook;
        cookie.transform.localScale = Vector3.one;
        listCandies.Clear();
        int candyLayer = LayerMask.NameToLayer("Candy");

        foreach (Transform child in cookie.trWin)
        {
            if (child.gameObject.layer == candyLayer)
            {
                listCandies.Add(child.gameObject);
            }
        }
    }

    public void CandyBreak(GameObject _cd)
    {
        listCandies.Remove(_cd);
        if (listCandies.Count <= 0)
        {
            Debug.Log("Win");
            State = GameState.End;

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.ShowWin();
            }
            else
            {
                UIManager.Instance.ShowUIWin();
            }


        }

    }

    IEnumerator SpawnCandy()
    {      
        yield return null;
        Cookie _cook;
        if (isTesting)
        {
            _cook = cookie;
        }
        else
        {
            if (cookie != null)
            {
                SimplePool.Despawn(cookie.gameObject);
                cookie = null;
            }

            yield return new WaitUntil(()=> UserModel.Instance!=null);

            if (Module.levelCurrent < cookiePrefabs.Count)
            {
                _cook = cookiePrefabs[Module.levelCurrent - 1];
            }
            else
            {
                _cook = cookieHard[Random.Range(0, cookieHard.Count)];
            }
        }

        cookieSave = _cook;

        //Cookie _cook = cookiePrefabs[Random.Range(0, cookiePrefabs.Count)];

        GameObject _g = SimplePool.Spawn(_cook.gameObject, Vector3.zero, _cook.transform.rotation, trSpawnCookie);
        _g.transform.localPosition = Vector3.zero;
        cookie = _g.GetComponent<Cookie>();
        cookie.Init();
    }

    public void SpawnCandyBattle()
    {

        Cookie _cook;
        _cook = BattleManager.Instance.rdCookie;

        //Cookie _cook = cookiePrefabs[Random.Range(0, cookiePrefabs.Count)];

        GameObject _g = SimplePool.Spawn(_cook.gameObject, Vector3.zero, _cook.transform.rotation, trSpawnCookie);
        _g.transform.localPosition = Vector3.zero;
        cookie = _g.GetComponent<Cookie>();
        cookie.Init();
    }

    public void ResetGame()
    {
        needleDrag.Refresh();
        StartCoroutine(SpawnCandy());
    }

    public void RetryGame()
    {
        if (cookie != null)
        {
            SimplePool.Despawn(cookie.gameObject);
            cookie = null;
        }

        needleDrag.Refresh();
        Cookie _cook = cookieSave;
        GameObject _g = SimplePool.Spawn(_cook.gameObject, Vector3.zero, _cook.transform.rotation, trSpawnCookie);
        _g.transform.localPosition = Vector3.zero;
        cookie = _g.GetComponent<Cookie>();
        cookie.Init();
    }
}
