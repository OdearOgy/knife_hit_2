using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour
{
  [SerializeField] private float rotationSpeed = 50f;
  [SerializeField] private bool isClockwise = true;


  private float currentSpeed = 0f;
  private float targetSpeed;
  private float directionMultiplier => isClockwise ? -1f : 1f;

  private float popupDuration = 0.25f;
  private float popupOvershoot = 1.15f;
  private Vector3 originalScale;

  public event System.Action OnPopupComplete;

  private void Awake()
  {
    originalScale = transform.localScale;
    transform.localScale = Vector3.zero;
  }

  void Start()
  {
    var config = LevelManager.Instance?.CurrentLevel;
    if (config != null)
    {
      rotationSpeed = config.rotationSpeed;
      isClockwise = config.clockwise;
    }
    targetSpeed = rotationSpeed;

    StartCoroutine(PopupAnimation());
  }

  void Update()
  {
    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 100f * Time.deltaTime);

    float zIndex = directionMultiplier * currentSpeed * Time.deltaTime;
    transform.Rotate(0f, 0f, zIndex);
  }

  private IEnumerator PopupAnimation()
  {
    float elapsed = 0f;
    Vector3 finalScale = originalScale;

    float upDuration = popupDuration * 0.6f;
    while (elapsed < upDuration)
    {
      float t = elapsed / upDuration;
      t = t * (2f - t);
      transform.localScale = Vector3.Lerp(Vector3.zero, finalScale * popupOvershoot, t);
      elapsed += Time.deltaTime;
      yield return null;
    }

    float downDuration = popupDuration * 0.4f;
    elapsed = 0f;
    Vector3 overshootScale = finalScale * popupOvershoot;
    while (elapsed < downDuration)
    {
      float t = elapsed / downDuration;
      // smoothstep
      t = t * t * (3f - 2f * t);
      transform.localScale = Vector3.Lerp(overshootScale, finalScale, t);
      elapsed += Time.deltaTime;
      yield return null;
    }

    transform.localScale = finalScale;
    OnPopupComplete?.Invoke();
  }
}
