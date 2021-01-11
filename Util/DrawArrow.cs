using UnityEditor;
using UnityEngine;

public static class DrawArrow
{
    public static void ForHandle(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Arrow(TargetType.Handle, pos, direction, Handles.color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForHandle(in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Arrow(TargetType.Handle, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForGizmo(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Arrow(TargetType.Gizmo, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForGizmo(in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Arrow(TargetType.Gizmo, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForDebug(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction);
        Arrow(TargetType.Debug, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForDebug(in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction, color);
        Arrow(TargetType.Debug, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    private static void Arrow(TargetType targetType, in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        var right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back * arrowHeadLength;
        var left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back * arrowHeadLength;
        var up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back * arrowHeadLength;
        var down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back * arrowHeadLength;
        var end = pos + direction;
        Color colorPrew;

        switch (targetType)
        {
            case TargetType.Gizmo:
                colorPrew = Gizmos.color;
                Gizmos.color = color;
                Gizmos.DrawRay(pos, direction);
                Gizmos.DrawRay(end, right);
                Gizmos.DrawRay(end, left);
                Gizmos.DrawRay(end, up);
                Gizmos.DrawRay(end, down);
                Gizmos.color = colorPrew;
                break;

            case TargetType.Debug:
                Debug.DrawRay(end, right, color);
                Debug.DrawRay(end, left, color);
                Debug.DrawRay(end, up, color);
                Debug.DrawRay(end, down, color);
                break;

            case TargetType.Handle:
                colorPrew = Handles.color;
                Handles.color = color;
                Handles.DrawLine(pos, end);
                Handles.DrawLine(end, end + right);
                Handles.DrawLine(end, end + left);
                Handles.DrawLine(end, end + up);
                Handles.DrawLine(end, end + down);
                Handles.color = colorPrew;
                break;
        }
    }

    private enum TargetType
    {
        Gizmo, Debug, Handle
    }
}
