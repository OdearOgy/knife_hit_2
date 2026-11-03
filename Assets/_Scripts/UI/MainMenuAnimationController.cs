using UnityEngine;
using System.Collections;

public class MainMenuAnimationController : MonoBehaviour
{
  [Header("UI Elements")]
  [SerializeField] private Transform knife;
  [SerializeField] private Transform logoKnife; // KNIFE word
  [SerializeField] private Transform logoHit; // HIT word
  [SerializeField] private Transform slashLine; // White curved line / middle knife
  [SerializeField] private GameObject stageScoreContainer; // Parent of stage, emoji, and score
  [SerializeField] private SpriteRenderer emojiSprite; // Emoji sprite renderer (separate because it's not UI)
  [SerializeField] private Transform playButton;

  [Header("Animation Settings")]
  [SerializeField] private float knifeRiseDuration = 0.5f;
  [SerializeField] private float logoTextDuration = 0.4f;
  [SerializeField] private float slashDuration = 0.25f;
  [SerializeField] private float textFadeDuration = 0.3f;
  [SerializeField] private float buttonFadeDuration = 0.3f;
  [SerializeField] private float sequenceDelay = 0.15f;

  [Header("Logo Animation")]
  [SerializeField] private float sideDistance = 500f;   // How far KNIFE/HIT start from their final spot
  [SerializeField] private float slashDistance = 600f;  // How far the slash starts from its final spot

  [Header("Pulse Settings")]
  [SerializeField] private float buttonPulseScale = 1.05f;
  [SerializeField] private float buttonPulseDuration = 1f;
  [SerializeField] private float buttonPressScale = 0.9f;
  [SerializeField] private float buttonPressDuration = 0.1f;

  private Coroutine buttonPulseCoroutine;
  private Coroutine buttonPressCoroutine;

  // Final (Editor) positions of logo parts
  private Vector3 knifeFinal, hitFinal, slashFinal;

  private void Start()
  {
    StartCoroutine(PlayIntroAnimation());
  }

  private IEnumerator PlayIntroAnimation()
  {
    // Reset initial states
    Vector3 knifeTargetPos = Vector3.zero;

    // Prepare logo parts
    PrepareLogo();

    if (knife != null)
    {
      Vector3 knifeOriginalPos = knife.position; // Store original position
      knifeTargetPos = knifeOriginalPos; // Final position is original position
      float bottomY = GetScreenBottomY();
      knife.position = new Vector3(knifeOriginalPos.x, bottomY - 200f, knifeOriginalPos.z);

      CanvasGroup knifeCanvasGroup = knife.GetComponent<CanvasGroup>();
      if (knifeCanvasGroup == null)
      {
        knifeCanvasGroup = knife.gameObject.AddComponent<CanvasGroup>();
      }
      knifeCanvasGroup.alpha = 0f;
    }

    if (stageScoreContainer != null)
    {
      CanvasGroup containerCanvasGroup = stageScoreContainer.GetComponent<CanvasGroup>();
      if (containerCanvasGroup == null)
      {
        containerCanvasGroup = stageScoreContainer.AddComponent<CanvasGroup>();
      }
      containerCanvasGroup.alpha = 0f;
    }

    if (emojiSprite != null)
    {
      Color emojiColor = emojiSprite.color;
      emojiColor.a = 0f;
      emojiSprite.color = emojiColor;
    }

    if (playButton != null)
    {
      CanvasGroup buttonCanvasGroup = playButton.GetComponent<CanvasGroup>();
      if (buttonCanvasGroup == null)
      {
        buttonCanvasGroup = playButton.gameObject.AddComponent<CanvasGroup>();
      }
      buttonCanvasGroup.alpha = 0f;
      playButton.localScale = Vector3.one;
    }

    // 1. Knife animation (starts first)
    if (knife != null)
    {
      CanvasGroup knifeCanvasGroup = knife.GetComponent<CanvasGroup>();

      yield return StartCoroutine(FadeIn(knifeCanvasGroup, knifeRiseDuration));
      yield return StartCoroutine(MoveY(knife, knifeTargetPos.y, knifeRiseDuration, EaseType.OutQuad));
    }

    // 2. Logo animation (delayed)
    yield return new WaitForSeconds(sequenceDelay);
    yield return StartCoroutine(PlayLogo());

    // 3. Text fade in (delayed) - container fades in
    yield return new WaitForSeconds(sequenceDelay);

    Coroutine containerFade = null;
    Coroutine emojiFade = null;

    if (stageScoreContainer != null)
    {
      CanvasGroup containerCanvasGroup = stageScoreContainer.GetComponent<CanvasGroup>();
      if (containerCanvasGroup != null)
      {
        containerFade = StartCoroutine(FadeIn(containerCanvasGroup, textFadeDuration));
      }
    }

    if (emojiSprite != null)
    {
      emojiFade = StartCoroutine(FadeInSprite(emojiSprite, textFadeDuration));
    }

    // Wait for both to finish
    if (containerFade != null) yield return containerFade;
    if (emojiFade != null) yield return emojiFade;

    // 4. Play button fade in and pulse (delayed)
    yield return new WaitForSeconds(sequenceDelay);

    if (playButton != null)
    {
      CanvasGroup buttonCanvasGroup = playButton.GetComponent<CanvasGroup>();
      yield return StartCoroutine(FadeIn(buttonCanvasGroup, buttonFadeDuration));
      buttonPulseCoroutine = StartCoroutine(ButtonPulse());
    }
  }

  // ---------- Logo ----------

  private void SetAlpha(Transform t, float a)
  {
    if (t == null) return;
    CanvasGroup cg = t.GetComponent<CanvasGroup>();
    if (cg == null) cg = t.gameObject.AddComponent<CanvasGroup>();
    cg.alpha = a;
  }

  // Remembers the Editor positions as final positions and moves everything to the start state
  private void PrepareLogo()
  {
    if (logoKnife != null)
    {
      knifeFinal = logoKnife.position;
      // Start from left
      logoKnife.position = new Vector3(knifeFinal.x - sideDistance, knifeFinal.y, knifeFinal.z);
      // Keep alpha at 1 (visible)
      SetAlpha(logoKnife, 1f);
    }

    if (logoHit != null)
    {
      hitFinal = logoHit.position;
      // Start from right
      logoHit.position = new Vector3(hitFinal.x + sideDistance, hitFinal.y, hitFinal.z);
      // Keep alpha at 1 (visible)
      SetAlpha(logoHit, 1f);
    }

    if (slashLine != null)
    {
      slashFinal = slashLine.position;
      // Start from left
      slashLine.position = new Vector3(slashFinal.x - slashDistance, slashFinal.y, slashFinal.z);
      // Keep alpha at 1 (visible)
      SetAlpha(slashLine, 1f);
    }
  }

  private IEnumerator PlayLogo()
  {
    // 1. KNIFE from left (with overshoot)
    if (logoKnife != null)
    {
      yield return StartCoroutine(MoveX(logoKnife, knifeFinal.x, logoTextDuration, EaseType.OutBack));
    }

    yield return new WaitForSeconds(sequenceDelay);

    // 2. HIT from right (with overshoot)
    if (logoHit != null)
    {
      yield return StartCoroutine(MoveX(logoHit, hitFinal.x, logoTextDuration, EaseType.OutBack));
    }

    yield return new WaitForSeconds(sequenceDelay);

    // 3. Slash from left
    if (slashLine != null)
    {
      yield return StartCoroutine(MoveX(slashLine, slashFinal.x, slashDuration, EaseType.OutQuad));
    }
  }

  // ---------- Idle animations ----------

  private IEnumerator ButtonPulse()
  {
    if (playButton == null) yield break;

    bool scalingUp = true;

    while (true)
    {
      Vector3 targetScale = scalingUp ? Vector3.one * buttonPulseScale : Vector3.one;
      float halfDuration = buttonPulseDuration / 2f;

      yield return StartCoroutine(Scale(playButton, targetScale, halfDuration, EaseType.InOutSine));
      scalingUp = !scalingUp;
    }
  }

  public void OnPlayButtonPress()
  {
    if (playButton == null) return;

    // Stop pulse animation temporarily
    if (buttonPulseCoroutine != null)
    {
      StopCoroutine(buttonPulseCoroutine);
      buttonPulseCoroutine = null;
    }

    if (buttonPressCoroutine != null)
    {
      StopCoroutine(buttonPressCoroutine);
    }

    buttonPressCoroutine = StartCoroutine(ButtonPressAnimation());
  }

  private IEnumerator ButtonPressAnimation()
  {
    yield return StartCoroutine(Scale(playButton, Vector3.one * buttonPressScale, buttonPressDuration, EaseType.OutQuad));
    yield return StartCoroutine(Scale(playButton, Vector3.one, buttonPressDuration, EaseType.OutQuad));

    // Resume pulse
    buttonPulseCoroutine = StartCoroutine(ButtonPulse());
  }

  // ---------- Helper Coroutines ----------

  private IEnumerator MoveX(Transform transform, float targetX, float duration, EaseType easeType)
  {
    float startX = transform.position.x;
    float elapsed = 0f;

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      float easedT = ApplyEase(t, easeType);
      Vector3 pos = transform.position;
      pos.x = Mathf.Lerp(startX, targetX, easedT);
      transform.position = pos;
      elapsed += Time.deltaTime;
      yield return null;
    }

    Vector3 finalPos = transform.position;
    finalPos.x = targetX;
    transform.position = finalPos;
  }

  private IEnumerator FadeInSprite(SpriteRenderer spriteRenderer, float duration)
  {
    float elapsed = 0f;
    Color startColor = spriteRenderer.color;

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      float easedT = EaseOutQuad(t);
      Color newColor = startColor;
      newColor.a = Mathf.Lerp(startColor.a, 1f, easedT);
      spriteRenderer.color = newColor;
      elapsed += Time.deltaTime;
      yield return null;
    }

    Color finalColor = startColor;
    finalColor.a = 1f;
    spriteRenderer.color = finalColor;
  }

  private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
  {
    float elapsed = 0f;
    float startAlpha = canvasGroup.alpha;

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      float easedT = EaseOutQuad(t);
      canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, easedT);
      elapsed += Time.deltaTime;
      yield return null;
    }

    canvasGroup.alpha = 1f;
  }

  private IEnumerator MoveY(Transform transform, float targetY, float duration, EaseType easeType)
  {
    float startY = transform.position.y;
    float elapsed = 0f;

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      float easedT = ApplyEase(t, easeType);
      Vector3 pos = transform.position;
      pos.y = Mathf.Lerp(startY, targetY, easedT);
      transform.position = pos;
      elapsed += Time.deltaTime;
      yield return null;
    }

    Vector3 finalPos = transform.position;
    finalPos.y = targetY;
    transform.position = finalPos;
  }

  private IEnumerator Scale(Transform transform, Vector3 targetScale, float duration, EaseType easeType)
  {
    Vector3 startScale = transform.localScale;
    float elapsed = 0f;

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      float easedT = ApplyEase(t, easeType);
      transform.localScale = Vector3.Lerp(startScale, targetScale, easedT);
      elapsed += Time.deltaTime;
      yield return null;
    }

    transform.localScale = targetScale;
  }

  // ---------- Easing Functions ----------

  private float ApplyEase(float t, EaseType easeType)
  {
    switch (easeType)
    {
      case EaseType.OutQuad:
        return EaseOutQuad(t);
      case EaseType.OutBack:
        return EaseOutBack(t);
      case EaseType.InOutSine:
        return EaseInOutSine(t);
      default:
        return t;
    }
  }

  private float EaseOutBack(float t)
  {
    float c1 = 1.70158f;
    float c3 = c1 + 1f;
    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
  }

  private float EaseOutQuad(float t)
  {
    return 1f - (1f - t) * (1f - t);
  }

  private float EaseInOutSine(float t)
  {
    return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
  }

  private float GetScreenBottomY()
  {
    if (Camera.main == null) return 0f;
    return Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane)).y;
  }

  private float GetScreenTopY()
  {
    if (Camera.main == null) return 0f;
    return Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, Camera.main.nearClipPlane)).y;
  }

  private void OnDestroy()
  {
    // Clean up coroutines
    if (buttonPulseCoroutine != null)
    {
      StopCoroutine(buttonPulseCoroutine);
    }
    if (buttonPressCoroutine != null)
    {
      StopCoroutine(buttonPressCoroutine);
    }
  }

  private enum EaseType
  {
    OutQuad,
    OutBack,
    InOutSine
  }
}