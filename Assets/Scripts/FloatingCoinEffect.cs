using UnityEngine;
using System.Collections;

public class FloatingCoinEffect : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
    }

    private void OnEnable()
    {
        if (sr != null) sr.color = originalColor;
        StopAllCoroutines();
        StartCoroutine(FloatRoutine());
    }

    private IEnumerator FloatRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = startPos + Vector3.up * (floatSpeed * t);

            if (sr != null)
            {
                Color c = originalColor;
                c.a = alphaCurve.Evaluate(t);
                sr.color = c;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
