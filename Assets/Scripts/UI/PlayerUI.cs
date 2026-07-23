using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TimerUI timer;
    [SerializeField] WeaponUI weaponUI;

    public TimerUI GetTimer
    {
        get { return timer; }
    }

    public WeaponUI GetWeaponUI
    {
        get { return weaponUI; }
    }

    static PlayerUI instance;
    public static PlayerUI Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }
}
