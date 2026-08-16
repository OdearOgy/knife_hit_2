using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenDimmer : MonoBehaviour
{
  public static ScreenDimmer Instance { get; private set; }

  [SerializeField] private Image overlay;
  [SerializeField] private float dimAlpha = 0.35f;
  [SerializeField] private float flashDuration = 0.35f;

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;

    if (overlay == null)
    {
      overlay = GetComponentInChildren<Image>();
    }

    if (overlay != null)
      overlay.color = new Color(0f, 0f, 0f, 0f);
  }

  public void Flash()
  {
    if (overlay == null) return;
    StopAllCoroutines();
    StartCoroutine(DimRoutine());
  }

  private IEnumerator DimRoutine()
  {
    Color dim = new Color(0f, 0f, 0f, dimAlpha);
    Color clear = new Color(0f, 0f, 0f, 0f);

    overlay.color = dim;
    yield return new WaitForSeconds(flashDuration);

    float elapsed = 0f;
    float fadeOut = 0.2f;
    while (elapsed < fadeOut)
    {
      overlay.color = Color.Lerp(dim, clear, elapsed / fadeOut);
      elapsed += Time.deltaTime;
      yield return null;
    }

    overlay.color = clear;
  }
}
