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
        m_Target.fade = Mathf.Clamp(EditorGUILayout.FloatField("Fade", m_Target.fade), 0, 4);
        m_Target.color = EditorGUILayout.ColorField("Color", m_Target.color);
        EditorGUILayout.PropertyField(m_BlendMode);
        m_Target.texture = EditorGUILayout.ObjectField("Texture", m_Target.texture, typeof(Texture2D), false) as Texture2D;
        EditorGUILayout.PropertyField(m_CullingMask);
        serializedObject.ApplyModifiedProperties();
    }
}
