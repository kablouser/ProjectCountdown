using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

[System.Flags]
public enum WeaponUpgradeFlags
{
    None = 0,
    Damage = 1,
    AmmoCapacity = 1 << 2,
    ReloadSpeed = 1 << 3,
    RoundsPerMinute = 1 << 4,
}

public class ShopScreenUI : MonoBehaviour
{
    [System.Serializable]
    public struct ShopWeapon
    {
        public bool isUnlockedInShop;
        // Added to allow for guns to be stored in a separate file to make editing easier
        public GunDataObject gunData;
        // upgrade stats
        public float addDamage;
        public int addAmmoCapacity;
        public float multiplyReloadSpeed;
        public float multiplyRoundsPerMinute;

        [HideInInspector] public ShopWeaponUI ui;
    }

    public CanvasGroup hintCanvasGroup;
    public TextMeshProUGUI hintText;
    public Transform shopWeaponUIParent;
    public TextMeshProUGUI playerTimeLeftText;
    public ShopWeapon[] shopWeapons;

    public ShopWeaponUI weaponPrefab;

    private void Awake()
    {
        int i = 0;
        foreach (ref ShopWeapon weapon in shopWeapons.AsSpan())
        {
            weapon.multiplyReloadSpeed = weapon.multiplyRoundsPerMinute = 1f;
            weapon.isUnlockedInShop = weapon.gunData.stats.isUnlocked;

            weapon.ui = Instantiate(weaponPrefab, shopWeaponUIParent);
            weapon.ui.SetName(weapon);

            int weaponI = i;
            weapon.ui.unlock.GetComponent<Image>().sprite = weapon.gunData.stats.weaponImage;
            weapon.ui.unlock.onClick.AddListener(() => TryUnlockWeapon(weaponI));
            weapon.ui.damageUpgrade.onClick.AddListener(() => TryUnlockWeaponUpgrade(weaponI, WeaponUpgradeFlags.Damage));
            weapon.ui.ammoUpgrade.onClick.AddListener(() => TryUnlockWeaponUpgrade(weaponI, WeaponUpgradeFlags.AmmoCapacity));
            weapon.ui.reloadUpgrade.onClick.AddListener(() => TryUnlockWeaponUpgrade(weaponI, WeaponUpgradeFlags.ReloadSpeed));
            weapon.ui.roundsPerMinuteUpgrade.onClick.AddListener(() => TryUnlockWeaponUpgrade(weaponI, WeaponUpgradeFlags.RoundsPerMinute));
            TryUnlockWeapon(i, true);

            i++;
        }
    }

    private void OnEnable()
    {
        hintCanvasGroup.alpha = 0f;
        UpdatePlayerTimeLeftText();
    }

    private void Update()
    {
        hintCanvasGroup.alpha -= Time.deltaTime * 0.5f;
    }

    public void TryUnlockWeapon(int weaponI, bool isSetup = false)
    {
        if (!isSetup)
        {
            if (shopWeapons[weaponI].isUnlockedInShop)
            {
                ShowHint("Already unlocked");
                return;
            }

            ref float playerTimeLeft = ref Main.Singleton.playerTimeLeft;
            if (playerTimeLeft < shopWeapons[weaponI].gunData.unlockCost)
            {
                ShowHint($"Insufficient time! Need additional {shopWeapons[weaponI].gunData.unlockCost - playerTimeLeft:0.###}s");
                return;
            }
            playerTimeLeft -= shopWeapons[weaponI].gunData.unlockCost;
            UpdatePlayerTimeLeftText();
            shopWeapons[weaponI].isUnlockedInShop = true;
        }

        shopWeapons[weaponI].ui.SetUnlocked(shopWeapons[weaponI]);
    }

    public void TryUnlockWeaponUpgrade(int weaponI, WeaponUpgradeFlags upgradeType)
    {
        if (!shopWeapons[weaponI].isUnlockedInShop)
        {
            ShowHint("Weapon not unlocked");
            return;
        }

        if ((upgradeType & shopWeapons[weaponI].gunData.upgradables) == WeaponUpgradeFlags.None)
        {
            ShowHint("Cannot upgrade this attribute!");
            return;
        }

        ref float playerTimeLeft = ref Main.Singleton.playerTimeLeft;
        if (playerTimeLeft < shopWeapons[weaponI].gunData.upgradeCost)
        {
            ShowHint($"Insufficient time! Need additional {shopWeapons[weaponI].gunData.upgradeCost - playerTimeLeft:0.###}s");
            return;
        }
        playerTimeLeft -= shopWeapons[weaponI].gunData.upgradeCost;
        UpdatePlayerTimeLeftText();

        switch (upgradeType)
        {
            default: break;
            case WeaponUpgradeFlags.Damage:
                shopWeapons[weaponI].addDamage += 1f;
                break;
            case WeaponUpgradeFlags.AmmoCapacity:
                shopWeapons[weaponI].addAmmoCapacity += 1;
                break;
            case WeaponUpgradeFlags.ReloadSpeed:
                shopWeapons[weaponI].multiplyReloadSpeed += 0.2f;
                break;
            case WeaponUpgradeFlags.RoundsPerMinute:
                shopWeapons[weaponI].multiplyRoundsPerMinute += 0.2f;
                break;
        }
        shopWeapons[weaponI].ui.UpdateUpgradeTexts(shopWeapons[weaponI]);
    }

    public GunStat[] GetGunStats()
    {
        GunStat[] gunStats = new GunStat[shopWeapons.Length];
        int i = 0;
        foreach (ShopWeapon weapon in shopWeapons)
        {
            gunStats[i] = weapon.gunData.stats;
            gunStats[i].damage += weapon.addDamage;
            gunStats[i].ammoCapacity += weapon.addAmmoCapacity;
            gunStats[i].reloadTime /= Mathf.Max(Mathf.Epsilon, weapon.multiplyReloadSpeed);
            gunStats[i].roundsPerMin *= weapon.multiplyRoundsPerMinute;
            gunStats[i].isUnlocked = weapon.isUnlockedInShop;
            i++;
        }
        return gunStats;
    }

    public void ShowHint(string text)
    {
        if (Mouse.current != null)
        {
            hintCanvasGroup.transform.position = Mouse.current.position.ReadValue();
            hintCanvasGroup.alpha = 1f;
            hintText.SetText(text);
        }
    }

    public void UpdatePlayerTimeLeftText()
    {
        playerTimeLeftText.SetText($"Time Left: {Main.FormatTime(Main.Singleton.playerTimeLeft)}");
    }
}
