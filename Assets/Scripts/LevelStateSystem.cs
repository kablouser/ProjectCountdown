using System;
using UnityEngine;

public static class LevelStateSystem
{
    public static void Start(Main main)
    {
        SetLevelState(main, main.levelState, true);
    }

    public static void Update(Main main)
    {
        ref PlayerCharacter player = ref main.playerCharacter;

        switch (main.levelState)
        {
            case LevelState.MainMenu:
                break;
            case LevelState.Playing:
                {
                    InternalUpdatePlayingLevel(main, ref player);
                }
                break;
            case LevelState.LevelCleared:
                {
                    main.levelClearedCountdown -= Time.deltaTime;
                    if (main.levelClearedCountdown <= 0f)
                    {
                        main.levelClearedCountdown = 0f;
                        SetLevelState(main, LevelState.Shop);
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
            case LevelState.Playing:
                // Spawn enemies.
                main.levelStats.enemiesRemaining = main.levelStats.GetEnemyCount(0);
                for (int i = 0; i < main.levelStats.enemiesRemaining; i++)
                {
                    GameObject enemy = main.enemyPool.GetNext();
                    
                    // We've reached the max enemies we allow.
                    if (enemy is null)
                    {
                        break;
                    }

                    int spawnPointIndex = i % main.enemySpawnPoints.Count;
                    Transform spawnTransform = main.enemySpawnPoints[spawnPointIndex].transform;
                    enemy.transform.position = spawnTransform.position;
                    enemy.transform.rotation = spawnTransform.rotation;
                }
                
                // TODO: Set up health here too.
                main.accumulatedCountdown = 0f;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case LevelState.LevelCleared:
                main.playerTimeLeft = main.playerCharacter.character.currentHealth;
                main.levelClearedCountdown = main.levelClearedDuration;
                PlayerUI.Instance.levelClearedScreen.SetLevelClearedCountdown(main.levelClearedCountdown);
                break;
            case LevelState.Shop:
                Cursor.lockState = CursorLockMode.None;
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
