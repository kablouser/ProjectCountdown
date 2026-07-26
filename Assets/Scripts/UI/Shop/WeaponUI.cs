using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoText;

    public void SetAmmo(GunStat currGun)
    {
        ammoText.text = $"{currGun.currentAmmo}/{currGun.ammoCapacity}";
    }
}
