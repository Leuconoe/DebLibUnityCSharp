using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform), true)]
public class CustomTransformInspector : Editor
{
    static public CustomTransformInspector instance;

    private SerializedProperty mPos;
    private SerializedProperty mRot;
    private SerializedProperty mScale;

    private void OnEnable()
    {
        instance = this;

        mPos = serializedObject.FindProperty("m_LocalPosition");
        mRot = serializedObject.FindProperty("m_LocalRotation");
        mScale = serializedObject.FindProperty("m_LocalScale");
    }

    private void OnDestroy()
    { instance = null; }

    /// <summary>
    /// Draw the inspector widget.
    /// </summary>

    public override void OnInspectorGUI()
    {
        EditorWindowUtil.SetLabelWidth(15f);

        serializedObject.Update();

        bool widgets = false;

        DrawPosition();
        DrawRotation(widgets);
        DrawScale(widgets);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPosition()
    {
        GUILayout.BeginHorizontal();
        bool reset = GUILayout.Button("P", GUILayout.Width(20f));
        EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
        EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
        EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //reset = GUILayout.Button("W", GUILayout.Width(20f));
        //EditorGUILayout.Vector3Field("", (target as Transform).position);

        if (reset) mPos.vector3Value = Vector3.zero;
        //GUILayout.EndHorizontal();
    }

    private void DrawScale(bool isWidget)
    {
        GUILayout.BeginHorizontal();
        bool reset = GUILayout.Button("S", GUILayout.Width(20f));
        bool zIsZero = mScale.FindPropertyRelative("z").floatValue == 0;
        if (isWidget) GUI.color = new Color(0.7f, 0.7f, 0.7f);
        EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
        EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
        if (zIsZero) GUI.color = Color.red;
        EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));
        if (isWidget || zIsZero) GUI.color = Color.white;
        if (zIsZero)
        {
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            EditorGUILayout.HelpBox("some value is invalidate, check please", MessageType.Warning);

            if (GUILayout.Button("reset"))
            {
                reset = true;
            }
            GUILayout.EndVertical();
        }

        if (reset) mScale.vector3Value = Vector3.one;
        if (!zIsZero)
        {
            GUILayout.EndHorizontal();
        }
    }

    #region Rotation is ugly as hell... since there is no native support for quaternion property drawing

    private enum Axes : int
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = 7,
    }

    private Axes CheckDifference(Transform t, Vector3 original)
    {
        Vector3 next = t.localEulerAngles;

        Axes axes = Axes.None;

        if (Differs(next.x, original.x)) axes |= Axes.X;
        if (Differs(next.y, original.y)) axes |= Axes.Y;
        if (Differs(next.z, original.z)) axes |= Axes.Z;

        return axes;
    }

    private Axes CheckDifference(SerializedProperty property)
    {
        Axes axes = Axes.None;

        if (property.hasMultipleDifferentValues)
        {
            Vector3 original = property.quaternionValue.eulerAngles;

            foreach (Object obj in serializedObject.targetObjects)
            {
                axes |= CheckDifference(obj as Transform, original);
                if (axes == Axes.All) break;
            }
        }
        return axes;
    }

    /// <summary>
    /// Draw an editable float field.
    /// </summary>
    /// <param name="hidden">Whether to replace the value with a dash</param>
    /// <param name="greyedOut">Whether the value should be greyed out or not</param>

    private static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt)
    {
        float newValue = value;
        GUI.changed = false;

        if (!hidden)
        {
            if (greyedOut)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
                GUI.color = Color.white;
            }
            else
            {
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
            }
        }
        else if (greyedOut)
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            GUI.color = Color.white;
        }
        else
        {
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
        }

        if (GUI.changed && Differs(newValue, value))
        {
            value = newValue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Because Mathf.Approximately is too sensitive.
    /// </summary>

    private static bool Differs(float a, float b)
    { return Mathf.Abs(a - b) > 0.0001f; }

    private void DrawRotation(bool isWidget)
    {
        GUILayout.BeginHorizontal();
        {
            bool reset = GUILayout.Button("R", GUILayout.Width(20f));

            Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;

            visible.x = EditorWindowUtil.WrapAngle(visible.x);
            visible.y = EditorWindowUtil.WrapAngle(visible.y);
            visible.z = EditorWindowUtil.WrapAngle(visible.z);

            Axes changed = CheckDifference(mRot);
            Axes altered = Axes.None;

            GUILayoutOption opt = GUILayout.MinWidth(30f);

            if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, isWidget, opt)) altered |= Axes.X;
            if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, isWidget, opt)) altered |= Axes.Y;
            if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt)) altered |= Axes.Z;

            if (reset)
            {
                mRot.quaternionValue = Quaternion.identity;
            }
            else if (altered != Axes.None)
            {
                EditorWindowUtil.RegisterUndo("Change Rotation", serializedObject.targetObjects);

                foreach (Object obj in serializedObject.targetObjects)
                {
                    Transform t = obj as Transform;
                    Vector3 v = t.localEulerAngles;

                    if ((altered & Axes.X) != 0) v.x = visible.x;
                    if ((altered & Axes.Y) != 0) v.y = visible.y;
                    if ((altered & Axes.Z) != 0) v.z = visible.z;

                    t.localEulerAngles = v;
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    #endregion Rotation is ugly as hell... since there is no native support for quaternion property drawing
}