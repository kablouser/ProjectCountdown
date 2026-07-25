using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "GunData", menuName = "GunData", order = 0)]
    public class GunDataObject : ScriptableObject
    {
        [SerializeField] public GunStat stats;
    }
}