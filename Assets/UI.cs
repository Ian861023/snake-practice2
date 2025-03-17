using UnityEngine;
using TMPro;


public class UI : MonoBehaviour
{
    
    public TMP_Text scoreText;
    int score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ResetScore()
    {
       


        score = 0;
        scoreText.text = score.ToString();
        Debug.Log("Reset");
    }
    public void AddScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
    
}
