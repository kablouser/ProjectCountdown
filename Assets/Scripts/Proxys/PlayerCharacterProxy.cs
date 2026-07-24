using UnityEngine;

public class PlayerCharacterProxy : MonoBehaviour, IProxy
{
    public PlayerCharacter player = PlayerCharacter.Default;
    private void Awake()
    {
        Main.Singleton.AwakePlayer(player);
    }

    private void OnDestroy()
    {
        Main.Singleton.DestroyPlayer();
    }

    ID IProxy.GetID()
    {
        return new ID { type = IDType.Player };
    }
}
