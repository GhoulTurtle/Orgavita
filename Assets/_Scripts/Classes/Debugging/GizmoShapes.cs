using UnityEngine;

public static class GizmoShapes{
    public static void DrawCone(Vector3 position, Vector3 direction, float angle, float length, int circleSideCount = 36){
        // Normalize the direction
        direction.Normalize();

        // Calculate the right and up vectors
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0) * Vector3.forward;
        Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0) * Vector3.forward;

        // Draw the circular base of the cone
        DrawCircle(position, direction, right, angle, length, circleSideCount);

        // Draw the cone sides
        Gizmos.DrawLine(position, position + Quaternion.AngleAxis(-angle, right) * direction * length);
        Gizmos.DrawLine(position, position + Quaternion.AngleAxis(-angle, up) * direction * length);
        Gizmos.DrawLine(position, position + Quaternion.AngleAxis(angle, right) * direction * length);
        Gizmos.DrawLine(position, position + Quaternion.AngleAxis(angle, up) * direction * length);
    }

    public static void DrawCircle(Vector3 position, Vector3 direction, Vector3 right, float angle, float length, int circleSideCount = 36){
        float angleStep = 360.0f / circleSideCount;
        Vector3 previousPoint = position + Quaternion.AngleAxis(-angle, right) * direction * length;
        
        for (int i = 1; i <= circleSideCount; i++){
            float currentAngle = i * angleStep;
            Vector3 nextPoint = position + Quaternion.AngleAxis(currentAngle, direction) * (Quaternion.AngleAxis(-angle, right) * direction * length);
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}