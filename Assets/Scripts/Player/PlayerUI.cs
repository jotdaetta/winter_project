using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Text text_ammo;
    [SerializeField] Text ExecutionKey;
    [SerializeField] Slider slider_reload;

    public void SetAmmoText(int curammo, int totalammo)
    {
        text_ammo.text = $"{curammo} | {totalammo}";
    }

    // duration 초 동안 슬라이더를 0에서 1로 채움
    public void ReloadSlider(float duration)
    {
        StartCoroutine(FillSlider(duration));
    }

    private IEnumerator FillSlider(float duration)
    {
        slider_reload.value = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 진행 비율에 따라 slider.value를 설정 (0~1 사이)
            slider_reload.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        slider_reload.value = 0f;
    }

    public void PadExecution(bool parm)
    {
        ExecutionKey.text = parm ? "X" : "I";
        ExecutionKey.color = parm ? new Color(0.08962262f, 0.390222f, 1) : Color.white;
    }
}
