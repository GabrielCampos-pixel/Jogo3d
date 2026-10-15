using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// Faz o texto piscar de vez em quando (efeito de luz falhando).
[RequireComponent(typeof(Text))]
public class UIFlicker : MonoBehaviour
{
    Text t;
    Color baseColor;
    float next;

    void Awake()
    {
        t = GetComponent<Text>();
        baseColor = t.color;
        next = Time.unscaledTime + Random.Range(2f, 5f);
    }

    void Update()
    {
        if (Time.unscaledTime < next) return;
        next = Time.unscaledTime + Random.Range(2f, 6f);
        StartCoroutine(Flick());
    }

    IEnumerator Flick()
    {
        int times = Random.Range(2, 5);
        for (int i = 0; i < times; i++)
        {
            SetAlpha(0.25f);
            yield return new WaitForSecondsRealtime(0.05f);
            SetAlpha(1f);
            yield return new WaitForSecondsRealtime(0.07f);
        }
    }

    void SetAlpha(float a) => t.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * a);
}
