using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetBreakController : MonoBehaviour
{
  [Header("Visuals")]
  [SerializeField] private SpriteRenderer intactSprite;
  [SerializeField] private Transform fragmentRoot;

  [Header("Physics")]
  [SerializeField] private float explosionForce = 8f;
  [SerializeField] private float upwardBias = 2f;
  [SerializeField] private float torqueForce = 10f;
  [SerializeField] private float gravityScale = 1.5f;

  [Header("Timing")]
  [SerializeField] private float breakDelay = 0.1f;
  [SerializeField] private float fadeDuration = 1.5f;

  private List<SpriteRenderer> fragmentRenderers = new List<SpriteRenderer>();
  private List<Rigidbody2D> fragmentBodies = new List<Rigidbody2D>();
  private bool hasBroken = false;

  private void Awake()
  {
    if (intactSprite == null)
    {
      Transform logSprite = transform.Find("LogSprite");
      if (logSprite != null)
        intactSprite = logSprite.GetComponent<SpriteRenderer>();
    }

    if (fragmentRoot == null)
    {
      fragmentRoot = transform.Find("LogSprite/Akacia");
      if (fragmentRoot == null)
        fragmentRoot = transform.Find("Akacia");
    }

    CacheFragments();
  }

  private void CacheFragments()
  {
    if (fragmentRoot == null) return;

    foreach (Transform child in fragmentRoot)
    {
      if (!child.gameObject.activeSelf) continue;

      SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
      if (sr == null) sr = child.GetComponentInChildren<SpriteRenderer>();

      Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
      if (rb == null) rb = child.gameObject.AddComponent<Rigidbody2D>();

      if (child.GetComponent<Collider2D>() == null && rb != null)
        child.gameObject.AddComponent<BoxCollider2D>();

      if (sr != null)
        fragmentRenderers.Add(sr);

      if (rb != null)
      {
        rb.gravityScale = 0f;
        rb.isKinematic = true;
        fragmentBodies.Add(rb);
      }
    }
  }

  public void Break()
  {
    if (hasBroken) return;
    hasBroken = true;

    StartCoroutine(BreakRoutine());
  }

  private IEnumerator BreakRoutine()
  {
    // yield return new WaitForSeconds(breakDelay);

    // Hide only the brown circle intact sprite
    if (intactSprite != null)
      intactSprite.enabled = false;

    // Hide flash overlay too
    Transform flashOverlay = transform.Find("FlashOverlay");
    if (flashOverlay != null)
      flashOverlay.gameObject.SetActive(false);

    // Unparent the fragment root so it stops rotating with the target
    if (fragmentRoot != null)
    {
      Vector3 worldPos = fragmentRoot.position;
      Quaternion worldRot = fragmentRoot.rotation;
      fragmentRoot.SetParent(null);
      fragmentRoot.position = worldPos;
      fragmentRoot.rotation = worldRot;
    }

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

    // Cleanup
    if (fragmentRoot != null)
      Destroy(fragmentRoot.gameObject);
  }
}
