using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private float score;

    void Update()
    {
        //saniye arttikca skor artsin
        score += Time.deltaTime;
        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }
}
