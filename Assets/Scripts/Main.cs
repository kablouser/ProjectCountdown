using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

struct DrawArrowLifetime
{
    public Vector3 from;
    public Vector3 to;
    public float lifeEnd;
}

// menu doesn't include pause menu
public enum LevelState { MainMenu, Playing, LevelCleared, GameOver, Shop };

[Serializable]
public struct Settings
{
    public float music;
    public float sfx;
    public float mouseSensitivity;
    public bool invertLookY;
}

public class Main : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public static Main Singleton { get; private set; }

    [Header("Settings")]
    public LevelState levelState;
    public Settings settings = new Settings { music = 0.7f, sfx = 0.7f, mouseSensitivity = 0.5f, invertLookY = false };
    public int mainMenuSceneIndex = 0;
    public int playingSceneIndex = 1;
    public int shopSceneIndex = 2;
    public float levelClearedDuration = 5f;

    public LayerMask shootLayerMask;
    public PickUp pickUpExtraTimePrefab;

    [Header("Audio")]
    public AudioMixer masterMixer;
    public AudioSource musicSource;
    public AudioClip musicIntro;
    public AudioClip musicLoop;

    [Header("For Viewing Purposes")]
    public bool isPlayerAlive;
    public float playerTimeLeft;
    public PlayerCharacter playerCharacter = PlayerCharacter.Default;
    // saved when level is cleared. used as currency in shop
    public VersionedList<EnemyCharacter> enemyCharacters;

    // decimal part of the countdown. we only countdown in ints
    public float accumulatedCountdown;
    public float levelClearedCountdown;

    InputSystem_Actions inputSystem_Actions;
    RaycastHit[] raycastHitCache;

    List<DrawArrowLifetime> drawArrows;

    public EnemyPool enemyPool;
    public LevelStats levelStats;

    void Awake()
    {
        // keep first Singleton across loading levels
        if (Singleton != null)
        {
            // use this levelState
            LevelStateSystem.SetLevelState(Singleton, levelState);
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Singleton = this;

        if (inputSystem_Actions == null)
        {
            inputSystem_Actions = new InputSystem_Actions();
            inputSystem_Actions.Player.SetCallbacks(this);
        }
        if (raycastHitCache == null)
        {
            raycastHitCache = new RaycastHit[32];
        }
        drawArrows = new List<DrawArrowLifetime>();
        enemyCharacters.type = IDType.Enemy;
    }

    void Start()
    {
        ApplySettings();

        musicSource.clip = musicIntro;
        musicSource.loop = false;
        musicSource.Play();

        LevelStateSystem.Start(this);
    }

    void OnEnable()
    {
        inputSystem_Actions.Player.Enable();
    }

    private void OnDisable()
    {
        if (inputSystem_Actions != null)
        {
            inputSystem_Actions.Player.Disable();
        }
    }

    void Update()
    {
        if (isPlayerAlive)
        {
            WeaponSelectSystem.Update(ref playerCharacter.character);
            PlayerUI.Instance.weaponUI.SetAmmo(playerCharacter.character.ActiveGun);
            ShootSystem.Update(ref playerCharacter.character, shootLayerMask, raycastHitCache);
            GunAnimationSystem.Update(ref playerCharacter.character);
            WalkingSystem.Update(ref playerCharacter.character);
        }
        // update countdown before enemies. so in case of simulatenous shoot out, player won't lose
        LevelStateSystem.Update(this);

        foreach (ref EnemyCharacter enemy in enemyCharacters)
        {
            EnemyAI_System.Update(ref enemy);
            ShootSystem.Update(ref enemy.character, shootLayerMask, raycastHitCache);
            GunAnimationSystem.Update(ref enemy.character);
            WalkingSystem.Update(ref enemy.character);
        }

        if (!musicSource.isPlaying)
        {
            musicSource.clip = musicLoop;
            musicSource.loop = true;
            musicSource.Play();

            musicIntro.UnloadAudioData();
        }
    }

    void FixedUpdate()
    {
        if (isPlayerAlive)
        {
            WalkingSystem.FixedUpdate(ref playerCharacter.character);
        }

        foreach (ref EnemyCharacter enemy in enemyCharacters)
        {
            WalkingSystem.FixedUpdate(ref enemy.character);
        }
        // place this in FixedUpdate because Update() will clear it before FixedUpdate() can read it
        ResetOneTimeInputs();
    }
    private void OnDrawGizmos()
    {
        if (drawArrows == null) return;
        for (int i = 0; i < drawArrows.Count; i++)
        {
            if (Time.time < drawArrows[i].lifeEnd)
            {
                drawArrows.RemoveAt(i);
                i--;
                continue;
            }
            GizmosMore.DrawArrow(drawArrows[i].from, drawArrows[i].to);
        }
    }

    void InputSystem_Actions.IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        playerCharacter.character.moveInput = context.ReadValue<Vector2>();
    }

    void InputSystem_Actions.IPlayerActions.OnLook(InputAction.CallbackContext context)
    {
        playerCharacter.character.lookInput = context.ReadValue<Vector2>();
    }

    void InputSystem_Actions.IPlayerActions.OnShoot(InputAction.CallbackContext context)
    {
        playerCharacter.character.shootInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnReload(InputAction.CallbackContext context)
    {
        playerCharacter.character.reloadInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnJump(InputAction.CallbackContext context)
    {
        playerCharacter.character.jumpInput = context.action.GetButtonDown();
    }

    void InputSystem_Actions.IPlayerActions.OnFirstWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            playerCharacter.character.weaponSelectInput = 0;
        }
    }

    void InputSystem_Actions.IPlayerActions.OnSecondWeapon(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            playerCharacter.character.weaponSelectInput = 1;
        }
    }

    void InputSystem_Actions.IPlayerActions.OnPause(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            playerCharacter.pauseInput = true;
        }
    }

    public void AwakePlayer(PlayerCharacter player)
    {
        isPlayerAlive = true;
        playerCharacter = player;
        playerCharacter.Awake();
        ApplySettings();
    }

    public ID AwakeEnemy(EnemyCharacter enemyCharacter)
    {
        enemyCharacter.Awake();
        return enemyCharacters.Add(enemyCharacter);
    }

    public void SleepEnemy(ID id)
    {
        enemyCharacters.Remove(id);
    }

    public void DestroyPlayer()
    {
        isPlayerAlive = false;
    }

    public bool DestroyEnemy(ID id)
    {
        return enemyCharacters.Remove(id);
    }

    public void DrawArrowGizmo(Vector3 from, Vector3 to, float duration)
    {
        drawArrows.Add(new DrawArrowLifetime { from = from, to = to, lifeEnd = Time.time + duration });
    }

    private void ResetOneTimeInputs()
    {
        playerCharacter.character.shootInput = false;
        playerCharacter.character.reloadInput = false;
        playerCharacter.character.jumpInput = false;
        playerCharacter.character.weaponSelectInput = -1;
        playerCharacter.pauseInput = false;
    }

    public static string FormatTime(float time)
    {
        float seconds = time;
        float hours = Mathf.Floor(seconds / 60f / 60f);
        seconds -= hours * 60f * 60f;
        float minutes = Mathf.Floor(seconds / 60f);
        seconds -= minutes * 60f;
        if (0 < hours)
        {
            return $"{hours}h:{minutes}m:{seconds:0.##}s";
        }
        else if (0 < minutes)
        {
            return $"{minutes}m:{seconds:0.##}s";
        }
        else
        {
            return $"{seconds:0.##}s";
        }
    }

    public void ApplySettings()
    {
        masterMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, -10, settings.music));
        masterMixer.SetFloat("SFXVolume", Mathf.Lerp(-80, 0, settings.sfx));
        playerCharacter.character.turnSpeed = Mathf.Lerp(0.1f, 100, settings.mouseSensitivity * settings.mouseSensitivity);
        playerCharacter.character.invertLookY = settings.invertLookY;
    }

    public void OnPlayPressed()
    {
        SceneManager.LoadScene(playingSceneIndex);
    }

    public void OnResumePlayPressed()
    {
        PlayerUI.Instance.SetUI_Screen(levelState, false);
    }

    public void OnRestartPressed()
    {
        // Reset main by destroying it
        Destroy(gameObject);
        // this will load a new main
        SceneManager.LoadScene(playingSceneIndex);
    }
}
