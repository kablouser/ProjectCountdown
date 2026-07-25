using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;

    private void Awake()
    {
        if (gameOverText.text.Length == 0)
        {
            SetGameOverReason(TimeAdjustmentReason.COUNTDOWN, true);
        }
    }

    public void SetGameOverReason(TimeAdjustmentReason reason, bool isAwake = false)
    {
        if (!isAwake && gameObject.activeInHierarchy)
        {
            // already has a reason
            return;
        }

        if (reason == TimeAdjustmentReason.DAMAGE)
        {
            string purpertrator = Random.value < 0.5f ? "The Entropy People's Front" : "The People's Front of Entropy";
            gameOverText.SetText(
                $"Rick Dickman killed in Active Service by {purpertrator}.\n\nSuper State thanks you for your time in service.");
        }
        else
        {
            gameOverText.SetText(
                $"Rick Dickman's life was out of time.\n\nSuper State thanks you for your time in service.");
        }
    }
}
