using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
  [SerializeField] private Button restartButton;
  [SerializeField] private Button homeButton;

  void Start()
  {
    if (restartButton != null)
      restartButton.onClick.AddListener(OnRestart);

    if (homeButton != null)
      homeButton.onClick.AddListener(OnHome);
  }

  void OnRestart()
  {
    LevelManager.Instance?.RestartFromLevelOne();
    SceneManager.LoadScene("Gameplay");
  }

  void OnHome()
  {
    SceneManager.LoadScene("MainMenu");
  }
}
