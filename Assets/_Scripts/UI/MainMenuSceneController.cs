using UnityEngine;
using TMPro;

public class MainMenuSceneController : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI highScoreText;
  [SerializeField] private TextMeshProUGUI maxStageText;

  void Start()
  {
    int highScore = PlayerPrefs.GetInt("HighScore", 0);
    int maxStage = PlayerPrefs.GetInt("MaxStage", 0);

    if (highScoreText != null)
      highScoreText.text = "SCORE " + highScore;

    if (maxStageText != null)
      maxStageText.text = "STAGE " + maxStage;
  }
}
