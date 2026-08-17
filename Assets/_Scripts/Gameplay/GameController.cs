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
  [SerializeField] private KnifeCountUI knifeCountUI;

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
    targetTransform?.GetComponent<TargetBreakController>()?.Break();

    yield return new WaitForSeconds(1.2f);

    if (LevelManager.Instance != null)
    {
      LevelManager.Instance.LoadNextLevel();
    }
    ReloadScene();
  }

  IEnumerator GameOverAfterDelay()
  {
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

    if (config != null)
    {
      knivesRemaining = config.knivesToThrow;
      knifeCountUI?.Setup(config.knivesToThrow);

      if (targetTransform != null)
      {
        TargetController targetController = targetTransform.GetComponent<TargetController>();
        if (targetController != null)
        {
          targetController.OnPopupComplete += () =>
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
      else
      {
        SpawnStuckKnives(config);
        SpawnApples(config);
      }
    }

    SpawnKnife();
  }

  [SerializeField] private float logRadius = 1.5f;
  void SpawnStuckKnives(LevelConfig config)
  {
    if (config.stuckKnifeAngles == null || targetTransform == null) return;

    Transform knifeHolder = targetTransform.Find("KnifeHolder");
    if (knifeHolder == null) return;

    foreach (float angle in config.stuckKnifeAngles)
    {
      float rad = angle * Mathf.Deg2Rad;

      Vector3 positionOnCircle = new Vector3(
        Mathf.Cos(rad) * logRadius,
        Mathf.Sin(rad) * logRadius,
        0.0f
      );

      Knife stuckKnife = Instantiate(knifePrefab, knifeHolder);
      stuckKnife.transform.localPosition = positionOnCircle;
      stuckKnife.transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);
      stuckKnife.GetComponent<Collider2D>().enabled = true;
      stuckKnife.SetStuck();
      stuckKnife.ShrinkColliderToVisiblePart();
    }
  }

  void SpawnApples(LevelConfig config)
  {
    if (targetTransform == null) return;
    int appleCount = Random.Range(config.minApples, config.maxApples + 1);
  }

  void SpawnKnife()
  {
    Vector3 initialSpawnPoint = spawnPoint.localPosition - Vector3.up * spawnOffset;
    currentKnife = Instantiate(knifePrefab, initialSpawnPoint, Quaternion.identity);
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
