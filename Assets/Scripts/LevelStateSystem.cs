using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelStateSystem
{
    public static void Start(Main main)
    {
        SetLevelState(main, main.levelState, true);
    }

    public static void Update(Main main)
    {
        ref PlayerCharacter player = ref main.playerCharacter;

        if (main.isLevelStateQueued)
        {
            main.isLevelStateQueued = false;
            SetLevelState(main, main.queueChangeLevelState);
        }

        bool canPause = false;
        switch (main.levelState)
        {
            case LevelState.MainMenu:
                break;
            case LevelState.Playing:
                InternalUpdatePlayingLevel(main, ref player);
                // InternalUpdatePlayingLevel may change levelState
                canPause = main.levelState == LevelState.Playing;
                break;
            case LevelState.LevelCleared:
                {
                    main.levelClearedCountdown -= Time.deltaTime;
                    if (main.levelClearedCountdown <= 0f)
                    {
                        main.levelClearedCountdown = 0f;
                        SceneManager.LoadScene(main.shopSceneIndex);
                    }
                    else
                    {
                        PlayerUI.Instance.levelClearedScreen.SetLevelClearedCountdown(main.levelClearedCountdown);
                    }
                }
                break;
            case LevelState.GameOver:
                break;
            case LevelState.Shop:
                break;
            default:
                Debug.LogWarning("levelState not implemented " + main.levelState);
                break;
        }

        if (canPause)
        {
            if (main.playerCharacter.pauseInput)
            {
                main.playerCharacter.pauseInput = false;
                PlayerUI.Instance.SetUI_Screen(main.levelState, Time.timeScale != 0f);
            }
        }

        if (!main.musicSource.isPlaying)
        {
            main.musicSource.clip = main.levelState == LevelState.MainMenu ? main.menuLoop : main.musicLoop;
            main.musicSource.loop = true;
            main.musicSource.Play();
        }
    }

    public static void SetLevelState(Main main, LevelState state, bool isStart = false)
    {
        if (!isStart &&
            main.levelState == state)
        {
            return;
        }

        // old state
        if (!isStart)
        {
            switch (main.levelState)
            {
                default: break;
            }
        }

        // new state
        main.levelState = state;
        switch (main.levelState)
        {
            default: break;
            case LevelState.MainMenu:
                main.musicSource.clip = main.menuIntro;
                main.musicSource.loop = false;
                main.musicSource.Play();
                break;

            case LevelState.Playing:
                main.musicSource.clip = main.musicIntro;
                main.musicSource.loop = false;
                main.musicSource.Play();

                // Spawn enemies.
                main.levelStats.enemiesRemaining = main.levelStats.GetEnemyCount(main.level);
                for (int i = 0; i < main.levelStats.enemiesRemaining; i++)
                {
                    if (main.enemyPool == null)
                    {
                        Debug.LogError("Playing level must contain enemy pool");
                        break;
                    }
                    
                    GameObject enemy;
                    // We've reached the max enemies we allow.
                    if (!main.enemyPool.GetNext(out enemy))
                    {
                        break;
                    }

                    int spawnPointIndex = i % main.enemyPool.enemySpawnPoints.Count;
                    Transform spawnTransform = main.enemyPool.enemySpawnPoints[spawnPointIndex];
                    enemy.transform.position = spawnTransform.position;
                    enemy.transform.rotation = spawnTransform.rotation;
                }
                main.accumulatedCountdown = 0f;
                break;

            case LevelState.LevelCleared:
                main.playerTimeLeft = main.playerCharacter.character.currentHealth;
                main.levelClearedCountdown = main.levelClearedDuration;
                PlayerUI.Instance.levelClearedScreen.SetLevelClearedCountdown(main.levelClearedCountdown);
                break;
        }

        PlayerUI.Instance.SetUI_Screen(state, false);
    }

    private static void InternalUpdatePlayingLevel(Main main, ref PlayerCharacter player)
    {
        if (player.character.currentHealth <= 0)
        {
            SetLevelState(main, LevelState.GameOver);
            return;
        }

        if (main.enemyPool == null)
        {
            Debug.LogError("Playing level must contain enemy pool");
            return;
        }

        var (activeEnemies, freeEnemies) = main.enemyPool.GetActiveAndFreeCount();
        // If we have more enemies we need to spawn and there are free enemies available then spawn them.
        if (activeEnemies < main.levelStats.enemiesRemaining && freeEnemies > 0)
        {
            GameObject enemy;
            if (main.enemyPool.GetNext(out enemy))
            {
                int randomIndex = Random.Range(0, main.enemyPool.enemySpawnPoints.Count);     
                Transform spawnTransform = main.enemyPool.enemySpawnPoints[randomIndex];
                enemy.transform.position = spawnTransform.position;
                enemy.transform.rotation = spawnTransform.rotation;
            }
        }
        

        bool isLevelCleared = main.levelStats.enemiesRemaining == 0;
        if (isLevelCleared)
        {
            SetLevelState(main, LevelState.LevelCleared);
            return;
        }

        main.accumulatedCountdown += Time.deltaTime;
        // tick down in int seconds
        float accumulatedInts = Mathf.Floor(main.accumulatedCountdown);
        if (0 < accumulatedInts)
        {
            main.accumulatedCountdown -= accumulatedInts;
            // Players health is the time remaining.
            if (DamageCharacter.Damage(player.character.id, accumulatedInts, TimeAdjustmentReason.COUNTDOWN))
            {
                SetLevelState(main, LevelState.GameOver);
                return;
            }
        }       
    }
}
