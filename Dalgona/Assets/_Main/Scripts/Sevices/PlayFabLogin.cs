using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayFabLogin : Singleton<PlayFabLogin>
{
    public RankModel rankModel;
    public UserModel userModel;

    protected override void Awake()
    {
        base.Awake();
        PlayFabSettings.staticSettings.TitleId = "1720A2";
    }

    void Start()
    {
#if UNITY_EDITOR
        CallLogin("test124");
#endif
    }

    #region Login
    public void CallLogin(string _CustomId)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = _CustomId, // hoặc tự sinh ID cho user
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true // <-- để trả về DisplayName khi login
            }
        };
        userModel.id = _CustomId;
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("Đăng nhập thành công!");
        // Kiểm tra xem đã có DisplayName chưa
        string displayName = null;
        if (result.InfoResultPayload != null && result.InfoResultPayload.PlayerProfile != null)
            displayName = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (string.IsNullOrEmpty(displayName))
        {
            // Nếu chưa có tên, random rồi đặt
            string randomName = GetRandomUsername();
            SetDisplayName(randomName);
            Debug.Log("Chưa có tên. Đặt tên mới: " + randomName);

        }
        else
        {
            Debug.Log("User đã có tên: " + displayName);

        }

        StartCoroutine(IEGetData());

    }

    IEnumerator IEGetData()
    {
        LoadPlayerData("userData");

        yield return null;
        MiniAppBridge.Instance.CallGetToken();

        yield return null;
        LoadPlayerData("ClaimHistory");

        yield return null;
        GetMyRanking();

        yield return null;
        GetLeaderboard();

        yield return null;
        CheckNameWorldApp();


    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Đăng nhập thất bại: " + error.GenerateErrorReport());
    }

    #endregion

    #region UserData
    public void SaveUserData()
    {
        SavePlayerData("userData", userModel.json());
    }

    public void SavePlayerData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
        {
            { key, value }
        }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendFail);
    }

    void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Lưu data thành công lên PlayFab!");
    }

    void OnDataSendFail(PlayFabError error)
    {
        Debug.LogError("Lưu data thất bại: " + error.GenerateErrorReport());
    }


    public void LoadPlayerData(string key)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
        {
            if (result.Data != null && result.Data.ContainsKey(key))
            {
                string value = result.Data[key].Value;
                Debug.Log("User data: " + key + " = " + value);

                switch (key)
                {
                    case "userData":
                        userModel.OveriteJson(value);
                        userModel.walletID = Module.walletID;
                        userModel.jwt = Module.jwt;
                        Module.Action_Event_Change_Money();
                        Module.isLoaded = true;
                        break;
                    case "ClaimHistory":
                        HistoryModel.Instance.OveriteJson(value);

                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.Log("Không có dữ liệu cho key: " + key);

                switch (key)
                {
                    case "userData":
                        SavePlayerData(key, userModel.json());
                        break;
                    case "ClaimHistory":
                        SavePlayerData(key, HistoryModel.Instance.json());
                        break;
                    default:
                        SavePlayerData(key, "");
                        break;
                }
            }
        },
        (error) =>
        {
            Debug.LogError("Lỗi khi lấy user data: " + error.GenerateErrorReport());
        });
    }

    #endregion


    #region Ranking
    public void SubmitScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = "HighScore", // Tạo tên leaderboard
                Value = score
            }
        }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreSubmit, OnError);
    }

    void OnScoreSubmit(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Đã gửi điểm lên Leaderboard!");

        GetLeaderboard();
        GetMyRanking();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Lỗi: " + error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            rankModel.rankData.Clear();
            foreach (var entry in result.Leaderboard)
            {
                Debug.Log($"{entry.Position + 1}. {entry.DisplayName ?? entry.PlayFabId}: {entry.StatValue}");
                rankModel.rankData.Add(new RankData()
                {
                    rank = entry.Position + 1,
                    id = entry.PlayFabId,
                    highScore = entry.StatValue,
                    name = entry.DisplayName
                });
            }
        }, OnError);
    }

    public void GetMyHighScore()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnGetStats, OnError);
    }

    void OnGetStats(GetPlayerStatisticsResult result)
    {
        int highScore = 0;
        foreach (var stat in result.Statistics)
        {
            if (stat.StatisticName == "HighScore")
            {
                highScore = stat.Value;
                break;
            }
        }

        rankModel.highScore = highScore;
        Debug.Log("HighScore của user hiện tại là: " + highScore);
    }

    // Đổi thành tên statistic bạn đã dùng cho leaderboard (mặc định: "HighScore")
    public string statisticName = "HighScore";

    public void GetMyRanking()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = statisticName,
            MaxResultsCount = 1 // Lấy đúng vị trí của mình, hoặc để 5-10 để lấy thêm các vị trí lân cận
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetMyRanking, OnError);
    }

    void OnGetMyRanking(GetLeaderboardAroundPlayerResult result)
    {
        if (result.Leaderboard != null && result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            Debug.Log($"User của bạn đang ở vị trí: {myEntry.Position + 1} với điểm {myEntry.StatValue}");
            // Nếu muốn lấy PlayFabId, DisplayName:
            // myEntry.PlayFabId, myEntry.DisplayName
            rankModel.myRank.rank = myEntry.Position + 1;
            rankModel.myRank.highScore = myEntry.StatValue;
            rankModel.myRank.id = myEntry.PlayFabId;
            rankModel.myRank.name = myEntry.DisplayName;
            rankModel.highScore = myEntry.StatValue;
        }
        else
        {
            Debug.Log("User chưa có trên leaderboard hoặc chưa có điểm.");
        }
    }

    #endregion

    #region Name
    [Serializable]
    public class UserDataResult
    {
        public string username;
        public string profilePictureUrl;
        public string error;
    }

    public void CheckNameWorldApp()
    {
        MiniAppBridge.Instance.CallGetUserData(Module.walletID, (json) =>
        {
            // Parse JSON result
            //Debug.Log("Wallet: "+ userModel.walletID + " UserData trả về: " + json);
            UserDataResult data = JsonUtility.FromJson<UserDataResult>(json);
            Debug.Log("User name: " + data.username + ", avatar: " + data.profilePictureUrl);

            if (data.username != userModel.username)
            {
                userModel.username = data.username;
                SetDisplayName(data.username);
            }


        });

    }


    // Gọi hàm này khi user chọn/nhập tên muốn đặt
    public void SetDisplayName(string newName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = newName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSetDisplayNameSuccess, OnSetDisplayNameError);
    }

    void OnSetDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Cập nhật tên hiển thị thành công: " + result.DisplayName);
        // Nếu cần, cập nhật lại userModel hoặc UI ở đây
    }

    void OnSetDisplayNameError(PlayFabError error)
    {
        Debug.LogError("Lỗi cập nhật tên hiển thị: " + error.GenerateErrorReport());
    }

    public string GetRandomUsername()
    {
        string[] prefixes = {
    "alex", "james", "lucas", "emily", "sophia", "mason", "oliver", "mia", "liam", "ella",
    "noah", "ava", "jack", "leo", "ben", "chloe", "grace", "lily", "ryan", "zoe",
    "harry", "sam", "max", "daniel", "dylan", "sara", "amy", "john", "matt", "kate",
    "tom", "josh", "mark", "olivia", "nina", "nathan", "evan", "mia", "paul", "ruby"
};
        string[] suffixes = {
    "", "pro", "gamer", "plays", "tv", "yt", "official", "x", "zz", "prime", "dev",
    "01", "007", "theone", "star", "master", "king", "queen", "jr", "senior", "boss"
};

        System.Random rnd = new System.Random();
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

        return username.ToLower();
    }
    #endregion

    public void ResetUserData()
    {
        userModel.ResetData(); // Khởi tạo lại model mặc định
        HistoryModel.Instance.ResetData();

        SavePlayerData("userData", userModel.json());
        SavePlayerData("ClaimHistory", HistoryModel.Instance.json()); // hoặc format mặc định
    }

    [ContextMenu("TEST")]
    public void TEst()
    {
        LogHubEvent("deposit", 1, "WL");
    }

    public void LogHubEvent(string type, float amount, string _currency)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = "hub_transaction",
            Body = new Dictionary<string, object>
        {
            { "type", type },           // deposit hoặc claim
            { "amount", amount },
            { "currency", _currency }
            //{ "timestamp", DateTime.UtcNow.ToString("o") }
            // Có thể bổ sung hubId, txHash...
        }
        }, (result) =>
        {
            Debug.Log("Hub event logged!");
        },
        (error) =>
        {
        Debug.Log("LogHubEvent ERROR: " + error.GenerateErrorReport());
        });
    }
}
