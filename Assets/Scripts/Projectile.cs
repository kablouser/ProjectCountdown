using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Must Set")]
    public float lifeTime = 60f;
    public float radiusApproximate = 0.5f;
    public float speed = 1f;
    [Range(0,360)]
    public float homingScale;
    [Header("Will be set when Instantiated")]
    // can be null
    public Transform homingTarget;
    public float damage;
    public ID source;

    private Rigidbody rb;
    private float awakeTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        awakeTime = Time.time;
        Destroy(gameObject, lifeTime);

        rb.linearVelocity = transform.forward * speed;
    }

    public void FixedUpdate()
    {
        if (Time.timeScale == 0) return;

        Vector3 velocity = transform.forward * speed;
        rb.collisionDetectionMode = 100f < speed ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
        rb.linearVelocity = velocity;

        if (homingTarget != null)
        {
            Vector3 homingDirection = homingTarget.position - transform.position;
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(homingDirection, Vector3.up), homingScale * Time.deltaTime);
        }
        else if (0f < velocity.sqrMagnitude)
        {
            rb.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            IProxy iproxy = collision.rigidbody.GetComponent<IProxy>();
            if (iproxy != null)
            {
                if (iproxy.GetID().IsEqual(source) && awakeTime < Time.time + 0.1f)
                {
                    return;
                }

                DamageCharacter.Damage(iproxy.GetID(), damage, TimeAdjustmentReason.DAMAGE, source.type);

                if (source.type == IDType.Player)
                {
                    Main.Singleton.playerCharacter.character.audioSource.PlayOneShot(Main.Singleton.hitMarkerSFX);
                }
            }
        }
        Destroy(gameObject);
    }
}
