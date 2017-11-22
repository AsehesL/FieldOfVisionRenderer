using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfVisionRenderer))]
public class FieldOfVisionRendererEditor : Editor
{

    private FieldOfVisionRenderer m_Target;

    private SerializedProperty m_CullingMask;
    private SerializedProperty m_BlendMode;

    void OnEnable()
    {
        m_Target = (FieldOfVisionRenderer) target;
        m_CullingMask = serializedObject.FindProperty("m_CullingMask");
        m_BlendMode = serializedObject.FindProperty("m_BlendMode");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_Target.angle = Mathf.Clamp(EditorGUILayout.FloatField("Angle", m_Target.angle), 0.01f, 179.999f);
        m_Target.radius = Mathf.Max(0.01f, EditorGUILayout.FloatField("Radius", m_Target.radius));
        m_Target.color = EditorGUILayout.ColorField("Color", m_Target.color);
        EditorGUILayout.PropertyField(m_BlendMode);
        m_Target.texture = EditorGUILayout.ObjectField("Texture", m_Target.texture, typeof(Texture2D), false) as Texture2D;
        EditorGUILayout.PropertyField(m_CullingMask);
        serializedObject.ApplyModifiedProperties();

        if (m_Target.m_DepthRenderCamera)
        {
            GUILayout.Space(20);
            Matrix4x4 mat = m_Target.m_DepthRenderCamera.projectionMatrix;
            GUILayout.Label(mat.m00.ToString("f3") + "," + mat.m01.ToString("f3") + "," + mat.m02.ToString("f3") + "," + mat.m03.ToString("f3"));
            GUILayout.Label(mat.m10.ToString("f3") + "," + mat.m11.ToString("f3") + "," + mat.m12.ToString("f3") + "," + mat.m13.ToString("f3"));
            GUILayout.Label(mat.m20.ToString("f3") + "," + mat.m21.ToString("f3") + "," + mat.m22.ToString("f3") + "," + mat.m23.ToString("f3"));
            GUILayout.Label(mat.m30.ToString("f3") + "," + mat.m31.ToString("f3") + "," + mat.m32.ToString("f3") + "," + mat.m33.ToString("f3"));
        }
    }
}
