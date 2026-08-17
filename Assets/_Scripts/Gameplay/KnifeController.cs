using UnityEngine;

public enum KnifeState
{
  Unset,
  Queued,
  Prepared,
  Thrown,
  Stuck,
  Falling
}


public class Knife : MonoBehaviour
{

  public KnifeState State { get; private set; } = KnifeState.Unset;

  [SerializeField] private float speed = 12f;

  private Vector3 slideTarget;
  private float slideSpeed;

  private GameController controller;
  public void SetController(GameController c) => controller = c;

  private float originalColliderHeight;
  private float originalColliderOffsetY;
  private float originalColliderRadius;

  void Awake()
  {
    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
    if (ps != null)
    {
      ps.playOnAwake = false;
      ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    Collider2D col = GetComponent<Collider2D>();
    if (col is BoxCollider2D box)
    {
      originalColliderHeight = box.size.y;
      originalColliderOffsetY = box.offset.y;
    }
    else if (col is CircleCollider2D circle)
    {
      originalColliderRadius = circle.radius;
      originalColliderOffsetY = circle.offset.y;
    }
  }


  void Update()
  {
    switch (State)
    {
      case KnifeState.Unset:
      case KnifeState.Queued:
        transform.position = Vector3.MoveTowards(transform.position, slideTarget, slideSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, slideTarget) < 0.01f)
        {
          GetComponent<Collider2D>().enabled = true;

          if (State == KnifeState.Queued)
          {
            State = KnifeState.Thrown;
          }
          else
          {
            State = KnifeState.Prepared;
          }

        }
        break;

      case KnifeState.Thrown:
        transform.position += Vector3.up * speed * Time.deltaTime;
        break;

      case KnifeState.Falling:
        FallDown();
        break;

    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (State == KnifeState.Stuck) return;


    if (State == KnifeState.Thrown)
    {
      if (other.CompareTag("Knife"))
      {
        State = KnifeState.Falling;
        GameManager.Instance.SetState(GameState.Lost);
        SoundManager.Instance?.PlayKnifeMiss();
        PlayClashParticles();
        PlayClashSplash();
        ScreenDimmer.Instance?.Flash();
        controller?.OnKnifeMissed();
      }
      else if (other.CompareTag("Target"))
      {
        Stick(other.transform);
      }
    }

  }

  public void Prepare(Vector3 target, float speed)
  {
    slideTarget = target;
    slideSpeed = speed;
    GetComponent<Collider2D>().enabled = false;
    ResetColliderToFull();
  }

  public void Throw()
  {
    if (State == KnifeState.Stuck || State == KnifeState.Falling)
    {
      return;
    }

    if (State == KnifeState.Unset)
    {
      State = KnifeState.Queued;
    }
    else if (State == KnifeState.Prepared)
    {
      State = KnifeState.Thrown;
    }
  }

  public void SetStuck()
  {
    State = KnifeState.Stuck;
  }

  private void PlayClashParticles()
  {
    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
    if (ps == null) return;

    GameObject spawned = Instantiate(ps.gameObject, ps.transform.position, Quaternion.identity);
    spawned.transform.SetParent(null);
    spawned.GetComponent<ParticleSystem>()?.Play();
    Destroy(spawned, 1f);
  }

  private void PlayClashSplash()
  {
    Transform splashTemplate = transform.Find("ClashSplash");
    if (splashTemplate == null) return;

    GameObject spawned = Instantiate(splashTemplate.gameObject, splashTemplate.position, Quaternion.identity);
    spawned.transform.SetParent(null);
    spawned.SetActive(true);
    StartCoroutine(AnimateSplash(spawned.GetComponent<SpriteRenderer>()));
  }

  private System.Collections.IEnumerator AnimateSplash(SpriteRenderer sr)
  {
    if (sr == null) yield break;

    Transform t = sr.transform;
    t.localScale = Vector3.one * 1.0f;

    Color startColor = new Color(1f, 1f, 1f, 0.45f);
    Color endColor = new Color(1f, 1f, 1f, 0.12f);

    float duration = 0.04f;
    float elapsed = 0f;

    while (elapsed < duration)
    {
      sr.color = Color.Lerp(startColor, endColor, elapsed / duration);
      elapsed += Time.deltaTime;
      yield return null;
    }

    Destroy(sr.gameObject);
  }

  private void Stick(Transform target)
  {
    State = KnifeState.Stuck;
    transform.SetParent(target.Find("KnifeHolder"));

    float radius = controller != null ? controller.LogRadius : 0.76f;
    Vector2 localPos = new Vector2(transform.localPosition.x, transform.localPosition.y);
    if (localPos.magnitude > 0.01f)
    {
      localPos = localPos.normalized * radius;
    }

    float angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
    transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);
    transform.localPosition = new Vector3(localPos.x, localPos.y, 0.0f);

    // Shrink collider to only cover visible part
    ShrinkColliderToVisiblePart();

    target.GetComponent<TargetController>()?.OnHit(transform.position);

    controller?.OnSpawnKnife();
  }

  public void ShrinkColliderToVisiblePart()
  {
    Collider2D col = GetComponent<Collider2D>();
    if (col == null) return;

    if (col is BoxCollider2D box)
    {
      float originalHeight = box.size.y;
      float visibleHeight = originalHeight * 0.4f;
      float offsetFromCenter = (originalHeight / 2f) + (visibleHeight / 2f);
      // float offsetFromCenter = visibleHeight;

      box.size = new Vector2(box.size.x, visibleHeight);
      box.offset = new Vector2(box.offset.x, -offsetFromCenter);
    }
    else if (col is CircleCollider2D circle)
    {
      float originalRadius = circle.radius;
      float visibleRadius = originalRadius * 0.4f;
      float offsetFromCenter = originalRadius - visibleRadius;

      circle.radius = visibleRadius;
      circle.offset = new Vector2(circle.offset.x, offsetFromCenter);
    }
  }

  public void ResetColliderToFull()
  {
    Collider2D col = GetComponent<Collider2D>();
    if (col == null) return;

    if (col is BoxCollider2D box)
    {
      box.size = new Vector2(box.size.x, originalColliderHeight);
      box.offset = new Vector2(box.offset.x, originalColliderOffsetY);
    }
    else if (col is CircleCollider2D circle)
    {
      circle.radius = originalColliderRadius;
      circle.offset = new Vector2(circle.offset.x, originalColliderOffsetY);
    }
  }

  private void FallDown()
  {
    transform.position += Vector3.down * (slideSpeed / 10) * Time.deltaTime;
    transform.Rotate(0, 0, 360 * Time.deltaTime);
  }
}
