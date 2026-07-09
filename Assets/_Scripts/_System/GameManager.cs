using UnityEngine;

public enum GameState
{
  Playing,
  Won,
  Lost
}

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  public GameState State { get; private set; } = GameState.Playing;


  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
  }

  public void SetState(GameState state)
  {
    State = state;
  }
}
