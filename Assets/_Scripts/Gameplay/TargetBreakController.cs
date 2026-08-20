using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetBreakController : MonoBehaviour
{
  [Header("Visuals")]
  [SerializeField] private SpriteRenderer intactSprite;

  [Header("Physics")]
  [SerializeField] private float explosionForce = 8f;
  [SerializeField] private float upwardBias = 2f;
  [SerializeField] private float torqueForce = 10f;
  [SerializeField] private float gravityScale = 1.5f;

  [Header("Timing")]
  [SerializeField] private float fadeDuration = 1.5f;

  private List<GameObject> fragmentObjects = new List<GameObject>();
  private List<SpriteRenderer> fragmentRenderers = new List<SpriteRenderer>();
  private List<Rigidbody2D> fragmentBodies = new List<Rigidbody2D>();
  private bool hasBroken = false;

  private void Awake()
  {
    if (intactSprite == null)
    {
      Transform logSprite = transform.Find("LogSprite");
      if (logSprite == null) logSprite = transform.Find("BossSprite");
      if (logSprite != null)
        intactSprite = logSprite.GetComponent<SpriteRenderer>();
    }

    if (intactSprite == null)
      intactSprite = GetComponentInChildren<SpriteRenderer>(true);
  }

  private void CacheFragments()
  {
    fragmentObjects.Clear();
    fragmentRenderers.Clear();
    fragmentBodies.Clear();

    GameObject[] tagged = GameObject.FindGameObjectsWithTag("LogMaterial");
    foreach (GameObject go in tagged)
    {
      if (!go.activeSelf) continue;

      foreach (Transform child in go.transform)
      {
        if (!child.gameObject.activeSelf) continue;

        SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
        if (sr == null) sr = child.GetComponentInChildren<SpriteRenderer>();

        Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
        if (rb == null) rb = child.gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        if (child.GetComponent<Collider2D>() == null)
          child.gameObject.AddComponent<BoxCollider2D>();

        if (sr != null)
          fragmentRenderers.Add(sr);

        fragmentBodies.Add(rb);
        fragmentObjects.Add(child.gameObject);
      }
    }
  }

  public void Break()
  {
    if (hasBroken) return;
    hasBroken = true;

    if (IsBoss())
    {
      StartCoroutine(BreakBoss());
    }
    else
    {
      CacheFragments();
      StartCoroutine(BreakRoutine());
    }
  }

  private bool IsBoss()
  {
    return transform.Find("BossSprite") != null;
  }

  private IEnumerator BreakBoss()
  {
    SoundManager.Instance?.PlayBossBreak();

    Transform bossSprite = transform.Find("BossSprite/Boss");
    if (bossSprite != null)
      bossSprite.gameObject.SetActive(false);

    Transform flashOverlay = transform.Find("FlashOverlay");
    if (flashOverlay != null)
      flashOverlay.gameObject.SetActive(false);

    ReleaseStuckKnives();

    Transform bossParticles = transform.Find("BossSprite/BossParticles");
    if (bossParticles != null)
    {
      Vector3 worldPos = bossParticles.position;
      Quaternion worldRot = bossParticles.rotation;
      bossParticles.SetParent(null);
      bossParticles.position = worldPos;
      bossParticles.rotation = worldRot;

      bossParticles.gameObject.SetActive(true);
      ParticleSystem ps = bossParticles.GetComponent<ParticleSystem>();
      if (ps != null) ps.Play();
    }

    yield return new WaitForSeconds(2f);
  }

  private void ReleaseStuckKnives()
  {
    Transform knifeHolder = transform.Find("KnifeHolder");
    if (knifeHolder == null) return;

    List<Transform> stuckKnives = new List<Transform>();
    foreach (Transform child in knifeHolder)
    {
      stuckKnives.Add(child);
    }

    foreach (var knife in stuckKnives)
    {
      Vector3 worldScale = knife.lossyScale;
      Vector3 worldPos = knife.position;
      Quaternion worldRot = knife.rotation;
      knife.SetParent(null);
      knife.localScale = worldScale;
      knife.position = worldPos;
      knife.rotation = worldRot;

      Rigidbody2D rb = knife.GetComponent<Rigidbody2D>();
      if (rb == null) rb = knife.gameObject.AddComponent<Rigidbody2D>();

      rb.isKinematic = false;
      rb.simulated = true;
      rb.gravityScale = gravityScale * 0.5f;
      rb.constraints = RigidbodyConstraints2D.None;
      rb.WakeUp();

      rb.angularDamping = 0f;
      rb.angularVelocity = Random.Range(90f, 180f) * (Random.value > 0.5f ? 1f : -1f);

      Vector2 dir = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(0.3f, 1.2f)).normalized;
      rb.AddForce(dir * explosionForce * 0.5f, ForceMode2D.Impulse);
    }
  }

  private IEnumerator BreakRoutine()
  {
    SoundManager.Instance?.PlayTargetBreak();

    if (intactSprite != null)
      intactSprite.enabled = false;

    Transform flashOverlay = transform.Find("FlashOverlay");
    flashOverlay?.gameObject.SetActive(false);

    yield return null;

    foreach (GameObject go in fragmentObjects)
    {
      Transform t = go.transform;
      Vector3 worldPos = t.position;
      Quaternion worldRot = t.rotation;
      t.SetParent(null);
      t.position = worldPos;
      t.rotation = worldRot;
    }

    ReleaseStuckKnives();

    // Enable physics and launch each fragment
    for (int i = 0; i < fragmentBodies.Count; i++)
    {
      Rigidbody2D rb = fragmentBodies[i];
      if (rb == null) continue;

      rb.isKinematic = false;
      rb.gravityScale = gravityScale;

      Vector2 dir = (rb.transform.position - transform.position).normalized;
      dir += Vector2.up * upwardBias * 0.3f;
      dir.Normalize();

      rb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
      rb.AddTorque(Random.Range(-torqueForce, torqueForce), ForceMode2D.Impulse);

      StartCoroutine(Spin3D(rb.transform));
    }

    // Fade out fragments
    float elapsed = 0f;
    while (elapsed < fadeDuration)
    {
      float alpha = 1f - (elapsed / fadeDuration);
      foreach (var sr in fragmentRenderers)
      {
        if (sr != null)
        {
          Color c = sr.color;
          c.a = alpha;
          sr.color = c;
        }
      }
      elapsed += Time.deltaTime;
      yield return null;
    }

    foreach (GameObject go in fragmentObjects)
    {
      if (go != null)
        Destroy(go);
    }
  }

  private IEnumerator Spin3D(Transform t)
  {
    if (t == null) yield break;

    float speedX = Random.Range(-200f, 200f);
    float speedY = Random.Range(-200f, 200f);
    float speedZ = Random.Range(-100f, 100f);

    while (t != null)
    {
      t.Rotate(speedX * Time.deltaTime, speedY * Time.deltaTime, speedZ * Time.deltaTime);
      yield return null;
    }
  }
}
