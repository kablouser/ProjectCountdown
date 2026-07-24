using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform visuals;
    public float extraTime = 1f;

    private void Update()
    {
        visuals.Rotate(0, Time.deltaTime * 100f, 0, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            if (other.attachedRigidbody.GetComponent<PlayerCharacterProxy>())
            {
                Main main = Main.Singleton;
                if (main.isPlayerAlive)
                {
                    DamageCharacter.Damage(ID.Player, -extraTime, TimeAdjustmentReason.HEAL);
                    Destroy(gameObject);
                }
            }
        }
    }
}
