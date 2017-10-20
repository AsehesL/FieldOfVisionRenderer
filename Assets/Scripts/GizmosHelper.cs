using UnityEngine;


public static class GizmosHelper
{

    public static void DrawWireBox(Vector3 position, Quaternion rotation, Vector3 size, Color color)
    {
        Gizmos.color = color;
        Vector3 p1 = position + rotation * new Vector3(-size.x / 2, -size.y / 2, -size.z / 2);
        Vector3 p2 = position + rotation * new Vector3(-size.x / 2, -size.y / 2, size.z / 2);
        Vector3 p3 = position + rotation * new Vector3(size.x / 2, -size.y / 2, size.z / 2);
        Vector3 p4 = position + rotation * new Vector3(size.x / 2, -size.y / 2, -size.z / 2);
        Vector3 p5 = position + rotation * new Vector3(-size.x / 2, size.y / 2, -size.z / 2);
        Vector3 p6 = position + rotation * new Vector3(-size.x / 2, size.y / 2, size.z / 2);
        Vector3 p7 = position + rotation * new Vector3(size.x / 2, size.y / 2, size.z / 2);
        Vector3 p8 = position + rotation * new Vector3(size.x / 2, size.y / 2, -size.z / 2);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p6, p7);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p8, p5);

        Gizmos.DrawLine(p1, p5);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p3, p7);
        Gizmos.DrawLine(p4, p8);
    }

    public static void DrawWireCylinder(Vector3 position, float radius, float height, Color color)
    {
        Vector3 cp1 = new Vector3(position.x, position.y + height / 2, position.z);
        Vector3 cp2 = new Vector3(position.x, position.y - height / 2, position.z);
        Gizmos.color = color;
        for (int i = 0; i < 12; i++)
        {
            float ag = i * 30 * Mathf.Deg2Rad;
            float agn = (i + 1) * 30 * Mathf.Deg2Rad;


            float x = Mathf.Cos(ag) * radius;
            float y = Mathf.Sin(ag) * radius;

            float xn = Mathf.Cos(agn) * radius;
            float yn = Mathf.Sin(agn) * radius;

            Vector3 p1 = new Vector3(position.x + x, position.y + height / 2, position.z + y);
            Vector3 p2 = new Vector3(position.x + x, position.y - height / 2, position.z + y);

            Vector3 p3 = new Vector3(position.x + xn, position.y + height / 2, position.z + yn);
            Vector3 p4 = new Vector3(position.x + xn, position.y - height / 2, position.z + yn);

            Gizmos.DrawLine(cp1, p1);
            Gizmos.DrawLine(p1, p3);

            Gizmos.DrawLine(cp2, p2);
            Gizmos.DrawLine(p2, p4);

            Gizmos.DrawLine(p1, p2);
        }
    }

    public static void DrawWireSector(Vector3 position, float radius, float height, float angle, float field,
        Color color)
    {
        float currentAngle = Mathf.Clamp(field, 0, 360);
        int cell = Mathf.CeilToInt(currentAngle / 30.0f);

        Vector3 cp1 = new Vector3(position.x, position.y + height / 2, position.z);
        Vector3 cp2 = new Vector3(position.x, position.y - height / 2, position.z);

        float ag = 360 - angle;
        float fromAngle = ag - currentAngle / 2;

        if (cell <= 0)
            return;

        Gizmos.color = color;

        float dtAngle = currentAngle / cell;

        Gizmos.DrawLine(cp1, cp2);

        for (int i = 0; i < cell; i++)
        {
            float fag = fromAngle + i * dtAngle;
            float tag = fromAngle + (i + 1) * dtAngle;

            float fx = Mathf.Cos(fag * Mathf.Deg2Rad) * radius;
            float fy = Mathf.Sin(fag * Mathf.Deg2Rad) * radius;
            float tx = Mathf.Cos(tag * Mathf.Deg2Rad) * radius;
            float ty = Mathf.Sin(tag * Mathf.Deg2Rad) * radius;

            Vector3 p1 = new Vector3(position.x + fx, position.y + height / 2,
                position.z + fy);
            Vector3 p2 = new Vector3(position.x + fx, position.y - height / 2,
                position.z + fy);
            Vector3 p3 = new Vector3(position.x + tx, position.y + height / 2,
                position.z + ty);
            Vector3 p4 = new Vector3(position.x + tx, position.y - height / 2,
                position.z + ty);
            Gizmos.DrawLine(p1, p3);
            Gizmos.DrawLine(p2, p4);

            Gizmos.DrawLine(cp1, p1);
            Gizmos.DrawLine(cp2, p2);

            Gizmos.DrawLine(p1, p2);

            if (i == cell - 1)
            {
                Gizmos.DrawLine(cp1, p3);
                Gizmos.DrawLine(cp2, p4);

                Gizmos.DrawLine(p3, p4);
            }
        }
    }
}
