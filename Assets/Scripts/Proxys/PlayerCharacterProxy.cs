using UnityEngine;

public class PlayerCharacterProxy : MonoBehaviour
{
    public PlayerCharacter player = PlayerCharacter.Default;
    private void Awake()
    {
        Main.Singleton.RegisterPlayer(player);
    }
}
