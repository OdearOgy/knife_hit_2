using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
  [SerializeField] private float slideInSpeed = 25f;
  [SerializeField] private float spawnOffset = 2.5f;

  [SerializeField] private Knife knifePrefab;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private Transform targetTransform;

  private Knife currentKnife;
  private int knivesRemaining;


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

    if (knivesRemaining == 0)
    {
      GameManager.Instance.SetState(GameState.Won);
      StartCoroutine(WinLevelAfterDelay());
      return;
    }

    SpawnKnife();
  }

  public void OnKnifeMissed()
  {
    currentKnife = null;
    StartCoroutine(GameOverAfterDelay());
  }

  IEnumerator WinLevelAfterDelay()
  {
    yield return new WaitForSeconds(1f);
    Debug.Log($"[GameController] WinLevelAfterDelay - LevelManager.Instance: {LevelManager.Instance != null}");
    if (LevelManager.Instance != null)
    {
      LevelManager.Instance.LoadNextLevel();
    }
    else
    {
      Debug.LogError("[GameController] LevelManager.Instance is null!");
    }
    ReloadScene();
  }

  IEnumerator GameOverAfterDelay()
  {
    yield return new WaitForSeconds(1f);
    LevelManager.Instance.RestartFromLevelOne();
    ReloadScene();
  }

  void ReloadScene()
  {
    GameManager.Instance.SetState(GameState.Playing);
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  void Start()
  {
    var config = LevelManager.Instance?.CurrentLevel;
    Debug.Log($"[GameController] Start - LevelManager.Instance: {LevelManager.Instance != null}, Config: {config?.levelName ?? "NULL"}");
    if (config != null)
    {
      knivesRemaining = config.knivesToThrow;
      SpawnStuckKnives(config);
      SpawnApples(config);
    }
    else
    {
      knivesRemaining = 8;
      Debug.LogWarning("[GameController] No level config found, using defaults");
    }

    SpawnKnife();
  }

  [SerializeField] private float logRadius = 1.5f; // Adjust to match your target sprite size

  void SpawnStuckKnives(LevelConfig config)
  {
    if (config.stuckKnifeAngles == null || targetTransform == null) return;

    Transform knifeHolder = targetTransform.Find("KnifeHolder");
    if (knifeHolder == null) return;

    foreach (float angle in config.stuckKnifeAngles)
    {
      // Convert angle to radians for position calculation
      float rad = angle * Mathf.Deg2Rad;

      // Position on the log's circumference
      Vector3 positionOnCircle = new Vector3(
        Mathf.Cos(rad) * logRadius,
        Mathf.Sin(rad) * logRadius,
        0
      );

      Knife stuckKnife = Instantiate(knifePrefab, knifeHolder);
      stuckKnife.transform.localPosition = positionOnCircle;

      // Rotate so knife points outward from center (blade embedded in log)
      // If your knife sprite points UP by default, add +90 to point outward
      stuckKnife.transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);

      stuckKnife.GetComponent<Collider2D>().enabled = true;
      stuckKnife.SetStuck();

      // Shrink collider to only cover visible part sticking out of log
      stuckKnife.ShrinkColliderToVisiblePart();
    }
  }

  void SpawnApples(LevelConfig config)
  {
    if (targetTransform == null) return;

    int appleCount = Random.Range(config.minApples, config.maxApples + 1);
    // Apple spawning placeholder - requires apple prefab
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
    if (knivesRemaining <= 0) return;

    knivesRemaining--;
    currentKnife.Throw();
  }
}
