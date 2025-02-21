using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Text text_ammo;
    [SerializeField] Slider slider_reload;

    public void SetAmmoText(int curammo, int totalammo)
    {
        text_ammo.text = $"{curammo} / {totalammo}";
    }
}
