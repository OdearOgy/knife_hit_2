using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
  public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
  public void LoadGameplay() => SceneManager.LoadScene("Gameplay");
  public void LoadGameOver() => SceneManager.LoadScene("GameOver");

  public void RestartGameplay()
  {
    GameManager.Instance?.ResetScore();
    LevelManager.Instance?.RestartFromLevelOne();
    SceneManager.LoadScene("Gameplay");
  }
}
