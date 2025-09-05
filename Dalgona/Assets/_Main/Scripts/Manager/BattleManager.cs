using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager>
{
    public BotController botController;

    public GameObject objReady;
    public TextMeshProUGUI txtReady;
    public TextMeshProUGUI txtOpponent;
    public TextMeshProUGUI txtWLD;

    public UIWin mUIWin;
    public UILose mLose;

    public Cookie rdCookie;
    void Start()
    {
        rdCookie = GameCtrl.Instance.cookieHard[Module.EasyRandom(GameCtrl.Instance.cookieHard.Count)];
        txtWLD.text = Module.depositBattle() + " WLD";
        StartCoroutine(IeStart());

        if (Module.isPlayPVP == 0)
            Module.isPlayPVP = 1;
    }

    IEnumerator IeStart()
    {
        yield return null;
        botController.SpawnCookie();
        txtOpponent.text = "@" + nameBots[Module.EasyRandom(nameBots.Count)];
        int time = 3;
        while (time > 0)
        {
            txtReady.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;
        }

        yield return null;
        txtReady.text = $"{I2.Loc.LocalizationManager.GetTranslation("ready_title")}!";

        yield return new WaitForSeconds(1);
        objReady.SetActive(false);
        GameCtrl.Instance.State = GameState.Playing;
        Debug.Log("GameStart!");
        botController.Init();

        yield return null;
      
    }

    public void ShowLose()
    {

        mLose.gameObject.SetActive(true);
        mLose.CallStartBattle();
    }

    public void ShowWin()
    {
        if (!botController.isDone)
        {
            mUIWin.gameObject.SetActive(true);
            mUIWin.CallStartBattle();
        }

    }

    public void BackToHome()
    {
        Module.LoadScene(1);
    }

    #region BotName
    public List<string> nameBots;
    [ContextMenu("GenerateBotName")]
    public void GenerateBotName()
    {
        nameBots.Clear();
        nameBots = GenerateUsernames(1000);
    }

    List<string> GenerateUsernames(int count)
    {
        string[] prefixes = { "alex", "john", "lisa", "jane", "minh", "thao", "trung", "ngoc", "david", "anna", "mike", "phuong", "bao", "quang", "hoa", "hieu", "son", "duy", "khanh", "van" };
        string[] suffixes = { "", "dev", "it", "pro", "ai", "bot", "vip", "007", "player", "game", "king", "queen", "tv", "x", "alpha", "beta" };

        HashSet<string> usernames = new HashSet<string>();
        System.Random rnd = new System.Random();

        while (usernames.Count < count)
        {
            string prefix = prefixes[rnd.Next(prefixes.Length)];
            string suffix = suffixes[rnd.Next(suffixes.Length)];
            string usernameBase = prefix + suffix;

            bool isVerified = rnd.Next(2) == 0; // 50% xác suất đã verify

            string username;
            if (!isVerified)
            {
                int number = rnd.Next(1000, 10000); // 4 chữ số
                username = usernameBase + "." + number;
            }
            else
            {
                username = usernameBase;
            }

            usernames.Add(username.ToLower());
        }

        return new List<string>(usernames);
    }

    #endregion
}
