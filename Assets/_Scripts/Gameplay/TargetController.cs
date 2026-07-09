using UnityEngine;

public class TargetController : MonoBehaviour
{
  [SerializeField] private float rotationSpeed = 50f;
  [SerializeField] private bool isClockwise = true;

  private float currentSpeed = 0f;
  private float targetSpeed;
  private float directionMultiplier => isClockwise ? -1f : 1f;

  void Start()
  {
    var config = LevelManager.Instance?.CurrentLevel;
    if (config != null)
    {
      rotationSpeed = config.rotationSpeed;
      isClockwise = config.clockwise;
    }
    targetSpeed = rotationSpeed;
  }

  void Update()
  {
    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 100f * Time.deltaTime);

    float zIndex = directionMultiplier * currentSpeed * Time.deltaTime;
    transform.Rotate(0f, 0f, zIndex);
  }
}
