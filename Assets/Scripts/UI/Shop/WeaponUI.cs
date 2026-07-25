using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Image weaponImage;
    [SerializeField] TextMeshProUGUI ammoText;

    public void SetAmmo(GunStat currGun)
    {
        weaponImage.sprite = currGun.weaponImage;
        ammoText.text = $"{currGun.currentAmmo}/{currGun.ammoCapacity}";
    }
}
