using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.InteropServices;
using System.Numerics;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using System.Security.Claims;

/// <summary>
/// addContract:0xb60B3c5A52E26f8f8F01438F4A5E840C7C2861Ba
/// tokenWLD: 0x8603a12c549007a3AFE026EFAD797640Bda30760
/// tokenDAL: 0x717ef2955c812a1deC5BC4d3f90721445BFD6839
/// </summary>


[Serializable]
public class LoginResult { public string status; public string jwt; public string wallet; public string message; }
[Serializable]
public class DepositResult { public string status; public string hash; public string message; }
[Serializable]
public class TokenBalanceResult { public string status; public string balance; public string message; }

public class MiniAppBridge : Singleton<MiniAppBridge>
{
    public const string addContract = "0xb60B3c5A52E26f8f8F01438F4A5E840C7C2861Ba";
    public const string tokenWLD = "0x2cfc85d8e48f8eab294be644d9e25c3030863003";
    public const string tokenDAL = "0x717ef2955c812a1deC5BC4d3f90721445BFD6839";

    [DllImport("__Internal")] private static extern void LoginFromUnity(string url, string jwt);
    [DllImport("__Internal")] private static extern void DepositFromUnity(string contract, string token, string amount, string payload);
    [DllImport("__Internal")] private static extern void GetTokenBalanceFromUnity(string tokenAddress, string walletAddress);


    protected override void Awake()
    {
        base.Awake();

    }

    #region Login & deposit
    public void CallLogin()
    {
        LoginFromUnity("https://api-world-event-listener-dev.nysm.work",Module.jwt);
        //Debug.Log("LoginFromUnity chỉ hoạt động khi build WebGL.");

    }

    public event Action<DepositResult> OnDepositSuccess;
    public event Action<DepositResult> OnDepositFail;
    public void CallDeposit(string wld)
    {
        Debug.Log($"Deposit Called: {wld}");
        DepositFromUnity(
            addContract,
            tokenWLD,
            wld,
            "501"
        );

        //Debug.Log("DepositFromUnity chỉ hoạt động khi build WebGL.");
        depositvalue = Module.FormatF2FallbackDouble(wld);
    }

    public void CallDepositSKIP(string _wld)
    {
        Debug.Log($"Deposit Called: {_wld}");
        DepositFromUnity(
            addContract,
            tokenWLD,
            _wld,
            "500"
        );

        depositvalue = Module.deposit_SKIP();
    }

    public void CallGetToken()
    {
        Debug.Log("CallGetToken");
        if (string.IsNullOrEmpty(Module.walletID))
            CallLogin();
        else
        {
            CallGetTokenBalance(tokenWLD, Module.walletID);
        }

    }

    public void CallGetTokenBalance(string _tokenAddress, string walletAddress)
    {

        GetTokenBalanceFromUnity(_tokenAddress, walletAddress);
        //Debug.Log("GetTokenBalanceFromUnity chỉ hoạt động khi build WebGL.");
    }

    public void OnLoginResult(string json)
    {
        Debug.Log("LoginResult: " + json);
        var result = JsonUtility.FromJson<LoginResult>(json);
        if (result.status == "success")
        {

            // TODO: xử lý đăng nhập thành công
            Module.jwt = result.jwt;
            Module.walletID = result.wallet;

            UserModel.Instance.walletID = result.wallet;
            UserModel.Instance.jwt = result.jwt;
            //Debug.Log("JWT: " + UserModel.Instance.jwt + ", Wallet: " + result.wallet);

            PlayFabLogin.Instance.CallLogin(result.wallet);

            Module.Action_EventSignIn(true);
        }
        else
        {
            Debug.LogError("Login error: " + result.message);
            // TODO: xử lý lỗi
            Module.Action_EventSignIn(false);
            //GameManager.Instance.ShowMess(result.message);
        }
    }

    private float depositvalue = 0;
    public void OnDepositResult(string json)
    {
        Debug.Log("DepositResult: " + json);
        var result = JsonUtility.FromJson<DepositResult>(json);
        if (result.status == "success")
        {
            Debug.Log("Tx Hash: " + result.hash);
            // TODO: xử lý deposit thành công
            OnDepositSuccess?.Invoke(result);
            CallGetToken();

            
            UserModel.Instance.depositWLD += depositvalue;
            PlayFabLogin.Instance.LogHubEvent("deposit", depositvalue, "WL");
        }
        else
        {
            Debug.LogError("Deposit error: " + result.message);
            // TODO: xử lý lỗi
            OnDepositFail?.Invoke(result);

            GameManager.Instance.ShowMess("User reject!");
        }
    }

    public void OnGetTokenBalanceResult(string json)
    {
        //Debug.Log("TokenBalanceResult: " + json);
        var result = JsonUtility.FromJson<TokenBalanceResult>(json);
        if (result.status == "success")
        {
            //Debug.Log("Balance: " + result.balance);
            // TODO: xử lý balance thành công
            Module.wld_currency = result.balance;
            //PlayFabLogin.Instance.SavePlayerData("Token_WLD", result.balance);
        }
        else
        {
            Debug.LogError("Balance error: " + result.message);
            // TODO: xử lý lỗi

            //GameManager.Instance.ShowMess("Balance error");
        }
    }

    #endregion


    #region Claim
    [Serializable]
    public class ClaimResult
    {
        public string status;
        public string hash;
        public string message;
    }

    public void ClaimTokenWLD(int _amount)
    {
        //int decimals = 18;
        //BigInteger amount = new BigInteger(_amount) * BigInteger.Pow(10, decimals);
        string amountStr = _amount.ToString();
        Debug.Log("Claim WLD: " + amountStr);
        StartClaimRequestSafe(
        addContract,
        tokenWLD,
        amountStr,
        "100",
        Module.walletID);
    }

    public void ClaimTokenWLD(string _amount)
    {
        Debug.Log("ClaimTokenWLD: " + _amount);
        StartClaimRequestSafe(
        addContract,
        tokenWLD,
        _amount,
        "100",
        Module.walletID);

        float _claim = float.Parse(_amount);
        UserModel.Instance.claimWLD += _claim;
        PlayFabLogin.Instance.LogHubEvent("claim", _claim, "WL");
    }

    public void ClaimTokenDAL(int _amount)
    {
        //int decimals = 18;
        //BigInteger amount = new BigInteger(_amount) * BigInteger.Pow(10, decimals);
        string amountStr = _amount.ToString();
        Debug.Log("Claim DAL: " + amountStr);
        StartClaimRequestSafe(
        addContract,
        tokenDAL,
        amountStr,
        "101",
        Module.walletID);
        UserModel.Instance.claimDAL += _amount;
        //PlayFabLogin.Instance.LogHubEvent("claim", _amount, "DA");
    }

    #region Claim

    /*
    /// <summary>
    /// Gọi API claim rút token
    /// </summary>
    /// <param name="hubAddress">Địa chỉ hub (thường là addContract)</param>
    /// <param name="tokenAddress">Địa chỉ token đã deposit</param>
    /// <param name="amount">Số token (dạng string, thường là Wei)</param>
    /// <param name="payload">Dữ liệu payload tuỳ game (string/number)</param>
    /// <param name="recipient">Ví nhận</param>
    public void CallClaim(string hubAddress, string tokenAddress, string amount, string payload, string recipient)
    {
        StartCoroutine(ClaimCoroutine(hubAddress, tokenAddress, amount, payload, recipient));
    }

    IEnumerator ClaimCoroutine(string hubAddress, string tokenAddress, string amount, string payload, string recipient)
    {
        string url = "https://api-world-event-listener-dev.nysm.work/api/transaction-request/claim";

        // Tạo payload dạng anonymous type để chuyển thành JSON
        ClaimRequestData postData = new ClaimRequestData()
        {
            hubAddress = hubAddress,
            tokenAddress = tokenAddress,
            amount = amount,
            payload = payload,
            recipient = recipient
        };

        string jsonBody = JsonUtility.ToJson(postData);

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("accept", "application/json");
        req.SetRequestHeader("x-secret", "123");
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (req.result != UnityWebRequest.Result.Success)
#else
    if (req.isNetworkError || req.isHttpError)
#endif
        {
            Debug.LogError("Claim API lỗi: " + req.error + "\nResponse: " + req.downloadHandler.text);
            if (OnClaimFail != null)
            {
                var errResult = JsonUtility.FromJson<ClaimResult>(req.downloadHandler.text);
                OnClaimFail.Invoke(errResult);
            }
        }
        else
        {
            Debug.Log("Claim API thành công! Response: " + req.downloadHandler.text);
            if (OnClaimSuccess != null)
            {
                var result = JsonUtility.FromJson<ClaimResult>(req.downloadHandler.text);
                OnClaimSuccess.Invoke(result);

                CallGetToken();
            }
        }
    }
     */


    #endregion

    #region Claim Request 2
    public event Action<ClaimResult> OnClaimSuccess;
    public event Action<ClaimResult> OnClaimFail;
    [Serializable]
    public class ClaimRequestData
    {
        public string hubAddress;
        public string tokenAddress;
        public string amount;
        public string payload;
        public string recipient;
    }

    private Coroutine currentClaimRequestCoroutine;
    private void StartClaimRequestSafe(string hubAddress, string tokenAddress, string amount, string payload, string recipient)
    {
        if (currentClaimRequestCoroutine != null)
        {
            StopCoroutine(currentClaimRequestCoroutine);
            //Debug.Log("Đã dừng ClaimRequestCoroutine cũ để chạy mới.");
        }

        currentClaimRequestCoroutine = StartCoroutine(ClaimRequestCoroutine(hubAddress, tokenAddress, amount, payload, recipient));
    }

    IEnumerator ClaimRequestCoroutine(string hubAddress, string tokenAddress, string amount, string payload, string recipient)
    {
        string url = "https://api-world-event-listener-dev.nysm.work/api/transaction-request/claim-request";

        // Tạo payload dạng anonymous type để chuyển thành JSON
        ClaimRequestData postData = new ClaimRequestData()
        {
            hubAddress = hubAddress,
            tokenAddress = tokenAddress,
            amount = amount,
            payload = payload,
            recipient = recipient
        };

        string jsonBody = JsonUtility.ToJson(postData);

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("accept", "application/json");
        req.SetRequestHeader("x-secret", "$qAUkHX_#PC{,?>");
        req.SetRequestHeader("Content-Type", "application/json");
        Debug.Log(jsonBody);
        yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (req.result != UnityWebRequest.Result.Success)
#else
    if (req.isNetworkError || req.isHttpError)
#endif
        {
            Debug.LogError("Claim API lỗi: " + req.error + "\nResponse: " + req.downloadHandler.text);
         
        }
        else
        {
            Debug.Log("Claim API thành công! Response: " + req.downloadHandler.text);

            ClaimPayload result = JsonUtility.FromJson<ClaimPayload>(req.downloadHandler.text);
            CallClaim(addContract, result);
        }
    }



    [DllImport("__Internal")]
    private static extern void ClaimFromUnity(string contract, string payloadJson);

    public void CallClaim(string contract, ClaimPayload payload)
    {
        string payloadJson = JsonUtility.ToJson(payload);
        Debug.Log("Claim payload: " + payloadJson);
        ClaimFromUnity(contract, payloadJson);

    }

    // Callback từ JS trả về
    public void OnClaimResult(string json)
    {
        Debug.Log("ClaimResult: " + json);
        var result = JsonUtility.FromJson<ClaimResult>(json);
        if (result.status == "success")
        {
            Debug.Log("Claim success! TxHash: " + result.hash);
            OnClaimSuccess?.Invoke(result);
            // Xử lý thành công ở đây
        }
        else
        {
            Debug.LogError("Claim fail: " + result.message);
            OnClaimFail?.Invoke(result);

            // Xử lý thất bại ở đây
            GameManager.Instance.ShowMess("User reject!");
        }
    }


    // Định nghĩa payload cho claim (phải giống với cấu trúc claim phía JS)
    [Serializable]
    public class ClaimPayload
    { 
        public string id;
        public string chainId;
        public string tokenAddress;
        public string amount;
        public string payload;
        public string expiredBlockNumber;
        public string signature;
        public string adminAddress;
        public string recipient;
        
    }

    #endregion

    #endregion

    #region Get User Name
    [DllImport("__Internal")]
    private static extern void GetUserDataFromUnity(string userAddress, IntPtr callbackFunc);

    public delegate void UserDataCallback(string jsonResult);

    public void CallGetUserData(string userAddress, UserDataCallback callback)
    {
        _getUserDataCallback = callback;
        GetUserDataFromUnity(userAddress, Marshal.GetFunctionPointerForDelegate((Action<string>)InvokeGetUserDataCallback));
    }

    private static UserDataCallback _getUserDataCallback;

    [AOT.MonoPInvokeCallback(typeof(Action<string>))]
    public static void InvokeGetUserDataCallback(string jsonResult)
    {
        Debug.Log(jsonResult); // log JSON trả về để debug!
        if (_getUserDataCallback != null)
        {
            _getUserDataCallback(jsonResult);
            _getUserDataCallback = null;
        }
    }
    #endregion

    #region OpenSwap WLD
    [DllImport("__Internal")]
    private static extern void OpenSwapWldFromUnity();

    public void OpenSwapWld()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    OpenSwapWldFromUnity();
#else
        Application.OpenURL("https://world.org/mini-app?app_id=app_0d4b759921490adc1f2bd569fda9b53a&path=%2Ftoken%2Fbuy%3Faddress%3D0x2cFc85d8E48F8EAB294be644d9E25C3030863003");
#endif
    }

    #endregion
}
