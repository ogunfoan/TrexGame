using System;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private float score;
    public TextMeshProUGUI highScoreText;
    private int highScore;


    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }
 
    void Update()
    {
        //saniye arttikca skor artsin
        score += Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString();
    }

    public void CheckHighScore()
    {
        int finalScore = Mathf.FloorToInt(score);

        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            highScoreText.text = "High Score: " + highScore;
        }
    }
}
