using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWeaponUI : MonoBehaviour
{
    public Button unlock;
    public TextMeshProUGUI unlockCostText;

    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI ammoCapacityText;
    public TextMeshProUGUI reloadSpeedText;
    public TextMeshProUGUI roundsPerMinuteText;

    public GameObject unlockedPanel;
    public Button damageUpgrade;
    public Button ammoUpgrade;
    public Button reloadUpgrade;
    public Button roundsPerMinuteUpgrade;

    public void SetName(in ShopScreenUI.ShopWeapon shopWeapon)
    {
        weaponName.SetText(shopWeapon.gunData.stats.name);
    }

    public void SetUnlocked(in ShopScreenUI.ShopWeapon shopWeapon)
    {
        if (shopWeapon.isUnlockedInShop)
        {
            unlockCostText.gameObject.SetActive(false);
            unlockedPanel.SetActive(true);

            if ((shopWeapon.gunData.upgradables & WeaponUpgradeFlags.Damage) != WeaponUpgradeFlags.None)
            {
                damageUpgrade.gameObject.SetActive(true);
                damageUpgrade.GetComponentInChildren<TextMeshProUGUI>().SetText($"Upgrade -{shopWeapon.gunData.upgradeCost}s");
            }
            else
            {
                damageUpgrade.gameObject.SetActive(false);
            }
            if ((shopWeapon.gunData.upgradables & WeaponUpgradeFlags.AmmoCapacity) != WeaponUpgradeFlags.None)
            {
                ammoUpgrade.gameObject.SetActive(true);
                ammoUpgrade.GetComponentInChildren<TextMeshProUGUI>().SetText($"Upgrade -{shopWeapon.gunData.upgradeCost}s");
            }
            else
            {
                ammoUpgrade.gameObject.SetActive(false);
            }
            if ((shopWeapon.gunData.upgradables & WeaponUpgradeFlags.ReloadSpeed) != WeaponUpgradeFlags.None)
            {
                reloadUpgrade.gameObject.SetActive(true);
                reloadUpgrade.GetComponentInChildren<TextMeshProUGUI>().SetText($"Upgrade -{shopWeapon.gunData.upgradeCost}s");
            }
            else
            {
                reloadUpgrade.gameObject.SetActive(false);
            }
            if ((shopWeapon.gunData.upgradables & WeaponUpgradeFlags.RoundsPerMinute) != WeaponUpgradeFlags.None)
            {
                roundsPerMinuteUpgrade.gameObject.SetActive(true);
                roundsPerMinuteUpgrade.GetComponentInChildren<TextMeshProUGUI>().SetText($"Upgrade -{shopWeapon.gunData.upgradeCost}s");
            }
            else
            {
                roundsPerMinuteUpgrade.gameObject.SetActive(false);
            }
            UpdateUpgradeTexts(shopWeapon);
        }
        else
        {
            unlockCostText.SetText($"Unlock -{shopWeapon.gunData.unlockCost}s");
            unlockedPanel.SetActive(false);
        }
    }

    public void UpdateUpgradeTexts(in ShopScreenUI.ShopWeapon shopWeapon)
    {
        UpdateSingleUpgradeText(damageText, "Damage", shopWeapon.gunData.stats.damage, shopWeapon.addDamage, 1f);
        UpdateSingleUpgradeText(ammoCapacityText, "Magazine", shopWeapon.gunData.stats.ammoCapacity, shopWeapon.addAmmoCapacity, 1f);
        UpdateSingleUpgradeText(reloadSpeedText, "Reload Speed", shopWeapon.gunData.stats.reloadTime, 0, shopWeapon.multiplyReloadSpeed);
        UpdateSingleUpgradeText(roundsPerMinuteText, "Rounds Per Min.", shopWeapon.gunData.stats.roundsPerMin, 0, shopWeapon.multiplyRoundsPerMinute);
    }

    public void UpdateSingleUpgradeText(TextMeshProUGUI text, string attributeName, float baseValue, float addValue, float multiplyValue)
    {
        if (0 < addValue)
        {
            text.SetText($"{attributeName}:\n{baseValue} + {addValue}");
        }
        else if (1f < multiplyValue)
        {
            text.SetText($"{attributeName}:\n{baseValue} + {(multiplyValue - 1f) * 100f:0.##}%");
        }
        else
        {
            text.SetText($"{attributeName}:\n{baseValue}");
        }
    }
}
