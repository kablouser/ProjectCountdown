using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Image weaponImage;
    [SerializeField] TextMeshProUGUI ammoText;

    public void SetAmmo(GunStat currGun, int currAmmo, int maxAmmo)
    {
        weaponImage.sprite = currGun.weaponImage;
        ammoText.text = $"{currAmmo}/{maxAmmo}";
    }
}
