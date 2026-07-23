using UnityEngine;

public class IsStandingTracker : MonoBehaviour
{
    // correct for Update not FixedUpdate
    public bool isStanding;
    // usable from FixedUpdate, previous frames result
    public bool previousIsStanding;

    public float maxInclineAngle = 45;

    private void FixedUpdate()
    {
        previousIsStanding = isStanding;
        isStanding = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isStanding) return;

        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint contactPoint = collision.GetContact(i);
            if (Vector3.Angle(contactPoint.normal, Vector3.up) <= maxInclineAngle)
            {
                isStanding = true;
                break;
            }
        }
    }
}
