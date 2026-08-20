using UnityEngine;
using TMPro;

public class GameOverController : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI scoreText;
  [SerializeField] private TextMeshProUGUI stageText;

  void Start()
  {
    if (scoreText != null)
      scoreText.text = PlayerPrefs.GetInt("LastScore", 0).ToString();

    if (stageText != null)
      stageText.text = "STAGE " + PlayerPrefs.GetInt("LastStage", 1);
  }
}
