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
      stuckKnife.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
      
      stuckKnife.GetComponent<Collider2D>().enabled = true;
      stuckKnife.SetStuck();
      
      // Shrink collider to only cover visible part sticking out of log
      ShrinkColliderToVisiblePart(stuckKnife);
    }
  }
  
  void ShrinkColliderToVisiblePart(Knife stuckKnife)
  {
    Collider2D col = stuckKnife.GetComponent<Collider2D>();
    if (col == null) return;
    
    // The knife points outward from log center.
    // Bottom half is inside/hidden, top ~40% is visible and should collide.
    
    if (col is BoxCollider2D box)
    {
      float originalHeight = box.size.y;
      float visibleHeight = originalHeight * 0.4f; // Only top 40% sticks out
      float offsetFromCenter = (originalHeight / 2f) - (visibleHeight / 2f);
      
      box.size = new Vector2(box.size.x, visibleHeight);
      box.offset = new Vector2(box.offset.x, offsetFromCenter); // Shift toward blade tip
    }
    else if (col is CircleCollider2D circle)
    {
      float originalRadius = circle.radius;
      float visibleRadius = originalRadius * 0.4f;
      float offsetFromCenter = originalRadius - visibleRadius;
      
      circle.radius = visibleRadius;
      circle.offset = new Vector2(circle.offset.x, offsetFromCenter); // Shift toward blade tip
    }
    // Add CapsuleCollider2D etc. if needed
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
