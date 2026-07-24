using UnityEngine;

public static class CountdownSystem
{
    public static void Update(Main main)
    {
        ref PlayerCharacter player = ref main.playerCharacter;

        if (main.levelState != LevelState.Countdown) return;

        if (player.character.currentHealth <= 0)
        {
            EndLevel(main);
            return;
        }

        bool isLevelCleared = true;
        foreach (ref EnemyCharacter enemy in main.enemyCharacters)
        {
            if (0 < enemy.character.currentHealth)
            {
                isLevelCleared = false;
            }
        }
        if (isLevelCleared)
        {
            EndLevel(main);
            return;
        }

        main.accumulatedCountdown += Time.deltaTime;
        // tick down in int seconds
        float accumulatedInts = Mathf.Floor(main.accumulatedCountdown);
        if (0 < accumulatedInts)
        {
            main.accumulatedCountdown -= accumulatedInts;
            if (DamageCharacter.Damage(player.character.id, accumulatedInts, TimeAdjustmentReason.COUNTDOWN))
            {
                EndLevel(main);
                return;
            }
        }
    }

    public static void StartLevel(Main main)
    {
        main.levelState = LevelState.Countdown;
        main.accumulatedCountdown = 0f;
        PlayerUI.Instance.SetUI_Screen(PlayerUI.UI_Screen.Playing);
    }

    public static void EndLevel(Main main)
    {
        main.levelState = LevelState.Menu;
        main.accumulatedCountdown = 0f;

        if (main.isPlayerAlive)
        {
            PlayerUI.Instance.SetUI_Screen(PlayerUI.UI_Screen.LevelCleared);
        }
        else
        {
            PlayerUI.Instance.SetUI_Screen(PlayerUI.UI_Screen.GameOver);
        }
    }
}
