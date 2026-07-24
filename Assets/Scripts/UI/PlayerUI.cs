using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public enum UI_Screen { MainMenu, Playing, LevelCleared, GameOver, Shop, Pause };

    [SerializeField] UI_Screen currentUI_Screen;
    [SerializeField] BarUI timer;
    [SerializeField] WeaponUI weaponUI;

    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject levelClearedScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject shopScreen;
    [SerializeField] GameObject pauseScreen;

    public BarUI GetTimer
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

        SetUI_Screen(currentUI_Screen, true);
        instance = this;
    }

    public void SetUI_Screen(UI_Screen uiScreen, bool force = false)
    {
        if (currentUI_Screen == uiScreen && !force) return;

        timer.gameObject.SetActive(false);
        weaponUI.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(false);
        levelClearedScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
        shopScreen.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(false);

        currentUI_Screen = uiScreen;
        switch (currentUI_Screen)
        {
            case UI_Screen.MainMenu:
                mainMenuScreen.SetActive(true);
                break;
            case UI_Screen.Playing:
                timer.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                break;
            case UI_Screen.LevelCleared:
                timer.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                levelClearedScreen.SetActive(true);
                break;
            case UI_Screen.GameOver:
                timer.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                gameOverScreen.SetActive(true);
                break;
            case UI_Screen.Shop:
                shopScreen.SetActive(true);
                break;
            case UI_Screen.Pause:
                timer.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                pauseScreen.SetActive(true);
                break;
            default:
                Debug.LogWarning("Screen not implemented "+currentUI_Screen);
                break;
        }
    }
}
