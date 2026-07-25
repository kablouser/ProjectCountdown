using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider music;
    public Slider sfx;
    public Slider mouseSensitivity;
    public Toggle invertLookY;

    private void Awake()
    {
        music.onValueChanged.AddListener((x) =>
        {
            Main.Singleton.settings.music = x;
            Main.Singleton.ApplySettings();
        });

        sfx.onValueChanged.AddListener((x) =>
        {
            Main.Singleton.settings.sfx = x;
            Main.Singleton.ApplySettings();
        });

        mouseSensitivity.onValueChanged.AddListener((x) =>
        {
            Main.Singleton.settings.mouseSensitivity = x;
            Main.Singleton.ApplySettings();
        });

        invertLookY.onValueChanged.AddListener((x) =>
        {
            Main.Singleton.settings.invertLookY = x;
            Main.Singleton.ApplySettings();
        });
    }

    private void OnEnable()
    {
        Settings settings = Main.Singleton.settings;
        music.value = settings.music;
        sfx.value = settings.sfx;
        mouseSensitivity.value = settings.mouseSensitivity;
        invertLookY.isOn = settings.invertLookY;
    }
}
