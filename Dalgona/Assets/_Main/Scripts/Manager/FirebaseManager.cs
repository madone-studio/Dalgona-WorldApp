using UnityEngine;

using System.Collections.Generic;
using FirebaseWebGL.Scripts.FirebaseBridge;

public class FirebaseManager : Singleton<FirebaseManager>
{

    // Gọi hàm này để lưu điểm user lên Firestore
    public void SaveScore(string userId, string userName, int score)
    {
        // Tạo chuỗi JSON cho document
        string json = $"{{\"username\":\"{userName}\",\"score\":{score}}}";

        // Gọi SetDocument của FirebaseWebGL
        FirebaseFirestore.SetDocument(
            "users",            // collection
            userId,             // document id
            json,               // value (JSON)
            gameObject.name,    // tên GameObject callback
            "OnSaveScoreSuccess",  // callback success
            "OnFirebaseError"      // callback fail
        );
    }

    // Callback khi lưu score thành công
    public void OnSaveScoreSuccess(string msg)
    {
        Debug.Log("Score saved! " + msg);
        // TODO: Hiển thị lên UI nếu muốn
    }


    // Lấy danh sách user top score
    public void GetTopRanking(int limit = 10)
    {
        FirebaseFirestore.GetDocumentsInCollection(
            "users",
            gameObject.name,
            "OnGetRankingSuccess", // callback thành công
            "OnFirebaseError"      // callback lỗi
        );
    }

    // Callback khi lấy ranking thành công
    public void OnGetRankingSuccess(string json)
    {
        Debug.Log("Ranking Data: " + json);
        // TODO: Parse JSON và hiển thị lên UI leaderboard

        // Ví dụ: Đọc danh sách username, score
        // Bạn có thể dùng SimpleJSON hoặc JsonUtility nếu muốn parse đẹp hơn
    }

    // Callback khi có lỗi chung
    public void OnFirebaseError(string err)
    {
        Debug.LogError("Firebase Error: " + err);
        // TODO: Hiển thị thông báo lỗi lên UI
    }
}
