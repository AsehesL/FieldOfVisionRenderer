using UnityEngine;


public static class GizmosHelper
{

    public static void DrawWireSector(Vector3 position, float radius, float angle, float field, Color color)
    {
        float currentAngle = Mathf.Clamp(field, 0, 360);
        int cell = Mathf.CeilToInt(currentAngle / 18);

        Vector3 cp1 = new Vector3(position.x, position.y, position.z);

        float ag = 360 - angle;
        float fromAngle = ag - currentAngle / 2;

        if (cell <= 0)
            return;

        Gizmos.color = color;

        float dtAngle = currentAngle / cell;

        for (int i = 0; i < cell; i++)
        {
            float fag = fromAngle + i * dtAngle;
            float tag = fromAngle + (i + 1) * dtAngle;

            float fx = Mathf.Cos(fag * Mathf.Deg2Rad) * radius;
            float fy = Mathf.Sin(fag * Mathf.Deg2Rad) * radius;
            float tx = Mathf.Cos(tag * Mathf.Deg2Rad) * radius;
            float ty = Mathf.Sin(tag * Mathf.Deg2Rad) * radius;

            Vector3 p1 = new Vector3(position.x + fx, position.y,
                position.z + fy);
            Vector3 p3 = new Vector3(position.x + tx, position.y,
                position.z + ty);
            Gizmos.DrawLine(p1, p3);

            Gizmos.DrawLine(cp1, p1);

            if (i == cell - 1)
            {
                Gizmos.DrawLine(cp1, p3);
            }
        }
    }
}
