using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
public class Snake2 : MonoBehaviour
{
    private string apiUrl = "http://127.0.0.1:5000/api/games";
    public gameaudio gameaudio;
    public UI gameUI;
    public name inputsql;
    public TMP_InputField nameInputField;
    public TMP_Text scoreText;
    Vector3 direct;
    public float speeds;
    public Transform BodyPref;
    public List<Transform> bodies = new List<Transform>();
    void Start()
    {
        Time.timeScale = speeds;
        ResetStage();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && direct!=Vector3.down)
        {
            direct = Vector3.up;
            
        }
        if (Input.GetKeyDown(KeyCode.A) && direct != Vector3.right)
        {
            direct = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S) && direct != Vector3.up)
        {
            direct = Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.D) && direct != Vector3.left)
        {
            direct = Vector3.right;
        }
    }
    private void FixedUpdate()
    {for(int i = bodies.Count - 1; i > 0; i--)
        {
            bodies[i].position = bodies[i - 1].position;
        }
        transform.Translate(direct);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {if(collision.CompareTag("food"))
        { bodies.Add(Instantiate(BodyPref,
            transform.position,
            Quaternion.identity));
            gameUI.AddScore();
        }
    
        if (collision.CompareTag("wall"))
        {
            SendGameScore(nameInputField.text, int.Parse(scoreText.text));
            //sqlinput(nameInputField.text, scoreText.text);
            ResetStage();
            gameaudio.Replay();
           
        }
    }
    void ResetStage()
    {
        transform.position = Vector3.zero;
        direct = Vector3.zero;
        for (int i = 1; i < bodies.Count; i++)
        {
            Destroy(bodies[i].gameObject);
        }
        bodies.Clear();
        bodies.Add(transform);
        gameUI.ResetScore();
    }
    

    public void SendGameScore(string playerName, int score)
    {
        StartCoroutine(SendScoreCoroutine(playerName, score));
    }
    [System.Serializable]
    public class GameScore
    {
        public string snakename;
        public int score;
    }

    private IEnumerator SendScoreCoroutine(string playerName, int score)
    {
        // 創建 GameScore 物件並賦值
        GameScore gameScore = new GameScore();
        gameScore.snakename = playerName;
        gameScore.score = score;

        // 使用 JsonUtility 將物件轉換為 JSON 格式
        string jsonData = JsonUtility.ToJson(gameScore);
        Debug.Log("Sending JSON data: " + jsonData);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // 發送 POST 請求
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 發送請求
            yield return request.SendWebRequest();

            // 檢查回應結果
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score uploaded successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error uploading score: " + request.error);
            }
        }
    }
    void sqlinput(string name, string score)//將分數直接傳到mysql
    {
        string server = "localhost";
        string database = "sql_tutorial";
        string user = "root";
        string password = "@9861023";
        string connString = "Server=" + server + ";Database=" + database + ";User ID=" + user + ";Password=" + password + ";SslMode=None;";


        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            conn.Open();
            Debug.Log("✅ 成功連接 MySQL！");

            string query = "INSERT INTO snakescore (snakename, score) VALUES (@snakename, @score)";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@snakename", name);  // 玩家名稱
                cmd.Parameters.AddWithValue("@score", score);  // 玩家得分

                // 執行查詢
                cmd.ExecuteNonQuery();
            }
        }


    }
}
// 用來表示要送到 API 的資料
