using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;

    private void Awake()
    {
        string purpertrator = Random.value < 0.5f ? "The Entropy People's Front" : "The People's Front of Entropy";
        gameOverText.SetText(
            $"Rick Dickman killed in Active Service by {purpertrator}.\n\nSuper State thanks you for your time in service.");
    }
}
