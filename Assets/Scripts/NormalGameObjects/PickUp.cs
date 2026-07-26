using UnityEngine;

public class PickUp : MonoBehaviour
{
    [Header("Resource Gain")]
    public float extraTime = 1f;
    [Header("Visuals")]
    public Transform visuals;
    public float lifeTime = 5f;
    public float fadeOutAnimationTime = 1f;
    [SerializeField] private Material rareDropMaterial;

    public void SetRare()
    {
        visuals.gameObject.GetComponent<MeshRenderer>().material = rareDropMaterial;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (0 < lifeTime)
        {
            visuals.Rotate(0, Time.deltaTime * 100f, 0, Space.World);
            if (lifeTime < fadeOutAnimationTime)
            {
                float animationTime = (fadeOutAnimationTime - lifeTime) / fadeOutAnimationTime;
                visuals.localScale = Vector3.Lerp(visuals.localScale, Vector3.zero, animationTime);
            }
        }
        else
        {
            Destroy(gameObject);
        }
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
