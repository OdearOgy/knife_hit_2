using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
  [SerializeField] private Button restartButton;
  [SerializeField] private Button homeButton;

  private void Start()
  {
    if (restartButton == null)
      restartButton = GameObject.Find("Restart")?.GetComponent<Button>();
    if (homeButton == null)
      homeButton = GameObject.Find("Home")?.GetComponent<Button>();

    if (restartButton != null)
    {
      restartButton.onClick.RemoveAllListeners();
      restartButton.onClick.AddListener(OnRestart);
    }
    else
    {
      Debug.LogWarning("[GameOverController] Restart button not found. Create a UI Button named 'Restart' or assign it in the Inspector.");
    }

    if (homeButton != null)
    {
      homeButton.onClick.RemoveAllListeners();
      homeButton.onClick.AddListener(OnHome);
    }
    else
    {
      Debug.LogWarning("[GameOverController] Home button not found. Create a UI Button named 'Home' or assign it in the Inspector.");
    }
  }

  private void OnRestart()
  {
    LevelManager.Instance?.RestartFromLevelOne();
    GameSceneManager.Instance?.LoadGameplay();
  }

  private void OnHome()
  {
    GameSceneManager.Instance?.LoadMainMenu();
  }
}
