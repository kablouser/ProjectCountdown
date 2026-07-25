using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "GunData", order = 0)]
public class GunDataObject : ScriptableObject
{
    public float unlockCost;
    public GunStat stats;
    public WeaponUpgradeFlags upgradables;
    public float upgradeCost;
}