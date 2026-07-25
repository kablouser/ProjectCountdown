using TMPro;
using UnityEngine;

public class LevelClearedUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetLevelClearedCountdown(float countdown)
    {
        text.SetText($"Level Cleared\n\nMoving to Shop in {Mathf.CeilToInt(countdown)}s");
    }
}
