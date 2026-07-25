using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public BarUI healthBar;
    public WeaponUI weaponUI;
    public GameObject mainMenuScreen;
    public LevelClearedUI levelClearedScreen;
    public GameObject gameOverScreen;
    public ShopScreenUI shopScreen;
    public GameObject pauseScreen;

    public GameObject crossHair;
    public Image hitMarker;
    private const float HitMarkerTime = 0.3f;
    private float _hitMarkerTimer;
    

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
        _hitMarkerTimer = 0;
    }

    private void Update()
    {
        _hitMarkerTimer -= Time.deltaTime;
        _hitMarkerTimer = Math.Max(_hitMarkerTimer, 0.0f);
        
        Color currentColor = hitMarker.color;
        currentColor.a = _hitMarkerTimer / HitMarkerTime;
        hitMarker.color = currentColor;
    }

    public void ShowHitMarker()
    {
        _hitMarkerTimer = HitMarkerTime;
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
        crossHair.SetActive(false);

        switch (levelState)
        {
            case LevelState.MainMenu:
                mainMenuScreen.SetActive(true);
                break;
            case LevelState.Playing:
                healthBar.gameObject.SetActive(true);
                weaponUI.gameObject.SetActive(true);
                crossHair.SetActive(!isPaused);
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
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
            Cursor.lockState = CursorLockMode.None;
        else if (levelState == LevelState.Playing || levelState == LevelState.LevelCleared)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
