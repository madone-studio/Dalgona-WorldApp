using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    string saveURL = "https://YOUR_CLOUD_RUN_URL/api/save-score";

    public void SaveScore(string userId, int score)
    {
        StartCoroutine(SaveScoreRoutine(userId, score));
    }

    IEnumerator SaveScoreRoutine(string userId, int score)
    {
        var json = JsonUtility.ToJson(new ScoreData { userId = userId, score = score });
        UnityWebRequest www = new UnityWebRequest(saveURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log("Score saved!");
        else
            Debug.LogError(www.error);
    }

    [System.Serializable]
    public class ScoreData
    {
        public string userId;
        public int score;
    }

    string getURL = "https://YOUR_CLOUD_RUN_URL/api/get-scores?userId=USER_ID";

    IEnumerator GetScores()
    {
        UnityWebRequest www = UnityWebRequest.Get(getURL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log("Scores: " + www.downloadHandler.text);
        else
            Debug.LogError(www.error);
    }
}
