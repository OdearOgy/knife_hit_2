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

  [Header("Hit Feedback")]
  [SerializeField] private float nudgeDistance = 0.05f;
  [SerializeField] private float nudgeDuration = 0.06f;
  [SerializeField] private float flashDuration = 0.1f;
  [SerializeField] private GameObject flashOverlay;
  [SerializeField] private ParticleSystem hitParticles;

  private Vector3 basePosition;

  private void Awake()
  {
    originalScale = transform.localScale;
    transform.localScale = Vector3.zero;
    basePosition = transform.position;
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

  public void OnHit(Vector3 hitPosition)
  {
    StartCoroutine(NudgeUp());
    StartCoroutine(FlashWhite());
    PlayHitParticles(hitPosition);
    SoundManager.Instance?.PlayTargetHit();
  }

  private void PlayHitParticles(Vector3 hitPosition)
  {
    if (hitParticles == null) return;

    GameObject spawned = Instantiate(hitParticles.gameObject, hitPosition, Quaternion.identity);
    spawned.transform.SetParent(null);
    spawned.GetComponent<ParticleSystem>()?.Play();
    Destroy(spawned, 1f);
  }

  private IEnumerator NudgeUp()
  {
    Vector3 upPos = basePosition + new Vector3(0f, nudgeDistance, 0f);

    float elapsed = 0f;
    while (elapsed < nudgeDuration)
    {
      transform.position = Vector3.Lerp(basePosition, upPos, elapsed / nudgeDuration);
      elapsed += Time.deltaTime;
      yield return null;
    }
    transform.position = upPos;

    elapsed = 0f;
    while (elapsed < nudgeDuration)
    {
      transform.position = Vector3.Lerp(upPos, basePosition, elapsed / nudgeDuration);
      elapsed += Time.deltaTime;
      yield return null;
    }
    transform.position = basePosition;
  }

  private IEnumerator FlashWhite()
  {
    if (flashOverlay == null) yield break;

    flashOverlay.SetActive(true);
    yield return new WaitForSeconds(flashDuration);
    flashOverlay.SetActive(false);
  }
}
