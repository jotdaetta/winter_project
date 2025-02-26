using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameFade : MonoBehaviour
{
    [SerializeField] Image transitionImg;
    public float transitionTime;
    void Start()
    {
        StartCoroutine(FadeIO(false));
    }

    IEnumerator FadeIO(bool isIn = true)
    {
        float elapsedTime = 0f;
        Color color = transitionImg.color;
        int cur = isIn ? 0 : 1;
        int des = isIn ? 1 : 0;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            color.a = Mathf.Lerp(cur, des, t);
            transitionImg.color = color;
            yield return null;
        }
    }
    public void Fade(bool isIn = true)
    {
        StartCoroutine(FadeIO(isIn));
    }
}
