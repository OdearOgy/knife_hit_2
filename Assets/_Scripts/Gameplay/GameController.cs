using UnityEngine;

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
    var config = LevelManager.Instance?.CurrentLevel;
    if (config != null)
    {
      knivesRemaining = config.knivesToThrow;
      SpawnStuckKnives(config);
      SpawnApples(config);
    }
    else
    {
      knivesRemaining = 8;
    }

    SpawnKnife();
  }

  void SpawnStuckKnives(LevelConfig config)
  {
    if (config.stuckKnifeAngles == null || targetTransform == null) return;

    Transform knifeHolder = targetTransform.Find("KnifeHolder");
    if (knifeHolder == null) return;

    foreach (float angle in config.stuckKnifeAngles)
    {
      Knife stuckKnife = Instantiate(knifePrefab, knifeHolder);
      stuckKnife.transform.localPosition = Vector3.zero;
      stuckKnife.transform.localRotation = Quaternion.Euler(0, 0, angle);
      stuckKnife.GetComponent<Collider2D>().enabled = true;
      stuckKnife.SetStuck();
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
