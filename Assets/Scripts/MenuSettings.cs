using UnityEngine;

[CreateAssetMenu(fileName = "MenuSettings", menuName = "ScriptableObjects/MenuSettings", order = 1)]
public class MenuSettings : ScriptableObject
{
    public float music = 0.7f;
    public float sfx = 0.7f;
    public float mouseSensitivity = 0.5f;
    public bool invertLookY = false;
}
