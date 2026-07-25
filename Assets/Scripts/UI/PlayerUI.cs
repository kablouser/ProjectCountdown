using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public BarUI healthBar;
    public WeaponUI weaponUI;
    public GameObject mainMenuScreen;
    public LevelClearedUI levelClearedScreen;
    public GameObject gameOverScreen;
    public ShopScreenUI shopScreen;
    public GameObject pauseScreen;

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
        SetUI_Screen(LevelState.MainMenu, false);
    }

    public void SetUI_Screen(LevelState levelState, bool isPaused)
    {
        healthBar.gameObject.SetActive(false);
        weaponUI.gameObject.SetActive(false);
        mainMenuScreen.SetActive(false);
        levelClearedScreen.gameObject.SetActive(false);
        gameOverScreen.SetActive(false);
        shopScreen.gameObject.SetActive(false);
        pauseScreen.SetActive(false);

        switch (levelState)
        {
            case LevelState.MainMenu:
                mainMenuScreen.SetActive(true);
                break;
            case LevelState.Playing:
                healthBar.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                break;
            case LevelState.LevelCleared:
                healthBar.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                levelClearedScreen.gameObject.SetActive(true);
                break;
            case LevelState.GameOver:
                healthBar.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                gameOverScreen.SetActive(true);
                break;
            case LevelState.Shop:
                shopScreen.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Screen not implemented " + levelState);
                break;
        }
        pauseScreen.SetActive(isPaused);
    }
}
