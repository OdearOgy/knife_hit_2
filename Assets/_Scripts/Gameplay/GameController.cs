using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
  [SerializeField] private float slideInSpeed = 25f;
  [SerializeField] private float spawnOffset = 2.5f;

  [SerializeField] private PlayerConfig playerConfig;
  [SerializeField] private Knife knifePrefab;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private KnifeCountUI knifeCountUI;

  private Knife KnifePrefab => playerConfig != null && playerConfig.playerKnife != null
    ? playerConfig.playerKnife
    : knifePrefab;

  private Knife DefaultKnifePrefab => playerConfig != null && playerConfig.defaultKnife != null
    ? playerConfig.defaultKnife
    : knifePrefab;

  [SerializeField] private float logRadius = 0.76f;
  public float LogRadius => logRadius;

  private Knife currentKnife;
  private int knivesRemaining;
  private TargetController currentTarget;

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
    GameManager.Instance?.AddScore();

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
    GameManager.Instance?.SaveHighScore();
    currentTarget?.GetComponent<TargetBreakController>()?.Break();

    yield return new WaitForSeconds(1.2f);

    if (LevelManager.Instance != null)
    {
      LevelManager.Instance.LoadNextLevel();
    }
    ReloadScene();
  }

  IEnumerator GameOverAfterDelay()
  {
    GameManager.Instance?.SaveHighScore();
    yield return new WaitForSeconds(1f);
    GameManager.Instance.SetState(GameState.Playing);
    SceneManager.LoadScene("GameOver");
  }

  void ReloadScene()
  {
    GameManager.Instance.SetState(GameState.Playing);
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  void Start()
  {
    var config = LevelManager.Instance?.CurrentLevel;
    Debug.Log(LevelManager.Instance);

    if (config != null)
    {
      knivesRemaining = config.knivesToThrow;
      knifeCountUI?.Setup(config.knivesToThrow);

      if (config.targetPrefab != null)
      {
        currentTarget = Instantiate(config.targetPrefab, transform);
        currentTarget.transform.localPosition = new Vector3(0f, 2f, 0f);
        currentTarget.transform.localRotation = Quaternion.identity;
        currentTarget.OnPopupComplete += () =>
        {
          SpawnStuckKnives(config);
          SpawnApples(config);
        };
      }
      else
      {
        SpawnStuckKnives(config);
        SpawnApples(config);
      }
    }

    SpawnKnife();
  }

  void SpawnStuckKnives(LevelConfig config)
  {
    if (config.stuckKnifeAngles == null || config.stuckKnifeAngles.Length == 0 || currentTarget == null) return;

    Transform knifeHolder = currentTarget.transform.Find("KnifeHolder");
    if (knifeHolder == null) return;

    int stuckCount = Random.Range(config.minStuckKnives, config.maxStuckKnives + 1);
    for (int i = 0; i < stuckCount; i++)
    {
      float angle = config.stuckKnifeAngles[Random.Range(0, config.stuckKnifeAngles.Length)];
      float rad = angle * Mathf.Deg2Rad;

      Vector3 positionOnCircle = new Vector3(
        Mathf.Cos(rad) * logRadius,
        Mathf.Sin(rad) * logRadius,
        0.0f
      );

      Knife stuckKnife = Instantiate(DefaultKnifePrefab, knifeHolder);
      stuckKnife.transform.localPosition = positionOnCircle;
      stuckKnife.transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);
      stuckKnife.GetComponent<Collider2D>().enabled = true;
      stuckKnife.SetStuck();
      stuckKnife.ShrinkColliderToVisiblePart();
    }
  }

  void SpawnApples(LevelConfig config)
  {
    if (currentTarget == null) return;
    if (config.appleAngles == null || config.appleAngles.Length == 0) return;

    int appleCount = Random.Range(config.minApples, config.maxApples + 1);
    for (int i = 0; i < appleCount; i++)
    {
      float angle = config.appleAngles[Random.Range(0, config.appleAngles.Length)];
      float rad = angle * Mathf.Deg2Rad;

      Vector3 positionOnCircle = new Vector3(
        Mathf.Cos(rad) * logRadius,
        Mathf.Sin(rad) * logRadius,
        0.0f
      );

      // TODO: Instantiate apple prefab at positionOnCircle on target
      // For now just logging
      Debug.Log($"[GameController] Would spawn apple at angle {angle}, position {positionOnCircle}");
    }
  }

  void SpawnKnife()
  {
    Vector3 initialSpawnPoint = spawnPoint.localPosition - Vector3.up * spawnOffset;
    currentKnife = Instantiate(KnifePrefab, initialSpawnPoint, Quaternion.identity);
    currentKnife.SetController(this);

    currentKnife.Prepare(spawnPoint.localPosition, slideInSpeed);
  }

  void ThrowKnife()
  {
    if (GameManager.Instance.State != GameState.Playing) return;
    if (currentKnife == null) return;
    if (knivesRemaining <= 0) return;

    knivesRemaining--;
    knifeCountUI?.MarkOneUsed();
    currentKnife.Throw();
  }
}
