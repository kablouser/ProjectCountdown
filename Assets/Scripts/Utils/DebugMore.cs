using UnityEngine;

public static class DebugMore
{
    public static void DrawArrow(in Vector3 from, in Vector3 to, Color? color, float duration = 0f, bool depthTest = true)
    {
        Color unpackColor = color ?? Color.white;
        Debug.DrawLine(from, to, unpackColor, duration, depthTest);
        Vector3 direction = to - from;
        Vector3 perpendicular = Vector3.Cross(direction.normalized, Vector3.forward);
        if (perpendicular.sqrMagnitude < 0.001f)
        {
            perpendicular = Vector3.right;
        }

        Vector3 right = (Quaternion.AngleAxis(30f, perpendicular) * -direction) * 0.1f;
        Vector3 left = (Quaternion.AngleAxis(-30f, perpendicular) * -direction) * 0.1f;
        Debug.DrawLine(to, right + to, unpackColor, duration, depthTest);
        Debug.DrawLine(to, left + to, unpackColor, duration, depthTest);
    }
}
