using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private CanvasGroup CrossHairGroup;
    [SerializeField] private RectTransform First;
    [SerializeField] private RectTransform Second;
    [SerializeField] private RectTransform Third;
    [SerializeField] private RectTransform Forth;
    [SerializeField] private BowAttack bowAttack;

    private Vector2 firstBase;
    private Vector2 secondBase;
    private Vector2 thirdBase;
    private Vector2 forthBase;

    private float normalGap = 0;
    private float aimGap = -8f;

    private float currentGap;
    private float targetGap;
    private float zoomSpeed;
    private Coroutine coroutine;
    private bool preAim;
    void Start()
    {
        firstBase = First.anchoredPosition;
        secondBase = Second.anchoredPosition;
        thirdBase = Third.anchoredPosition;
        forthBase = Forth.anchoredPosition;
    }
    private void Update()
    {
        // 활 조준 시작
        if (bowAttack.showCrosshair)
        {
            // 기존 Fade 중지
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            zoomSpeed = 4f;
            CrossHairGroup.alpha = 0.7f;
            targetGap = aimGap;
        }
        // 활 조준 종료
        if (!bowAttack.showCrosshair)
        {
            zoomSpeed = 12f;
            targetGap = normalGap;
        }

        currentGap = Mathf.Lerp(currentGap, targetGap, Time.deltaTime * zoomSpeed);

        ApplyCrosshair();
    }

    private void ApplyCrosshair()
    {
        First.anchoredPosition = firstBase + new Vector2(currentGap, currentGap);
        Second.anchoredPosition = secondBase + new Vector2(-currentGap, -currentGap);
        Third.anchoredPosition = thirdBase + new Vector2(currentGap, -currentGap);
        Forth.anchoredPosition = forthBase + new Vector2(-currentGap, currentGap);
    }
    public void RequestCorssHairFO()
    {
        if (coroutine != null)
            coroutine = StartCoroutine(CrossHairFO());
    }
    IEnumerator CrossHairFO()
    {
        float duration = 0.5f;

        yield return new WaitForSeconds(1f);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;

            float normalized = t / duration;
            CrossHairGroup.alpha = Mathf.Lerp(0.7f, 0f, normalized);

            yield return null;
        }
        CrossHairGroup.alpha = 0f;
        coroutine = null;
    }
}
