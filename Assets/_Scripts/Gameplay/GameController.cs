
using UnityEngine;


public class GameController : MonoBehaviour
{
  [SerializeField] private float slideInSpeed = 25f;
  [SerializeField] private float spawnOffset = 2.5f;

  [SerializeField] private Knife knifePrefab;
  [SerializeField] private Transform spawnPoint;

  private Knife currentKnife;
  private int knifeCount;


  private void OnEnable()
  {
    if (InputManager.Instance != null)
    {
      InputManager.Instance.OnTap += ThrowKnife;
    }
  }

  private void OnDisable()
  {
    if (InputManager.Instance != null)
    {
      InputManager.Instance.OnTap -= ThrowKnife;
    }
  }

  public void OnSpawnKnife()
  {
    currentKnife = null;

    if (knifeCount == 0)
    {
      GameManager.Instance.SetState(GameState.Won);
      return;
    }


    SpawnKnife();
  }

  public void OnKnifeMissed()
  {
    currentKnife = null;
  }

  void Start()
  {
    knifeCount = LevelManager.Instance?.CurrentLevel?.knifeCount ?? 8;
    SpawnKnife();
  }

  void SpawnKnife()
  {
    Vector3 initialSpawnPoint = spawnPoint.position - Vector3.up * spawnOffset;
    currentKnife = Instantiate(knifePrefab, initialSpawnPoint, Quaternion.identity);
    currentKnife.SetController(this);
    currentKnife.Prepare(spawnPoint.position, slideInSpeed);
  }

  void ThrowKnife()
  {
    if (GameManager.Instance.State != GameState.Playing) return;
    if (currentKnife == null) return;
    if (knifeCount <= 0) return;

    knifeCount--;
    currentKnife.Throw();
  }
}
